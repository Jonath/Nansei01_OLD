using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Class to be used for any GameObject holding a MeshPool and a sprite
[ExecuteInEditMode]
public class Entity : MonoBehaviour {
    public MeshPool     pool;          // Mesh pool representing the entity data
    public Sprite       sprite;        // Sprite to animate
	public EType		type;		   // Type of the object 
	public EMaterial    material;      // Material to use
    public List<Bullet> bullets;       // List of objects fired by the entity

    [System.NonSerialized]
    public Bullet obj;                 // Bullet object to manipulate

    [ExecuteInEditMode]
    public virtual void Init()
    {
        if(gameObject.activeInHierarchy) {
            bullets = new List<Bullet>();
			obj = pool.AddBullet(sprite, type, material, GameScheduler.ComputeScale(transform.position));
        }
    }

    public IEnumerator _Wait(float n) {
        yield return new WaitForSeconds(n);
    }
}
