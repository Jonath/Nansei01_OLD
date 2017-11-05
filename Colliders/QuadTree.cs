using UnityEngine;
using System.Collections;
using System.Collections.Generic; 

public class QuadTree {
	private int level;

	// Objects contained in terms of rect (should it be there ?)
	private List<Bullet> objects;

	// Bouding rectangle for current level
    [System.NonSerialized]
	public Rect rect;

	// Children nodes for descending it.
    [System.NonSerialized]
	public QuadTree[] nodes;

    // Color to draw the QuadTree
    private Color color;

    /// <summary>
    /// Return the contents of this node and all subnodes in the true below this one.
    /// </summary>
    public List<Bullet> SubTreeContents
    {
        get
        {
            List<Bullet> results = new List<Bullet>();

            foreach (QuadTree node in nodes) { 
                if(node != null) { 
                    results.AddRange(node.SubTreeContents);
                }
            }

            results.AddRange(objects);
            return results;
        }
    }

    public QuadTree(int pLevel, Rect pBounds, Color _color)
	{
		objects = new List<Bullet>();
		nodes = new QuadTree[4];

		level = pLevel;
		rect = pBounds;
        color = _color;
	}

    // Debugging the Quadtree
    public void Draw()
    {
        Vector3 a = new Vector3(rect.x, rect.y, 0);
        Vector3 b = new Vector3(rect.x + rect.width, rect.y, 0);
        Vector3 c = new Vector3(rect.x, rect.y + rect.height, 0);
        Vector3 d = new Vector3(rect.x + rect.width, rect.y + rect.height, 0);
        Debug.DrawLine(a, b, color, 1);
        Debug.DrawLine(a, c, color, 1);
        Debug.DrawLine(c, d, color, 1);
        Debug.DrawLine(b, d, color, 1);
    }
 
	// Clear quadtree
	public void Clear()
	{
		objects.Clear();
		for(int i = 0; i < nodes.Length; i++)
		{
			// If there is a node, clear it, then delete it
			if(nodes[i] != null) {
				nodes[i].Clear();
				nodes[i] = null;
			}
		}
	}

	// Split the node into 4 subnodes
	private void Split()
	{
		float subWidth = (rect.width / 2);
		float subHeight = (rect.height / 2);

		float x = rect.x;
		float y = rect.y;

		nodes[0] = new QuadTree(level + 1, new Rect(x + subWidth, y + subHeight, subWidth, subHeight), color);  // Top right
		nodes[1] = new QuadTree(level + 1, new Rect(x, y + subHeight, subWidth, subHeight), color);				// Top left	
		nodes[2] = new QuadTree(level + 1, new Rect(x, y, subWidth, subHeight), color);							// Bottom left
		nodes[3] = new QuadTree(level + 1, new Rect(x + subWidth, y, subWidth, subHeight), color);		        // Bottom right
	}

	// Insert the object into the Quadtree
	public void Insert(Bullet bullet)
	{
        // if the item is not at least partially contained in this quad, don't add it
        if (!rect.Overlaps(bullet.AABB)) {
            return;
        }

        // If the subnodes are null create them.
        if (nodes[0] == null) {
            Split();
        }

        // For each subnode:
        // If the node contains the item, add the item to that node and return
        // This recurses into the node that is just large enough to fit this item
        foreach (QuadTree node in nodes) {
            if (Contains(node.rect, bullet.AABB)) {
                node.Insert(bullet);
                return;
            }
        }

        // If we make it to here, either
        // 1) None of the subnodes completely contained the item. or
        // 2) We're at the smallest subnode size allowed 
        // Add the item to this node's contents in both cases.
        objects.Add(bullet);
    }

    // Reference the items in the rectangle pRect into the list bullets
    public List<Bullet> Get(Rect pRect) {
		return Retrieve(pRect);
	}

    // Get the items in a rectangle
    private List<Bullet> Retrieve(Rect pRect)
	{
        List<Bullet> bullets = new List<Bullet>();

        // This quad contains items that are not entirely contained by
        // it's four sub-quads. Iterate through the items in this quad 
        // to see if they intersect.
        foreach (Bullet bullet in objects)
        {
            if (pRect.Overlaps(bullet.AABB)) { 
                bullets.Add(bullet);
            }
        }

        foreach (QuadTree node in nodes)
        {
            if(node == null) {
                continue;
            }

            // Case 1: search area completely contained by sub-quad
            // if a node completely contains the query area, go down that branch
            // and skip the remaining nodes (break this loop)
            if (Contains(node.rect, pRect)) {
                bullets.AddRange(node.Retrieve(pRect));
                break;
            }

            // Case 2: Sub-quad completely contained by search area 
            // if the query area completely contains a sub-quad,
            // just add all the contents of that quad and it's children 
            // to the result set. You need to continue the loop to test 
            // the other quads
            if (Contains(pRect, node.rect)) {
                bullets.AddRange(node.SubTreeContents);
                continue;
            }

            // Case 3: search area intersects with sub-quad
            // traverse into this quad, continue the loop to search other
            // quads
            if (node.rect.Overlaps(pRect)) {
                bullets.AddRange(node.Retrieve(pRect));
            }
        }

        return bullets;
    }

    private bool Contains(Rect r1, Rect r2) {
        return r1.Contains(r2.min) && r1.Contains(r2.max);
    }
}

