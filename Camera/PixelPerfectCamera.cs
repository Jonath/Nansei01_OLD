using UnityEngine;

[ExecuteInEditMode]
public class PixelPerfectCamera : MonoBehaviour
{
    public int PixelsPerUnit = 16;

    private int _lastHeight;
    private int _lastPPU;

    public void Update()
    {
        if (_lastHeight == Screen.height && _lastPPU == PixelsPerUnit) return;

        _lastHeight = Screen.height;
        _lastPPU = PixelsPerUnit;

        Camera.main.orthographicSize = Screen.height / (PixelsPerUnit * 2f);
    }
}