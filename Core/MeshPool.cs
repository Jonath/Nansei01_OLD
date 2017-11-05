using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

// Add as many items as materials / fonts (in rendering order)
public enum EMaterial { MIKO, BULLET, BULLETADD, BORDER, GUI, DIALOGUE, TEXT, COUNT, NEUTRAL }

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter)), ExecuteInEditMode]
public partial class MeshPool : MonoBehaviour {
    public int BulletCount;
    public MeshRenderer[] MeshRenderers;
    public MeshFilter[] MeshFilters;

    // Collision stuff
    public QuadTreeHolder QuadTreeHolder;

    public string SortingLayerName;
    public int OrderInLayer;

    public List<Bullet> _active = new List<Bullet>();
    private List<Bullet> _temp = new List<Bullet>();

    // Number of bullets to pre-allocate
    public int MaxBullets = 5000;

    // Mesh and bullets pre-allocated
    private Mesh[] _meshs;
    private Bullet[] _bullets;

    // Vertices data
    private int[][] _indices;
    private Vector3[][] _vertices;
    private Vector3[][] _normals;
    private Vector2[][] _uvs;
    private Color32[][] _colors;

    private Material[] _materials;
    private Font[] _fonts;

    // Queue to reference available bullets
    private Queue<int> _available;

    [ExecuteInEditMode]
    public void Init() {
        _active.Clear();

        _bullets = new Bullet[MaxBullets];
        for(int i = 0; i < MaxBullets; i++) {
            _bullets[i] = ScriptableObject.CreateInstance("Bullet") as Bullet;
        }

        // Dirty init stuff here
        _available = new Queue<int>(Enumerable.Range(0, MaxBullets));

        int NbMaterials = (int)EMaterial.COUNT;

        // Vertices per material / mesh
        _vertices = new Vector3[NbMaterials][];

        // Indices per material / mesh
        _indices = new int[NbMaterials][];

        // Normals per material / mesh
        _normals = new Vector3[NbMaterials][];

        // UVs per material / mesh
        _uvs = new Vector2[NbMaterials][];

        // Colors per material / mesh
        _colors = new Color32[NbMaterials][];

        _fonts = new Font[] {
            Resources.Load("arial") as Font,
        };

        // Material array aligned with EMaterial enum
        _materials = new Material[] {
            // Main Materials
            Resources.Load("MikoMaterial") as Material,
            Resources.Load("BulletMaterial") as Material,
            Resources.Load("BulletAddMaterial") as Material,
            Resources.Load("BorderMaterial") as Material,
            Resources.Load("GUIMaterial") as Material,
            Resources.Load("DialogueMaterial") as Material,
            // Fonts
            _fonts[0].material
        };
			
        //_mesh = new Mesh {subMeshCount = (int)EMaterial.COUNT, vertices = _vertices, normals = _normals, uv = _uvs, colors32 = _colors};
        _meshs = new Mesh[NbMaterials];

        // Allocating for each material @TODO maybe set MaxBullets to less for some meshs. Or use sprites for them !
        for (int i = 0; i < NbMaterials; ++i) {
            _vertices[i] = new Vector3[MaxBullets * 4];
            _indices[i] = new int[MaxBullets * 6];
            _normals[i] = Enumerable.Repeat(Vector3.back, MaxBullets * 4).ToArray();
            _uvs[i] = new Vector2[MaxBullets * 4];
            _colors[i] = Enumerable.Repeat((Color32)Color.white, MaxBullets * 4).ToArray();
            _meshs[i] = new Mesh { vertices = _vertices[i], normals = _normals[i], uv = _uvs[i], colors32 = _colors[i] };
        }

    }

    public List<Bullet> GetBullets() {
        return _active;
    }

    public void PullAdd(Bullet bullet) {
        int index = _available.Dequeue();
        bullet.Index = index;
        bullet.CurrentTime = 0;
        _bullets[index] = bullet;
    }

    public Bullet PullBullet(EType type, EMaterial material) {
        if (_available.Count == 0) {
            Debug.LogWarning("No available quads, failed to add bullet");
            return null;
        }

        int index = _available.Dequeue();
        Bullet bullet = _bullets[index];

        // Get an available Index and set Time to 0
        bullet.Index = index;
        bullet.CurrentTime = 0;

        bullet.Type = type;
        bullet.Material = material;

        return bullet;
    }

    public void SetupBullet(Bullet bullet) {
        int MaterialIdx = (int)bullet.Material;
        // Update bullet appearance (UVs)
        bullet.SetupUVs(_uvs[MaterialIdx], _materials[MaterialIdx].mainTexture);

        // Setup triangles according to render mode
        bullet.SetupTriangles(_indices[MaterialIdx]);

        // Update bullet once (vertices)
        bullet.SetupVertices(_vertices[MaterialIdx], _colors[MaterialIdx]);
    }

    public Bullet AddBullet(Sprite sprite, EType type, EMaterial material, Vector3 position, float speed = 0, float angle = 0, float acc = 0, float ang_vec = 0) {
        Bullet bullet = AddBullet(sprite, type, material);
        bullet.CopyData(sprite, type, material, position, speed, angle, acc, ang_vec);
        return bullet;
    }

    public Bullet AddBullet(Sprite sprite, EType type, EMaterial material) {
        Bullet bullet = PullBullet(type, material);

        // Update some of the bullet data
        if(sprite != null) { 
            bullet.UVs = sprite.rect;
            bullet.Bounds = sprite.bounds;
        }

        SetupBullet(bullet);

		// Add the bullet to active list
        bullet.Active = true;
        _active.Add(bullet);

        return bullet;
    }

    public void CleanBullet(Bullet bullet) {
        // We only clean indice data because laziness / optimisation
        int xdx = bullet.Index * 6;
        int matidx = (int)bullet.Material;
        _indices[matidx][xdx] = 0;
        _indices[matidx][xdx + 1] = 0;
        _indices[matidx][xdx + 2] = 0;
        _indices[matidx][xdx + 3] = 0;
        _indices[matidx][xdx + 4] = 0;
        _indices[matidx][xdx + 5] = 0;
    }

    public void RemoveBullet(Bullet bullet) {
        CleanBullet(bullet);

        bullet.Active = false;
        _available.Enqueue(bullet.Index);
        _temp.Remove(bullet);
    }

    public Bullet ChangeBulletAppearance(Bullet bullet, Sprite sprite, EMaterial material) {
        CleanBullet(bullet);
        bullet.Bounds = sprite.bounds;
        bullet.UVs = sprite.rect;
        bullet.Material = material;
        SetupBullet(bullet);
        return bullet;
    }

    public void UpdateBulletAppearance(Bullet bullet) {
        int MaterialIdx = (int)bullet.Material;
        bullet.SetupUVs(_uvs[MaterialIdx], _materials[MaterialIdx].mainTexture);
    }

    public void UpdateAt(float dt) {
        _temp = new List<Bullet>();

        foreach (Bullet bullet in _active) {
			// Check if the bullet has expired
			bool removed = HandleBulletLifeTime(bullet);

			if (removed == false) {
				// Check events registered in a bullet (change of position, angle, speed, acceleration, etc.)


				// Update bullets position if they are not part of a group
				if (bullet.Owner != EOwner.GROUP) {
					int MaterialIdx = (int)bullet.Material;
					bullet.ComputePosition(_vertices [MaterialIdx], _colors [MaterialIdx], dt);
				}

				_temp.Add (bullet);
			}
        }

        _active = _temp;
        BulletCount = _active.Count;

        // Maybe only call that once before rendering
        SetMesh();
    }
		
    public void ReferenceBullets() {
		QuadTreeHolder.ReferenceBullets(_active);
    }

    void SetMesh() {
        for (int i = 0; i < (int)EMaterial.COUNT; ++i) {
            _meshs[i].vertices = _vertices[i];
            if (_indices != null) {
                _meshs[i].SetTriangles(_indices[i], 0);
            } 

            _meshs[i].uv = _uvs[i];
            _meshs[i].colors32 = _colors[i];
            _meshs[i].RecalculateBounds();
            MeshFilters[i].mesh = _meshs[i];
            MeshRenderers[i].material = _materials[i];
        }
    }

	private bool HandleBulletLifeTime(Bullet bullet) {
		bool removed = false;
		if ((bullet.Lifetime.HasValue && bullet.CurrentTime >= bullet.Lifetime.Value) ||
			((bullet.Type == EType.BULLET || bullet.Type == EType.SHOT) && !bullet.AABB.Overlaps(QuadTreeHolder.quadtree.rect))) {

			RemoveBullet(bullet);
			removed = true;
		}
		return removed;
	}
}

