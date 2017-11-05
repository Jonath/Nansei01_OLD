using UnityEngine;
using System.Collections;

namespace Collision
{
    public static class Extension {
        public static Vector2 Rotate(this Vector2 v, float degrees){
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            float tx = v.x;
            float ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }
    }

    public struct Line {
        public Vector2 start;
        public Vector2 end;

        public Line(Vector2 A, Vector2 B) {
            start = A; end = B;
        }

        public Vector2 Direction() {
            return (start - end).normalized;
        }

        public Vector2 Normal() {
            Vector2 direction = Direction();
            return new Vector2(-direction.y, direction.x);
        }

        // General equation of the line in the form Y = A * X + B
        public void Equation(out float A, out float B) {
            A = (end.y - start.y) / ( end.x - start.x);
            B = start.y - A * start.x;
        }

        // Projection of a point on the line
        public Vector2 Project(Vector2 p) {
            return Vector3.Project((p - start), (end - start)) + (Vector3)start;
        }
    }

    public struct Circle {
        public float radius;
        public Vector2 center;
    }

    public struct Cone {
        public float start_angle;
        public float end_angle;
        public Circle circle;

        public void Lines(out Vector2 start, out Vector2 end) {
            Vector2 upr = circle.center + Vector2.up * circle.radius;
            start = upr.Rotate(start_angle);
            end = upr.Rotate(end_angle);
        }
    }

    public class Collisions
    {
        // Cached variables
        Line l1 = new Line();
        Line l2 = new Line();

        // Line / Line collision
        public bool Collision(Line l1, Line l2)
        {
            Vector2 p1 = new Vector2(l1.start.x, l1.start.y);
            Vector2 p2 = new Vector2(l1.end.x, l1.end.y);

            Vector2 p3 = new Vector2(l2.start.x, l2.start.y);
            Vector2 p4 = new Vector2(l2.end.x, l2.end.y);

            float denominator = (p4.y - p3.y) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.y - p1.y);

            // If the equation has a real solution
            if(denominator >= 0) { 
                float u_a = ((p4.x - p3.x) * (p1.y - p3.y) - (p4.y - p3.y) * (p1.x - p3.x)) / denominator;
                float u_b = ((p2.x - p1.x) * (p1.y - p3.y) - (p2.y - p1.y) * (p1.x - p3.x)) / denominator;

                // There is intersection if u_a and u_b are between 0 and 1
                return (u_a >= 0 && u_a <= 1) && (u_b >= 0 && u_b <= 1);
            }

            return false;
        }

        // Line / Circle
        public bool Collision(Circle c, Line l) {
            Vector2 AB = l.end - l.start;
            Vector2 AC = c.center - l.start;
            float length = AB.magnitude;

            float proj_length = Vector2.Dot(AC, AB / length);
            Vector2 proj = proj_length * AB / length;

            // Closest segment point to circle 
            Vector2 D;
            if(proj_length < 0) {
                D = l.start;
            } else if(proj_length > length) {
                D = l.end;
            } else {
                D = l.start + proj;
            }

            Vector2 dist = c.center - D;
            return dist.sqrMagnitude < c.radius * c.radius; 
        }

        // Circle / Circle collision
        public bool Collision(Circle c1, Circle c2) {
            float diff_x = c1.center.x - c2.center.x;
            float diff_y = c2.center.y - c2.center.y;

            float dist_sqrt = (diff_x * diff_x) + (diff_y * diff_y);
            float radius_sum = (c1.radius + c2.radius);

            return dist_sqrt <= radius_sum * radius_sum;
        }

        // Cone / Circle collision
        public bool Collision(Cone cone, Circle circle)
        {
            // Do the circles collide
            bool CircleCollide = Collision(cone.circle, circle);

            // Does the circle intersect one of the lines of the cone
            Vector2 start; Vector2 end;
            cone.Lines(out start, out end);
            l1.start = l2.start = cone.circle.center;
            l1.end = start; l2.end = end;
            bool SegmentCollide = Collision(circle, l1) || Collision(circle, l2);

            // See if the angles between center of cone and center of circle lies between cone.start_angle and cone.end_angle
            float circle_angles = Vector2.Angle(circle.center, cone.circle.center);
            bool AngleCollide = (circle_angles > cone.start_angle) && (circle_angles < cone.end_angle);

            return CircleCollide && (SegmentCollide || AngleCollide);
        }
    }
}
