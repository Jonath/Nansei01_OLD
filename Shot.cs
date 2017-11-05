using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// A type of player shot (maybe make a static library instead)
public class Shot : MonoBehaviour {
    public Sprite sprite;

    private MeshPool pool;
    private IEnumerator pattern;

    public void SetMeshPool(MeshPool pool) {
        this.pool = pool;
    }

    public void SetPattern(IEnumerator pattern) {
        this.pattern = pattern;
    }

    public IEnumerator _MikoShot(Player.OptionData option_data, Vector3 player_pos) {
        while (true)
        {
            if (Input.GetKey("w") || Input.GetKey("z"))
            {
                // Fire main shot


                // Fire each option shot
                foreach (Bullet option in option_data.options)
                {
                    Vector3 ShotPos = new Vector3(option.Position.x,
                                                  option.Position.y);

                    Color ShotColor = new Color32(255, 255, 255, 50);

                    Bullet b1 = pool.AddBullet(sprite, EType.SHOT, EMaterial.MIKO, ShotPos + new Vector3(0, 10), 800, 90);
                    Bullet b2 = pool.AddBullet(sprite, EType.SHOT, EMaterial.MIKO, ShotPos, 800, 75);
                    Bullet b3 = pool.AddBullet(sprite, EType.SHOT, EMaterial.MIKO, ShotPos - new Vector3(0, 10), 800, 105);
                    b1.SpriteAngle.z = b1.Angle;
                    b2.SpriteAngle.z = b2.Angle;
                    b3.SpriteAngle.z = b3.Angle;
                    b1.Radius = b2.Radius = b3.Radius = 5;
                    b1.Color = b2.Color = b3.Color = ShotColor;
                    b1.Damage = b2.Damage = b3.Damage = 5;
                } // end foreach
            } // end if

            yield return new WaitForSeconds(0.1f);
        } // end while
    }
}
