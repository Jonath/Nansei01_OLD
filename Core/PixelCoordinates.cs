using UnityEngine;
using System.Collections;

[System.Serializable]
public struct PixelCoordinates {
    [SerializeField]
    private Vector3 position;

    public Vector3 Position {
        get {
			return position;
		}
    }

    public static PixelCoordinates FromOffsetCoordinates(Vector3 position) {
        return new PixelCoordinates(position);
    }

    public PixelCoordinates(Vector3 position) {
        this.position = Camera.main.WorldToScreenPoint(position);
    }

    public override string ToString() {
        return "(" + position.x.ToString() + ", " + position.y.ToString() + ", " + position.z.ToString() + ")";
    }
}
