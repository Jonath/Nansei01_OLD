using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class Enemy : Entity {
	public Sprite power_item;
	public Sprite point_item;

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
			StartCoroutine(_DropItems(1, 1));
            //StartCoroutine(_DeathExplotion());
            dead = true;
        }
    }

	public void UpdateAt(float dt) {
		pool.QuadTreeHolder.CheckCollision(this);
	}

    public IEnumerator _DeathExplotion() {
		Bullet effect = pool.AddBullet(death_effect, EType.EFFECT, EMaterial.GUI, obj.Position); // TODO : replace with appropriate enemy material / texture

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

		pool.RemoveBullet(effect);
    }

	public IEnumerator _DropItems(int nbPowerItems, int nbPointItems) {
		int spriteAngularSpeed = 2;
		for (int i = 0; i < nbPowerItems; ++i) {
			SpawnItem (power_item);
		}

		for (int i = 0; i < nbPowerItems; ++i) {
			SpawnItem (point_item);
		}

		yield return new WaitForFixedUpdate ();
	}

	private void SpawnItem(Sprite sprite) {
		float ang = 90;
		Vector3 pos = obj.Position + Vector3.forward * 10;
		Bullet item = pool.AddBullet(sprite, EType.ITEM, EMaterial.BULLETADD,
								     pos, 50, ang, -1);
		item.SpriteAngle = Vector3.forward * (ang - 90);
		item.SpriteAngularVelocity = Vector3.forward * 2;;
		StartCoroutine(item._Change(0.75f, null, null, 0, null));
	}
}
