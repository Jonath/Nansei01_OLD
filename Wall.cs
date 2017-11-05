using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {
    private bool _attached;
    private Vector3 cached_position;
    private Vector3 cached_scale;

    private Bounds bounds;
    private float width;
    private float height;

    public SpriteRenderer sprite_renderer;
    public enum ESide { LEFT, RIGHT, UP, DOWN };

	// Use this for initialization
	void Start () {
        bounds = sprite_renderer.bounds;
        width = bounds.extents.x;
        height = bounds.extents.y;
    }

    // Update is called once per frame
    void Update () {
        transform.position = cached_position;
        transform.localScale = cached_scale;
	}

    public void Attach(Vector3 pos, float size, ESide side) {
        switch(side) {
            case ESide.LEFT:
                cached_scale = new Vector3(1, size / height);
                cached_position = pos - new Vector3(width, 0);
                break;
            case ESide.RIGHT:
                cached_position = pos + new Vector3(width, 0);
                cached_scale = new Vector3(1, size / height);
                break;
            case ESide.UP:
                cached_position = pos + new Vector3(0, height);
                cached_scale = new Vector3(size / width, 1);
                break;
            case ESide.DOWN:
                cached_position = pos - new Vector3(0, height);
                cached_scale = new Vector3(size / width, 1);
                break;
        }
    }
}
