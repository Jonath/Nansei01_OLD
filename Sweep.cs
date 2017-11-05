using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sweep : MonoBehaviour {
    public List<SpriteRenderer> bomb_parts;
    private Rect bounds;

	// Update is called once per frame
	void Update () {
        float xmin = float.MaxValue;
        float xmax = float.MinValue;
        float ymin = float.MaxValue;
        float ymax = float.MinValue;

        for (int i = 0; i < bomb_parts.Count; ++i) {
            SpriteRenderer sprite = bomb_parts[i];
            float s_xmin = sprite.bounds.min.x;
            float s_xmax = sprite.bounds.max.x;
            float s_ymin = sprite.bounds.min.y;
            float s_ymax = sprite.bounds.max.y;

            if(s_xmin < xmin) {
                xmin = s_xmin;
            }

            if(s_xmax > xmax) {
                xmax = s_xmax;
            }

            if(s_ymin < ymin) {
                ymin = s_ymin;
            }

            if(s_ymax > ymax) {
                ymax = s_ymax;
            }
        }

        bounds = Rect.MinMaxRect(xmin, ymin, xmax, ymax);
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
