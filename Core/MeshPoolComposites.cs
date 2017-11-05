using UnityEngine;
using System.Collections;

public partial class MeshPool : MonoBehaviour {
    public void AddComposites(Composite composite, int size, EType type, EMaterial material, Sprite sprite = null) {
        if (_available.Count < 0) {
            Debug.LogWarning("No available quads, failed to add bullet");
            return;
        }

        composite._composites = new Bullet[size]; // New here :(

        int index = _active.Count;
        for (int i = 0; i < size; ++i) {
            Bullet bullet = AddBullet(sprite, type, material);
            bullet.Owner = EOwner.GROUP;
            composite._composites[i] = bullet;
        }

        composite._nb_composites = size;
    }

    public void RemoveComposites(Composite composite) {
        for(int i = 0; i < composite._nb_composites; ++i) {
            RemoveBullet(composite._composites[i]);
        }

        composite._nb_composites = 0;
        composite._composites = null;
    }

    public Laser AddLaser(Sprite sprite, EType type, EMaterial material, BezierCurve curve, int num_segments, float length, float width,
                          Vector3 position, float speed = 0, float angle = 0, float acc = 0, float ang_vec = 0) {
        // To cache in pool (surely)
        Laser laser = new Laser(curve, length, width);
        AddComposites(laser, num_segments, type, material, sprite);
        PullAdd(laser);

        laser.CopyData(sprite, type, material, position, speed, angle, acc, ang_vec);
        for (int i = 0; i < num_segments; ++i) {
            laser._composites[i].CopyData(sprite, type, material, Vector3.zero, speed, angle);
        }

        // Update laser once (vertices)
        laser.SetupVertices(_vertices[0], _colors[0]); // To update

        // Add the bullet to active list
        laser.Active = true;
        _active.Add(laser);

        return laser;
    }

    public Text AddText(string str, Dialogue pannel, EType type, EMaterial material, ETextStyle style,
                        Vector3 position, float speed = 0, float angle = 0, float acc = 0, float ang_vec = 0) {
        // To cache in pool (or reuse instances)
        Text text = new Text(_fonts[0], style, 40, str);

        // Number of composites needed
        int strlen = (str == null ? 0 : str.Length);
        if (style == ETextStyle.SHADOW) strlen *= 2;

        AddComposites(text, strlen, type, material);
        text.Material = material;
        text.pannel = pannel;

        text.CopyData(null, type, material, position, speed, angle, acc, ang_vec);

        PullAdd(text);
        SetupBullet(text);

        // Add the bullet to active list
        text.Active = true;
        _active.Add(text);

        return text;
    }

    public Text ChangeText(Text text, string str) {
        RemoveComposites(text);

        text._str = str;

        // Number of composites needed
        int strlen = (str == null ? 0 : str.Length);
        if (text._textstyle == ETextStyle.SHADOW) strlen *= 2;

        AddComposites(text, strlen, text.Type, text.Material);
        SetupBullet(text);

        return text;
    }
}
