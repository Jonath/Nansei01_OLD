using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class QuadTreeHolder : MonoBehaviour {
    public static QuadTree quadtree = null;         // Singleton
    public Rect startingBounds;                     // Use this for initialization

    public Bullet player_obj;
    private Player player;

    private List<Bullet> bullets_buffer;
    private List<Bullet> bullets_close_player;

    public MeshPool bullet_pool; // to remove bullets

    public bool trap;

    [ExecuteInEditMode]
    public void Init() {
        ResetQuadTree();
        bullets_close_player = new List<Bullet>();
        bullets_buffer = new List<Bullet>();
    }

    public void SetPlayer(Player p) {
        player = p;
		player_obj = p.obj;
    }

    public void ResetQuadTree() {
        if(quadtree == null) {
            quadtree = new QuadTree(0, startingBounds, Color.red);
        } else {
            quadtree.Clear();
        }
    }
	
	// Clear and re-add bullets to the QuadTree (maybe logic needs to be sorted out of the quadtree to avoid complexity)
	public void CheckCollisions (List<Bullet> pool_bullets) {
        ResetQuadTree();

        // If player has been removed for some reason, do not handle collisions
        if(player == null) {
            return;
        }

        foreach (Bullet bullet in pool_bullets) {
            quadtree.Insert(bullet);
        }

        bullets_close_player.Clear();
        if(player.can_be_damaged) { // Optimisation
            bullets_close_player = quadtree.Get(player_obj.AABB);

            // For each bullet close to the ennemy
            for(int i = 0; i < bullets_close_player.Count && player.can_be_damaged; ++i) {
                // Enemy collision => player loses a life
                if((bullets_close_player[i].Type == EType.BULLET ||
                    bullets_close_player[i].Type == EType.ENEMY) &&
                    CircleCollision(bullets_close_player[i])) {
                    player.life_bar.Remove();
                    player.StartCoroutine(player._Shield(2f));
                }
            }
        }

        /*else if(player.bombing) {
            // For each bomb component
            for(int i = 0; i < player.bomb_components.Count; ++i) {
                OBB OBB = OBB.GetOBB(player.bomb_components[i]);
                Rect AABB = OBB.GetAABBFromOBB(player.bomb_components[i]);

                bullets_buffer = quadtree.Get(AABB);
                for(int j = 0; j < bullets_buffer.Count; ++j) {
                    if (OBBCollision(OBB, bullets_buffer[j].OBB)) {
                        StartCoroutine(bullet_pool._Fade(bullets_buffer[j], 0, 1.0f));
                    }
                }
            }
        }*/

        // For each enemy referenced in the QuadTree
        /*for (int i = 0; i < enemies.Count; ++i) {
            if(enemies[i].obj != null) {
                // Get a list of bullets overlapping the current enemy bounding rect
                bullets_buffer = quadtree.Get(enemies[i].obj.AABB);
                // For each bullet in the list we just got
                for (int j = 0; j < bullets_buffer.Count; ++j) {
                    if (bullets_buffer[j].Type == EType.SHOT) {
                        enemies[i].life -= bullets_buffer[j].Damage;  // Add the damage of each bullet
                    }
                }
            }
        }*/
    }

	public void CheckCollision(Enemy enemy) {
		if(enemy.obj != null) {
			// Get a list of bullets overlapping the current enemy bounding rect
			bullets_buffer = quadtree.Get(enemy.obj.AABB);

			// For each bullet in the list we just got
			for (int j = 0; j < bullets_buffer.Count; ++j) {
				if (bullets_buffer[j].Type == EType.SHOT) {
					enemy.life -= bullets_buffer[j].Damage;  // Add the damage of each bullet

					if (enemy.life <= 0) {
						enemy.Die();
					}
				}
			}
		}
	}

	// Circle collision (between player and bullet @TODO generalize)
    public bool CircleCollision(Bullet bullet) {
        float diff_x = player_obj.Position.x - bullet.Position.x;
        float diff_y = player_obj.Position.y - bullet.Position.y;

        float dist_sqrt = (diff_x * diff_x) + (diff_y * diff_y);
        float radius_sum = (player_obj.Radius + bullet.Radius);

        return dist_sqrt <= radius_sum * radius_sum;
    }

    // AABB (Axis Aligned Bounding Box) collision (non-oriented rectangles)
    public static bool AABBCollision(float r1_minX, float r1_maxX, float r1_minZ, float r1_maxZ,
                                     float r2_minX, float r2_maxX, float r2_minZ, float r2_maxZ) {
        // If the min of one box in one dimension is greater than the max of another box then the boxes are not intersecting
        return !(r1_minX > r2_maxX || r2_minX > r1_maxX || r1_minZ > r2_maxZ || r2_minZ > r1_maxZ);
    }

    // OBB (Oriented Bounding Box) collision (oriented rectangles)
    public static bool OBBCollision(OBB r1,
                                    OBB r2) {
        // Check AABB then do a SAT
        return AABBFromOBBCollision(r1, r2) && RectangleSATCollision(r1, r2);
    }

    // Find out if OBB rectangles are intersecting by using AABB
    private static bool AABBFromOBBCollision(OBB r1,
                                             OBB r2) {
        // Find the min/max values for the AABB algorithm
        float r1_minX = Mathf.Min(r1.FL.x, Mathf.Min(r1.FR.x, Mathf.Min(r1.BL.x, r1.BR.x)));
        float r1_maxX = Mathf.Max(r1.FL.x, Mathf.Max(r1.FR.x, Mathf.Max(r1.BL.x, r1.BR.x)));
        float r2_minX = Mathf.Min(r2.FL.x, Mathf.Min(r2.FR.x, Mathf.Min(r2.BL.x, r2.BR.x)));
        float r2_maxX = Mathf.Max(r2.FL.x, Mathf.Max(r2.FR.x, Mathf.Max(r2.BL.x, r2.BR.x)));

        float r1_minZ = Mathf.Min(r1.FL.y, Mathf.Min(r1.FR.y, Mathf.Min(r1.BL.y, r1.BR.y)));
        float r1_maxZ = Mathf.Max(r1.FL.y, Mathf.Max(r1.FR.y, Mathf.Max(r1.BL.y, r1.BR.y)));
        float r2_minZ = Mathf.Min(r2.FL.y, Mathf.Min(r2.FR.y, Mathf.Min(r2.BL.y, r2.BR.y)));
        float r2_maxZ = Mathf.Max(r2.FL.y, Mathf.Max(r2.FR.y, Mathf.Max(r2.BL.y, r2.BR.y)));

        return AABBCollision(r1_minX, r1_maxX, r1_minZ, r1_maxZ, r2_minX, r2_maxX, r2_minZ, r2_maxZ);
    }

    // Use SAT (separating axis theorem) to test if to OBB rectangles collide
    public static bool RectangleSATCollision(OBB r1, OBB r2) {
        // Get the normal of r1 left face and check 
        Vector3 normal1 = GetNormal(r1.BL, r1.FL);
        if(!Overlapping(normal1, r1, r2)) {
            return false;
        }

        // Get the normal of r1 forward face and check
        Vector3 normal2 = GetNormal(r1.FL, r1.FR);
        if(!Overlapping(normal2, r1, r2)) {
            return false;
        }

        // Get the normal of r2 left face and check 
        Vector3 normal3 = GetNormal(r2.BL, r2.FL);
        if(!Overlapping(normal3, r1, r2)) {
            return false;
        }

        // Get the normal of r2 forward face and check 
        Vector3 normal4 = GetNormal(r2.FL, r2.FR);
        if (!Overlapping(normal4, r1, r2)) {
            return false;
        }

        // If we get past all the tests, then there is collision
        return true;
    }

    // Get the normal of a face from 2 points
    private static Vector3 GetNormal(Vector3 startPos, Vector3 endPos) {
        // Direction (sense is arbitrary)
        Vector3 dir = endPos - startPos;

        // Normal, just flip x and y and make one negative (don't need to normalize it)
        Vector3 normal = new Vector3(-dir.y, dir.x, dir.z);

        // Draw the normal from the center of the rectangle's side (TODO : add trap)
        Debug.DrawRay(startPos + (dir * 0.5f), normal.normalized * 2f, Color.red);

        return normal;
    }

    // Is this side overlapping ?
    private static bool Overlapping(Vector3 normal, OBB r1, OBB r2) {
        // Project the corners of rectangle 1 onto the normal
        float dot1 = DotProduct(normal, r1.FL);
        float dot2 = DotProduct(normal, r1.FR);
        float dot3 = DotProduct(normal, r1.BL);
        float dot4 = DotProduct(normal, r1.BR);

        // Find the range
        float min1 = Mathf.Min(dot1, Mathf.Min(dot2, Mathf.Min(dot3, dot4)));
        float max1 = Mathf.Max(dot1, Mathf.Max(dot2, Mathf.Max(dot3, dot4)));

        // Project the corners of rectangle 2 onto the normal
        float dot5 = DotProduct(normal, r2.FL);
        float dot6 = DotProduct(normal, r2.FR);
        float dot7 = DotProduct(normal, r2.BL);
        float dot8 = DotProduct(normal, r2.BR);

        // Find the range
        float min2 = Mathf.Min(dot5, Mathf.Min(dot6, Mathf.Min(dot7, dot8)));
        float max2 = Mathf.Max(dot5, Mathf.Max(dot6, Mathf.Max(dot7, dot8)));

        // Are the ranges overlapping ?
        return (min1 <= max2 && min2 <= max1);
    }

    // Get the dot product
    private static float DotProduct(Vector3 v1, Vector3 v2) {
        return v1.x * v2.x + v1.y * v2.y;
    }

    // Project a 2D polygon (unused)
    public void ProjectPolygon(Vector2 axis, Vector2[] polygon,
                               ref float min, ref float max)
    {
        // To project a point on an axis use the dot product
        float dotProduct = Vector2.Dot(axis, polygon[0]);
        min = dotProduct;
        max = dotProduct;

        for (int i = 0; i < polygon.Length; i++) {
            dotProduct = Vector2.Dot(polygon[i], axis);
            if (dotProduct < min) {
                min = dotProduct;
            } else if (dotProduct > max) {
                max = dotProduct;
            }
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(startingBounds.center, startingBounds.size);

        List<Bullet> bullets = bullet_pool.GetBullets();

        for (int i = 0; i < bullets.Count; i++) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(bullets[i].AABB.center, bullets[i].AABB.size);

            Gizmos.color = Color.yellow;
            OBB OBB = bullets[i].OBB;
            Gizmos.DrawLine(OBB.FL, OBB.FR);
            Gizmos.DrawLine(OBB.FR, OBB.BR);
            Gizmos.DrawLine(OBB.BR, OBB.BL);
            Gizmos.DrawLine(OBB.BL, OBB.FL);
        }

        if(player_obj != null) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(player_obj.AABB.center, player_obj.AABB.size);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player_obj.Position, player_obj.Radius);
        }
    }
}
