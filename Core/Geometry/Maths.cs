using UnityEngine;
using System.Collections;

public static class Maths {
    public static Vector3 Clamping(Vector3 vec, Vector3 min, Vector3 max) {
        return Vector3.Min(max, Vector3.Max(min, vec));
    }

    public static Vector3 Rotate(Vector3 vertex, Vector3 center, Vector3 angle) {
        Quaternion rotation = Quaternion.Euler(angle);
        return rotation * (vertex - center) + center;
    }
}
