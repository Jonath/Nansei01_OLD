using UnityEngine;

public struct OBB 
{
    // Corners
    public Vector3 FL, FR, BL, BR;

    public OBB(Vector3 FL, Vector3 FR, Vector3 BL, Vector3 BR)
    {
        this.FL = FL;
        this.FR = FR;
        this.BL = BL;
        this.BR = BR;
    }

    // Find the corners of a rectangle transform
    public static void GetCornerPositions(Transform r, out Vector3 FL, out Vector3 FR, out Vector3 BL, out Vector3 BR)
    {
        FL = r.position + r.up * r.localScale.y * 0.5f - r.right * r.localScale.x * 0.5f;
        FR = r.position + r.up * r.localScale.y * 0.5f + r.right * r.localScale.x * 0.5f;
        BL = r.position - r.up * r.localScale.y * 0.5f - r.right * r.localScale.x * 0.5f;
        BR = r.position - r.up * r.localScale.y * 0.5f + r.right * r.localScale.x * 0.5f;
    }

    public static OBB GetOBB(Transform r)
    {
        Vector3 FL; Vector3 FR; Vector3 BL; Vector3 BR;
        GetCornerPositions(r, out FL, out FR, out BL, out BR);
        return new OBB(FL, FR, BL, BR);
    }

    // Find the corners of a rectangle transform
    public static Rect GetAABBFromOBB(Transform r)
    {
        Vector3 FL; Vector3 FR; Vector3 BL; Vector3 BR;
        GetCornerPositions(r, out FL, out FR, out BL, out BR);
        Vector3 size = new Vector3(BR.x - BL.x, FL.y - BL.y);
        return new Rect(BL, size);
    }
}