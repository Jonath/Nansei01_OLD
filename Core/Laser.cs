using UnityEngine;
using System.Collections;

public class Laser : Composite {
    public BezierCurve curve;

    public float length;
    public float width;

    public void CopyData(Laser prefab) {
        base.CopyData(prefab);
        length = prefab.length;
        width = prefab.width;
    }

    // /!\ This is a constructor so we may have to get rid of it for perf. reasons.
    public Laser(BezierCurve bezier, float len, float wid) {
        curve = bezier;
        length = len;
        width = wid;
    }

    public new float Width(float t) {
        float x = t * Mathf.PI;
        return Mathf.Sin(x) * width;
    }

    public override void ComputePosition(Vector3[] _vertices, Color32[] _colors, float dt = 0) {
        float total_length = curve.TotalLengthEstimate();
        float factor = length / total_length;

        UpdateState(dt);

        for (int i = 0; i < _nb_composites; ++i) {
            int vert_idx = _composites[i].Index * 4;

            // Compute left side parameters
            float t0 = (i / (float)_nb_composites);
            float ct0 = t0 * factor + CurrentTime;
            Vector3 pos0 = curve.GetPoint(ct0);
            Vector3 tan0 = curve.GetDirection(ct0);
            Vector3 norm0 = Vector3.Cross(tan0, Vector3.forward);
            pos0 += tan0 * _composites[i].Speed;
            float width0 = Width(t0);

            // Compute right side parameters
            float t1 = ((i + 1) / (float)_nb_composites);
            float ct1 = t1 * factor + CurrentTime;
            Vector3 pos1 = curve.GetPoint(ct1);
            Vector3 tan1 = curve.GetDirection(ct1);
            Vector3 norm1 = Vector3.Cross(tan1, Vector3.forward);
            pos1 += tan1 * _composites[i].Speed;
            float width1 = Width(t1);

            _composites[i].Radius = (width0 + width1) / 2;

            // Set mesh pool vertices
            _vertices[vert_idx] = pos0 - norm0 * width0 / 2;
            _vertices[vert_idx + 1] = pos0 + norm0 * width0 / 2;
            _vertices[vert_idx + 2] = pos1 + norm1 * width1 / 2;
            _vertices[vert_idx + 3] = pos1 - norm1 * width1 / 2;

            CurrentTime += dt * curve.GetSpeed(CurrentTime);
        }
    }
}
