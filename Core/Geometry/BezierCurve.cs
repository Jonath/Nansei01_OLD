using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class Path {
    public Vector3[] points;
    public float[] time;

    public Path(int nb_points) {
        points = new Vector3[nb_points];
        time = new float[(points.Length - 1) / 3];
    }
}

[Serializable]
public class BezierCurve : MonoBehaviour {
    [SerializeField]
    private Path path;
    public int linesteps;
	
    public BezierCurve(int l) {
        linesteps = l;
        path = new Path(4);
    }

    public int CurveCount {
        get {
            return (path.points.Length - 1) / 3;
        }
    }

    public int ControlPointCount {
		get {
			return path.points.Length;
		}
	}

    public Vector3 GetControlPoint(int pt_index) {
        return path.points[pt_index];
    }

    public float GetTime(int cr_index) {
        return path.time[cr_index];
    }

    public void SetControlPoint(int index, Vector3 point) {
        path.points[index] = point;
    }

    public void ComputeTimeIndex(ref float t, out int i) {
        // Position the right index and time
        if (t >= 1f) {
            t = 1f;
            i = path.points.Length - 4;
        } else {
            float step = Mathf.Clamp01(t) * CurveCount;
            i = (int)step;
            t = step - i;
            i *= 3;
        }
    }

    public float GetSpeed(float t) {
        int i = 0;
        ComputeTimeIndex(ref t, out i);
        return 1 / path.time[i];
    }

    public Vector3 GetPoint(float t) {
        int i = 0;
        ComputeTimeIndex(ref t, out i);
        return transform.TransformPoint(Bezier.GetPoint(path.points[i], path.points[i+1], path.points[i+2], path.points[i+3], t));
	}
	
	public Vector3 GetVelocity(float t) {
        int i = 0;
        ComputeTimeIndex(ref t, out i);
        return transform.TransformPoint(Bezier.GetFirstDerivative(path.points[i], path.points[i + 1], path.points[i + 2], path.points[i + 3], t)) - transform.position;
	}
	
	public Vector3 GetDirection(float t) {
		return GetVelocity(t).normalized;
	}

    // Compute an estimate of the total length of the bezier curve
    public float TotalLengthEstimate()
    {
        // TODO : change to iterate over all the points
        float chord = (path.points[2] - path.points[0]).magnitude;
        float cont_net = (path.points[1] - path.points[0]).magnitude + (path.points[2] - path.points[1]).magnitude;
        return (cont_net + chord) / 2;
    }

    public void Reset() {
        path.points = new Vector3[] {
            new Vector3(1f, 0f, 0f),
            new Vector3(2f, 0f, 0f),
            new Vector3(3f, 0f, 0f),
            new Vector3(4f, 0f, 0f)
        };
	}

    public void AddCurve() {
        Vector3 point = path.points[path.points.Length - 1];
        Array.Resize(ref path.points, path.points.Length + 3);
        point.x += 1f;
        path.points[path.points.Length - 3] = point;
        point.x += 1f;
        path.points[path.points.Length - 2] = point;
        point.x += 1f;
        path.points[path.points.Length - 1] = point;
    }
}