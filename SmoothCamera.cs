using UnityEngine;
using System.Collections;

public class SmoothCamera : MonoBehaviour {
    public static SmoothCamera instance = null;

    // Damp time to move left / right
    public float dampTime = 0.15f;

    // Internal objects
    public Transform target;
    public Camera main_camera;

    /*public Wall left_wall;
    public Wall right_wall;
    public Wall up_wall;
    public Wall down_wall;*/

    public bool active = false;

    private Vector3 velocity = Vector3.zero;
    public Bounds camera_bounds;

    void Awake() {
        // Set instance to this object
        if (instance == null) {
            instance = this;
        }

        // Enforce there can be only one instance
        else if (instance != this) {
            Destroy(gameObject);
        }

        camera_bounds = CameraExtensions.OrthographicBounds(Camera.main);
        /*left_wall.Attach(new Vector3(-camera_bounds.extents.x, 0), camera_bounds.extents.y, Wall.ESide.LEFT);
        right_wall.Attach(new Vector3(camera_bounds.extents.x, 0), camera_bounds.extents.y, Wall.ESide.RIGHT);
        up_wall.Attach(new Vector3(0, camera_bounds.extents.y), camera_bounds.extents.x, Wall.ESide.UP);
        down_wall.Attach(new Vector3(0, -camera_bounds.extents.y), camera_bounds.extents.x, Wall.ESide.DOWN);*/
    }

    // Update is called once per frame
    void Update() {
        if (target && active) {
            Vector3 point = main_camera.WorldToViewportPoint(target.position);
            Vector3 delta = target.position - main_camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
            Vector3 destination = transform.position + delta;
            transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
        }
    }
}
