using UnityEngine;

public class Line : MonoBehaviour {
	public Vector3 p0, p1;

    void Awake()
    {
        LineRenderer line_renderer = GetComponent<LineRenderer>();
        line_renderer.SetPosition(0, transform.InverseTransformPoint(p0));
        line_renderer.SetPosition(1, transform.InverseTransformPoint(p1));
    }
}