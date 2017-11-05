using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TerrainManager : MonoBehaviour {
    public Terrain terrain;
    public new Camera camera;

    void Start () {
        transform.position = new Vector3(camera.transform.position.x - terrain.terrainData.size.x / 2,
                                         transform.position.y,
                                         transform.position.z);
	}
}
