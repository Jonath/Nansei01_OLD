using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshPool))]
public class Grid : MonoBehaviour
{
    public float x_size, y_size;
    public float width, height;
    public Sprite block;

    private MeshPool pool;

    private float origin_x;
    private float origin_y;

    void Awake() {
        pool = gameObject.GetComponent<MeshPool>();
        Generate();
    }

    // Simple basic grid
    private void Generate() {
        float x_step = 1 / x_size * width;
        float y_step = 1 / y_size * height;
        for (float x = 0; x <= x_size; x++) {
            for (float y = 0; y <= y_size; y++) {
                float pos_x = - width / 2 + x * x_step;
                float pos_y = - height / 2 + y * y_step;

                pool.AddBullet(block, EType.EFFECT, EMaterial.GUI, new Vector3(pos_x, pos_y));
            }
        }
    }
}
