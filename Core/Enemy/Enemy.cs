using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Enemy : Entity {
    public MeshPool bullet_pool;    // The mesh pool for bullets
    public MeshPool effect_pool;    // The mesh pool for special effects

    public Sprite death_effect;     // The effect used for coroutine DeathEffect
    public Color death_color;       // Color used for coroutine DeathEffect
	public float life;              // The amount of life this enemy has

    private bool dead;

    [ExecuteInEditMode]
    public override void Init() {
        base.Init();

        if(obj != null) {
			dead = false;
        }
    }

    public void Die() {
        if(!dead) {
            pool.RemoveBullet(obj);

			StopAllCoroutines();
            //StartCoroutine(_DeathExplotion());
            dead = true;
        }
    }

	public void UpdateAt(float dt) {
		pool.QuadTreeHolder.CheckCollision(this);
	}

    public IEnumerator _DeathExplotion()
    {
        Bullet effect = effect_pool.AddBullet(death_effect, EType.EFFECT, EMaterial.GUI, obj.Position); // TODO : replace with appropriate enemy material / texture

        float scale = 1;
        float spin = 0;
        byte alpha = 25;

        effect.Sprite = sprite;
        float angleX = Random.Range(0, 45);
        float angleY = Random.Range(0, 45);

        while (scale > 0)
        {
            effect.Color = death_color;
            effect.SpriteAngle = new Vector3(angleX, angleY, spin);
            effect.Scale = new Vector3(scale, scale);

            scale -= 0.01f;
            if(alpha > 0) alpha--;
            spin+=5f;

            yield return new WaitForFixedUpdate();
        }

        effect_pool.RemoveBullet(effect);
    }
}
