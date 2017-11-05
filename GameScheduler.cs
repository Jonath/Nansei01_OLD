using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class GameScheduler : MonoBehaviour
{
    public static GameScheduler instance = null;     // Singleton

    //public Transform origin;
    public Transform background;

    public MeshPool meshpool;
    public List<Enemy> enemies;
    public Player player;
    public Border border;
    public Dialogue dialogue;
    public Bar lifebar;
    public Bar bombbar;
    public SpriteScaler lifetext;
    public SpriteScaler bombtext;
    public QuadTreeHolder quadtree;

    private Camera cam;
    private float t = 0f;
    private float accumulator = 0;
    private float interpolation = 0;

    private static Vector3 default_resolution = new Vector3(640, 480);

    public static float dt = 0.01f; // Simulation delta time
    public static float cam_height;
    public static float cam_width;
    public static Vector3 origin;
    public static float scale_factor;

    public bool InDialogue;

    // Charged to initialize every other script in the right order.
    [ExecuteInEditMode]
    void OnEnable() {
        // Set instance to this object
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }

        InitializeCameraSettings();

        // Initialize quadtree and meshpool
        quadtree.Init();
        meshpool.Init();

        border.Init();
        border.obj.Scale = new Vector3(scale_factor, scale_factor);
        //origin.position = border.obj.OBB.FL;

        background.position = ComputePosition(new Vector3(-96, 0, 10));
        background.localScale = ComputeScale(new Vector3(384, 448));

        dialogue.Init();

        // Initialize player
        player.Init();

        // Initialize bomb and life bars
        lifebar.Init();
        bombbar.Init();
        lifetext.Init();
        bombtext.Init();

        // Initialize enemies
        for (int i = 0; i < enemies.Count; ++i){
            enemies[i].Init();
        }

        meshpool.UpdateAt(0);
    }

    void Update() {
        player.UpdateAt();
        //dialogue.UpdateAt();

        float frameTime = Time.deltaTime;
        accumulator += frameTime;

        while(accumulator >= dt) {
            meshpool.UpdateAt(dt);			// Movement
			meshpool.ReferenceBullets();	// Reference bullets for collisions

			// Collisions checks
			player.UpdateAt();
			for (int i = 0; i < enemies.Count; ++i) {
				enemies[i].UpdateAt(dt);
			}

            accumulator -= dt;
            t += dt;
        }

        /*float alpha = accumulator / dt;
        meshpool.PrepareRendering(alpha);*/
    }

    void InitializeCameraSettings() {
        cam = Camera.main;
        cam_height = 2f * cam.orthographicSize;
        cam_width = cam_height * cam.aspect;

        scale_factor = Camera.main.orthographicSize / 240;
        origin = new Vector3(-cam_width / 2f, cam_height / 2f); // Origin is top left
    }

    // Compute positions, and scale for different resolutions
    public static Vector3 ComputePosition(Vector3 position) {
        Vector3 rapport = new Vector3(position.x / default_resolution.x,
                                      position.y / default_resolution.y);

        position = new Vector3(rapport.x * cam_width, rapport.y * cam_height);
        return position;
    }

    public static Vector3 ComputeScale(Vector3 scale) {
        return scale * scale_factor;
    }
}
