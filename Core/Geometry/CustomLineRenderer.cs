using System;
using UnityEngine;
using System.Collections;

public class CustomLineRenderer : MonoBehaviour
{
    private Vector3[] vertices = new Vector3[0] { };
    public LineRenderer line_renderer;

    public void SetPosition(int index, Vector3 position) {
        vertices[index] = position;
        line_renderer.SetPosition(index, position);
    }

    public void SetVertexCount(int count) {
        Array.Resize(ref vertices, count);
        line_renderer.SetVertexCount(count);
    }

    public Vector3 GetPosition(int index) {
        return vertices[index];
    }
}
