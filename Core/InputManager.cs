using UnityEngine;
using System.Collections;

public static class InputManager {
    public static bool IsConfirming() {
        return Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Z);
    }

    public static bool IsDon() {
        return Input.GetKeyDown(KeyCode.D);
    }

    public static bool IsKat() {
        return Input.GetKeyDown(KeyCode.K);
    }

    public static Vector2 GetMousePosition() {
        return Camera.main.ScreenToWorldPoint(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
    }
}
