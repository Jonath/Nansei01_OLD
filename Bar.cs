using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Bar : MonoBehaviour {
    public MeshPool pool;

    public int max = 8;
    public int current = 2;
    public int fragment = 0;

    public Sprite full;
    public Sprite empty;

    public Vector3 start;

    private Bullet[] frame;

    // Use this for initialization
    public void Init () {
        if(gameObject.activeInHierarchy) {
            // Setting start position and scale
            Vector3 scale = GameScheduler.ComputeScale(transform.localScale);

            frame = new Bullet[max];
            for(int i = 0; i < max; i++) {
                Vector3 position = GameScheduler.ComputePosition(new Vector3(transform.position.x + i * 20, transform.position.y));
                // Add lives full
                frame[i] = pool.AddBullet(empty, EType.EFFECT, EMaterial.GUI, position);
                frame[i].Scale = scale;
            }
                
            UpdateBar();
        }
    }

    public void UpdateBar()
    {
        for (int i = 0; i < current; i++)
        {
            frame[i].Bounds = full.bounds;
            frame[i].UVs = full.rect;
            pool.UpdateBulletAppearance(frame[i]);
        }

        for(int i = current; i < max; i++) {
            frame[i].Bounds = empty.bounds;
            frame[i].UVs = empty.rect;
            pool.UpdateBulletAppearance(frame[i]);
        }

        //float remain = current - (int)current;
    }
    
    public void Remove() {
        current = Mathf.Max(0, current - 1);
        UpdateBar();
    }
}
