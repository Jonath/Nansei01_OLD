using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Resolution : MonoBehaviour {
    private int width = 640;
    private int height = 480;

	void Start () {
        Screen.SetResolution(width, height, true);
        Camera.main.pixelRect = new Rect(Screen.width, Screen.height, width, height);
    }
}
