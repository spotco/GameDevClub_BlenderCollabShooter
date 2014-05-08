using UnityEngine;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {

	public Vector3 _position;
	public Vector3 _vel = new Vector3(0,0,0);
	public GameObject _obj;
	float _speed = 0.1f;
	public bool _kill = false;
	Vector3 _rv;
	
	public void start(Vector3 position, GameObject obj) {
		_position = position;
		_obj = obj; 
		_obj.transform.position = position;
		_obj.transform.forward = new Vector3(0,1,0);

		_speed = Util.rand_range(0.1f,0.4f);

		_rv = new Vector3(Util.rand_range(-10,10),Util.rand_range(-10,10),Util.rand_range(-10,10));
	}
	
	void Update() {

		gameObject.transform.Rotate(_rv);

		_vel.x = S3KControl.inst.gameObject.transform.position.x - gameObject.transform.position.x;
		_vel.y = S3KControl.inst.gameObject.transform.position.y - gameObject.transform.position.y;
		_vel.z = S3KControl.inst.gameObject.transform.position.z - gameObject.transform.position.z;
		_vel.Normalize();
		_vel.Scale(Util.valv(_speed));

		_position.x += _vel.x;
		_position.y += _vel.y;
		_position.z += _vel.z;
		_obj.transform.position = _position;
	}
	
	public bool should_remove() {
		return _kill;	
	}
	
	public void do_remove() {
		GameObject.Destroy(_obj);
		_obj = null;
	}

	public void kill() {
		_kill = true;
	}
	
	void OnTriggerEnter(Collider col) {
		if (col.gameObject.GetComponent<S3KControl>() != null) {
			this.kill ();
			S3KControl.inst._is_game_over = true;

		}
	}
	
}