using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Health : MonoBehaviour {
    public Image life_bar_empty;
    public Image life_bar_full;

    public RectTransform life_bar;

    public AudioClip damageSound;
    public AudioClip healSound;

    public int startingHealth;

    [System.NonSerialized]
    public int currentHealth;

    private float maxX;
    private float minX;

	void Awake () {
        maxX = life_bar.position.x;
        minX = life_bar.position.x - life_bar.rect.width;
        currentHealth = startingHealth;
    }

    public void Heal(int heal) {
        currentHealth = Mathf.Min(currentHealth + heal, startingHealth);
        float currentXValue = MapValues(currentHealth, 0, startingHealth, minX, maxX);
        AudioManager.instance.PlaySingle(healSound);
        life_bar.localPosition = new Vector3(currentXValue, 0);
    }

    public void Damage(int damage, bool silenced = false)
    {
        currentHealth = Mathf.Max(0, currentHealth - damage);
        //float currentXValue = MapValues(currentHealth, 0, startingHealth, minX, maxX);

        if (currentHealth > 0 && !silenced) {
            AudioManager.instance.PlaySingle(damageSound);
        }
    }

    public void Show()
    {
        life_bar_empty.enabled = true;
        life_bar_full.enabled = true;
    }

    public void Hide()
    {
        life_bar_empty.enabled = false;
        life_bar_full.enabled = false;
    }

    private float MapValues (float x, float inMin, float inMax, float outMin, float outMax) {
        return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
}
