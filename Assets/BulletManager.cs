using UnityEngine;
using System.Collections.Generic;

public class BulletManager : MonoBehaviour {
	
	public static BulletManager inst;
	
	public List<Bullet> _bullets = new List<Bullet>();
	
	void Start () {
		inst = this;
	}
	
	void Update () {
		for (int i = _bullets.Count-1; i >= 0; i--) {
			Bullet b = _bullets[i];
			if (b.should_remove()) {
				_bullets.RemoveAt(i);
				b.do_remove();
			}
		}
	}
	
	
	int _allocid = 0;
	public void add_bullet(Vector3 position, Vector3 vel) {
		GameObject bullet_object = (GameObject)Instantiate(Resources.Load("Bullet"));
		bullet_object.transform.parent = gameObject.transform;
		bullet_object.AddComponent<Bullet>();
		Bullet neu_bullet = bullet_object.GetComponent<Bullet>();
		neu_bullet.start(position,vel,bullet_object,_allocid,0);
		_bullets.Add(neu_bullet);
		_allocid++;
	}
}