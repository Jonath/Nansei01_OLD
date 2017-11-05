using UnityEngine;
using System.Collections;

public class CameraScroll : MonoBehaviour {
    public Terrain terrain_scrolling;
    public int time_scroll;
    public float length;

    private Vector3 start;
    private Vector3 end;
    private float timeStartedLerping;

	// Use this for initialization
	void Start () {
        timeStartedLerping = Time.time;
        start = transform.position;
        end = start + new Vector3(0, 0, length);
    }
	
	// Update is called once per frame
	void LateUpdate () {
        float timeSinceStarted = Time.time - timeStartedLerping;
        float completion = timeSinceStarted / time_scroll;
        transform.position = Vector3.Lerp(start, end, completion);

        // Reset
        if (transform.position == end) {
            transform.position = start;
            timeStartedLerping = Time.time;
        }
    }
}
