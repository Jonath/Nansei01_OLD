using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Brush : MonoBehaviour {
    // Material to use
    public Material material;

    // Max spacing between two consecutive points
    public float spacing;

    // Points the path is made of
    List<Vector2> points;

    // Line renderers used to draw
    List<LineRenderer> lines;

    // Current line renderer
    LineRenderer line_renderer;

    // Pointer to current point
    Vector2 last_point;

    // Current mouse position
    Vector2 mouse_position;

    // Mouse pressed flag
    bool mouse_pressed;

    // Setting the tmpTexture
    void Start()
    {
        // Initialize the points list
        points = new List<Vector2>();

        // Initiaze the line list
        lines = new List<LineRenderer>();

        // Initiliaze a line
        AddLine();

        // Initialize the mouse down flag
        mouse_pressed = false;

        AddLine();
    }

    void AddLine()
    {
        GameObject child = new GameObject();
        child.transform.parent = transform;

        points.Clear();

        line_renderer = child.AddComponent<LineRenderer>();
        line_renderer.material = material;
        line_renderer.material.mainTexture = material.mainTexture;
        line_renderer.SetWidth(0.1f, 0.1f);
        line_renderer.useWorldSpace = true;
        line_renderer.SetVertexCount(0);
        line_renderer.SetColors(Color.white, Color.white);

        lines.Add(line_renderer);
    }

    void Update()
    {
        // If mouse button down, reinitialize line renderer
        if (Input.GetMouseButtonDown(0)) {
            mouse_pressed = true;
        }

        // If it is up, update the flag
        if (Input.GetMouseButtonUp (0)) {
            mouse_pressed = false;
            AddLine();
        }

        // Drawing line when mouse is pressed
        if (mouse_pressed) {
            mouse_position = InputManager.GetMousePosition();

            if(points.Count == 0) {
                points.Add(mouse_position);
                line_renderer.SetVertexCount(1);
            }

            // If distance between current position and last point is bigger than threshold, add new point
            else if (Vector2.Distance(mouse_position, last_point) > spacing)
            {
                points.Add(mouse_position);
                last_point = mouse_position;

                // Update the line renderer
                line_renderer.SetVertexCount(points.Count);
            }

            line_renderer.SetPosition(points.Count - 1, points[points.Count - 1]);
        }
    }
}