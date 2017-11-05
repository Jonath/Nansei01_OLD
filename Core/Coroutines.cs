using UnityEngine;
using System.Collections;

public partial class Bullet : ScriptableObject {
    public IEnumerator _Fade(float aValue, float aTime) {
        float alpha = Color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color32 newColor = new Color32(255, 255, 255, (byte)Mathf.Lerp(alpha, aValue, t)); // @TODO replace lerp
            Color = newColor;
            yield return null;
        }

        yield break;
    }
}
  