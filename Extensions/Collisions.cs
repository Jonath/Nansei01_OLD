using UnityEngine;
using System.Collections;

public static class Collisions {
    public static bool AABB(Vector3 obj1_pos, Vector3 obj2_pos, Vector3 obj1_ext, Vector3 obj2_ext)
    {
        return (obj1_pos.x + obj1_ext.x + obj2_ext.x > obj2_pos.x &&
                obj1_pos.x < obj2_pos.x + obj1_ext.x + obj2_ext.x &&
                obj1_pos.y + obj1_ext.y + obj2_ext.y > obj2_pos.y &&
                obj1_pos.y < obj2_pos.y + obj1_ext.y + obj2_ext.y);
    }

    public static bool Circle(Vector3 obj1_pos, Vector3 obj2_pos, float obj1_rad, float obj2_rad)
    {
        float distance2 = (obj1_pos.x - obj2_pos.x) * (obj1_pos.x - obj2_pos.x) +
                          (obj1_pos.y - obj2_pos.y) * (obj1_pos.y - obj2_pos.y);

        float radii_sum = obj1_rad * obj1_rad + obj2_rad * obj2_rad + 2 * obj1_rad * obj2_rad;
        return distance2 < radii_sum;
    }
}
