using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fairy01 : Enemy {
    public float time;
    public BezierCurve curve;
    public Sprite bullet;
    public float offset = 0;

    // Update is called once per frame
    public override void Init() {
        base.Init();

        if(obj != null) {
            StartCoroutine(Pattern());
        }
	}

    public void Circle(float n, float speed, float offset) {
        for(float i = 0; i < 360; i += 360 / n) {
            float ang = i + offset;
			Bullet shot = pool.AddBullet(bullet, EType.BULLET, EMaterial.BULLET,
                                         obj.Position, speed, ang, 0, 0);

            shot.Radius = 5;
            shot.SpriteAngle = new Vector3(0, 0, ang - 90);
            shot.AutoDelete = true;

            bullets.Add(shot);
        }
    }

    public IEnumerator Pattern() {
        yield return StartCoroutine(_Wait(2));
        StartCoroutine(obj._Follow(curve));
        yield return StartCoroutine(_Wait(1));

        for(int i = 0; i < 5; i++) {
            Circle(15, 75, offset);
            yield return new WaitForSeconds(0.25f);
        }

		yield return new WaitForSeconds (0.25f);
		Die ();
    }
}
