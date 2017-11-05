using UnityEngine;
using System.Collections;

public enum EOwner { ENEMY, PLAYER, GROUP, NONE };
public enum EType { DEFAULT, PLAYER, OPTION, SHOT, ENEMY, BULLET, BOMB, ITEM, EFFECT, CUSTOM };
public enum EStyle { SHOT, LASER, CURVE, TEXT }

[CreateAssetMenu(fileName = "Bullet", menuName = "Sprites/Bullet")]
public partial class Bullet : ScriptableObject
{
    // Index in the queue
    public int Index { get; set; }

    // Lifetime (optional !)
    public float? Lifetime { get; set; }

    // Clamping (optional !)
    public Bounds? Clamping { get; set; }

    // Physics (optional !)
    public float? Mass { get; set; }
    public Vector3? Force { get; set; }

    // Type and owner
    public EType Type;
    public EOwner Owner;

    // Material ID used for rendering
    public EMaterial Material;

    // Is it an active bullet (for coroutines)
    public bool Active = false;

    // Standard Position, Rotation, Scale
    public Vector3 Position;
    public Vector3 PreviousPosition;
    public Vector3 Scale = new Vector3(1, 1);
    public Quaternion Rotation { get; set; }

    // Width and Height of dipslayed quad
    public float Width {
        get { return Vector3.Distance(OBB.BL, OBB.BR); }
    }

    public float Height {
        get { return Vector3.Distance(OBB.BL, OBB.FL); }
    }

    // Cartesian Information
    private Vector3 _direction;
    public Vector3 Direction
    {
        get { return _direction; }
        set
        {
            _direction = value;
            float rad_angle = Mathf.Atan2(_direction.y, _direction.x);
            _angle = rad_angle * Mathf.Rad2Deg;
        }
    }

    // Polar information
    [SerializeField]
    private float _angle;
    public float Angle
    {
        get { return _angle; }
        set
        {
            _angle = value;
            float rad_angle = _angle * Mathf.Deg2Rad;
            _direction = new Vector3(Mathf.Cos(rad_angle), Mathf.Sin(rad_angle));
        }
    }

    // Sprite angle
    public Vector3 SpriteAngle;

    public float Speed;
	public float MinSpeed;
    public float Acceleration;
    public float AngularVelocity;

    // Vertices and UV information
    public Sprite Sprite; // Usualy Bounds, UVs are set from the sprite ref
    public Bounds Bounds { get; set; }
    public Rect UVs { get; set; }

    // Coordinate rect
    public Rect AABB { get; set; }
    public OBB OBB { get; set; }    

    [SerializeField]
    private Color32 _color = new Color32(255, 255, 255, 255);
    public Color32 Color
    {
        get { return _color; }
        set { _color = value; }
    }

    // Radius of the hitbox
    public float Radius;

    // Delay before appearing
    public float Delay;

    // Player shot data
    public float Penetration;
    public float Damage;

    // Deletion conditions
    public bool SpellResist = false;
    public bool AutoDelete = true;

    // Usually not to be modified by hand
    public float CurrentTime { get; set; }

	public void CopyData(float ? speed, float ? angle, float ? acc, float ? ang_vec) {
		if (speed.HasValue) { Speed = speed.Value; }
		if (angle.HasValue) { Angle = angle.Value; }
		if (acc.HasValue) { Acceleration = acc.Value; }
		if (ang_vec.HasValue) { AngularVelocity = ang_vec.Value; }
	}

    public void CopyData(Sprite sprite, EType type, EMaterial material,
                         Vector3 position, float speed = 0, float angle = 0, float acc = 0, float ang_vec = 0) {
        Position = position;
        PreviousPosition = position;
        Direction = new Vector3(0, 0);
        Sprite = sprite;
        Type = type;
        Material = material;
        Speed = speed;
        Angle = angle;
        Acceleration = acc;
        AngularVelocity = ang_vec;

        if(sprite != null) { 
            UVs = sprite.rect;
            Bounds = sprite.bounds;
        }
    }

    public void CopyData(Bullet prefab) {
        Position = prefab.Position;
        PreviousPosition = prefab.Position;
        Direction = prefab.Direction;
        Lifetime = prefab.Lifetime;
        Mass = prefab.Mass;
        Force = prefab.Force;
        Scale = prefab.Scale;
        Angle = prefab.Angle;
        Speed = prefab.Speed;
        Acceleration = prefab.Acceleration;
        AngularVelocity = prefab.AngularVelocity;
        UVs = prefab.Sprite.rect;
        Bounds = prefab.Sprite.bounds;
        Type = prefab.Type;
        Damage = prefab.Damage;
    }

    public virtual void SetupTriangles(int[] _indices) {
        int xdx = Index * 6;
        int ydx = Index * 4;

        _indices[xdx] = ydx + 0;
        _indices[xdx + 1] = ydx + 2;
        _indices[xdx + 2] = ydx + 1;
        _indices[xdx + 3] = ydx + 3;
        _indices[xdx + 4] = ydx + 2;
        _indices[xdx + 5] = ydx + 0;
    }

    public virtual void SetupUVs(Vector2[] _uvs, Texture tex) {
        int offset = Index * 4;
        Texture texture = tex;

        float width = texture.width;
        float height = texture.height;

        float min_x = UVs.xMin;
        float min_y = UVs.yMin;
        float max_x = UVs.xMax;
        float max_y = UVs.yMax;

        _uvs[offset] = new Vector2(min_x / width, min_y / height);
        _uvs[offset + 1] = new Vector2(max_x / width, min_y / height);
        _uvs[offset + 2] = new Vector2(max_x / width, max_y / height);
        _uvs[offset + 3] = new Vector2(min_x / width, max_y / height);
    }

    // This function updates the internal bullet data
    protected void UpdateState(float dt = 0) {
        CurrentTime += dt;
        PreviousPosition = Position;
		Speed = Speed + Acceleration;
        Position += dt * Speed * Direction;
        Angle += AngularVelocity;
    }

    // This is used for physics
    public virtual void ComputePosition(Vector3[] _vertices, Color32[] _colors, float dt = 0) {
        UpdateState(dt);
        SetupVertices(_vertices, _colors);     
    }

    // This is the real function setting up vertices
    public virtual void SetupVertices(Vector3[] _vertices, Color32[] _colors) {
        int offset = Index * 4;
        Vector3 bullet_ext = Vector3.Scale(Bounds.extents, Scale);

        // Clamp
        if (Clamping != null)
        {
            Vector3 clamping_ext = Clamping.Value.extents;
            Vector3 clamping_pos = Clamping.Value.center;
            Vector3 max_boundary = new Vector3(clamping_pos.x + clamping_ext.x - bullet_ext.x, clamping_pos.y - clamping_ext.y - bullet_ext.y);
            Vector3 min_boundary = new Vector3(clamping_pos.x - clamping_ext.x + bullet_ext.x - 0.5f, clamping_pos.y + clamping_ext.y + bullet_ext.y);
            Position = Maths.Clamping(Position, min_boundary, max_boundary);
        }

        float min_x = Position.x - bullet_ext.x;
        float min_y = Position.y - bullet_ext.y;
        float max_x = Position.x + bullet_ext.x;
        float max_y = Position.y + bullet_ext.y;

        _vertices[offset] = Maths.Rotate(new Vector3(min_x, min_y, Position.z), Position, SpriteAngle);          // Low left
        _vertices[offset + 1] = Maths.Rotate(new Vector3(max_x, min_y, Position.z), Position, SpriteAngle);      // Up left
        _vertices[offset + 2] = Maths.Rotate(new Vector3(max_x, max_y, Position.z), Position, SpriteAngle);      // Up right
        _vertices[offset + 3] = Maths.Rotate(new Vector3(min_x, max_y, Position.z), Position, SpriteAngle);      // Low right

        // Update colliders box
        float rect_min_x = Mathf.Min(_vertices[offset].x, _vertices[offset + 1].x, _vertices[offset + 2].x, _vertices[offset + 3].x);
        float rect_min_y = Mathf.Min(_vertices[offset].y, _vertices[offset + 1].y, _vertices[offset + 2].y, _vertices[offset + 3].y);
        float rect_max_x = Mathf.Max(_vertices[offset].x, _vertices[offset + 1].x, _vertices[offset + 2].x, _vertices[offset + 3].x);
        float rect_max_y = Mathf.Max(_vertices[offset].y, _vertices[offset + 1].y, _vertices[offset + 2].y, _vertices[offset + 3].y);
        AABB = Rect.MinMaxRect(rect_min_x, rect_min_y, rect_max_x, rect_max_y);
        OBB = new OBB(_vertices[offset + 3], _vertices[offset + 2], _vertices[offset], _vertices[offset + 1]);

        _colors[offset] = _colors[offset + 1] = _colors[offset + 2] = _colors[offset + 3] = Color;
    }
		
    public IEnumerator _Bind(Transform transform) {
        while (Active) {
            Position = transform.position;
            Rotation = transform.rotation;
            Scale = transform.lossyScale;
            yield return null;
        }
    }

    public IEnumerator _Follow(BezierCurve curve) {
        float elapsedTime = 0;
        while (elapsedTime < 1) {
            Position = curve.GetPoint(elapsedTime);
            Direction = curve.GetDirection(elapsedTime);
            elapsedTime += GameScheduler.dt * curve.GetSpeed(elapsedTime);
            yield return new WaitForSeconds(GameScheduler.dt);
        }
    }

	public IEnumerator _Change(float timeToWait, float ? speed, float ? angle, float ? acc, float ? ang_vec) {
		yield return new WaitForSeconds(timeToWait);
		CopyData(speed, angle, acc, ang_vec);
	}
}