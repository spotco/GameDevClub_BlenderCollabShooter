using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[System.Serializable]
public class S3KControl : MonoBehaviour {
	
	public static S3KControl inst;
	
	public static float JUMP_FORCE = 220f;
	public static float MOVE_SPEED = 7f;
	
	public Rigidbody _body;
	public Vector3 _last_body_position;
	public bool _has_last_position = false; 
	public GameObject _normal_camera;

	public GameObject _flame;
	public bool _is_game_over = false;
	public int _score = 0;

	void Start () {
		inst = this;
		_body = gameObject.GetComponent<Rigidbody>();
		_body.freezeRotation = true;
		_bullet_cooldown = 0;
		_normal_camera = Util.FindInHierarchy(gameObject,"Main Camera");
		_flame = Util.FindInHierarchy(gameObject,"Flame");
		_flame.SetActive(false);
	}
	
	bool _mouse_centered = true;
	Rect _r = new Rect(30,30,200,200);


	void OnGUI() {
		if (_is_game_over) {
			GUI.Label(_r,"GAME OVER\n Score: "+_score+"\nPRESS Q TO CONTINUE");
		} else {
			GUI.Label(_r,"Score: "+_score+"\n Enemies: "+EnemyManager.inst._enemies.Count);
		}

	}

	void Update () {
		if (_is_game_over) {
			if (Input.GetKeyDown(KeyCode.Q)) {
				_score = 0;
				_is_game_over = false;
				for (int i = 0; i < EnemyManager.inst._enemies.Count; i++) EnemyManager.inst._enemies[i].kill();
				EnemyManager.inst._spawn_timer = 150;

				gameObject.transform.position = new Vector3(0.3f,2.63f,-6.6f);

			}
			_flame.SetActive(true);
			return;
		} else {
			_flame.SetActive(false);
		}

		if (gameObject.transform.position.y < -20) _is_game_over = true;

		if (Input.GetKeyDown(KeyCode.Escape)) _mouse_centered = !_mouse_centered;
		if (_mouse_centered) {
			Screen.showCursor = false;
			Screen.lockCursor = true;
		} else {
			Screen.showCursor = true;
			Screen.lockCursor = false;
		}
		
		if (Input.GetKey(KeyCode.R)) {
			gameObject.transform.position = Vector3.zero;
			_body.velocity = Vector3.zero;
		}
		
		click_shoot();
		jump();
		move();
		if (_mouse_centered) {
			fps_turn();
		}
		
		_last_body_position = gameObject.transform.position;
		_has_last_position = true;
			

	}
	
	int _bullet_cooldown = 0;
	
	void click_shoot() {
		_bullet_cooldown--;
		if ( Input.GetMouseButton(0)  && _bullet_cooldown <= 0 ) {

			Vector3 bullet_vel = _normal_camera.transform.forward;
			bullet_vel.Normalize();
			bullet_vel = Util.vec_scale(bullet_vel,0.5f);
			BulletManager.inst.add_bullet(
				_normal_camera.transform.position,
				bullet_vel
			);
			
			_bullet_cooldown = 5;
		}
	}
	
	public int _jump_cooldown;
	public int _move_cooldown;
	int _jump_count = 0;
	
	void jump() {
		if (_jump_count > 0 && _jump_cooldown == 0 && Input.GetKey(KeyCode.Space)) {
			_jump_cooldown = 20;
			_move_cooldown = 20;
			Vector3 jump_dir = new Vector3(0,1f,0);
			jump_dir.Normalize();
			jump_dir.Scale(new Vector3(JUMP_FORCE,JUMP_FORCE,JUMP_FORCE));
			_body.AddForce(jump_dir);
			_jump_count--;
		}
		
		if (_jump_cooldown > 0) _jump_cooldown--;
		if (_move_cooldown > 0) _move_cooldown--;
		
		if (on_ground()) {
			_jump_count = 1;
		}
	}
	
	void move() {
		Vector3 neu_vel = _body.velocity;
		if (_move_cooldown <= 0 && on_ground()) {
			Vector3 forward = gameObject.transform.forward;
			forward.y = 0;
			forward.Normalize();
			
			bool move_ws = false;
			Vector3 ws_v = forward;
			ws_v = Util.vec_scale(ws_v,MOVE_SPEED);
			if (Input.GetKey(KeyCode.W)) {
				move_ws = true;
			} else if (Input.GetKey(KeyCode.S)) {
				move_ws = true;
				ws_v = Util.vec_scale(ws_v,-1);
			}
			
			bool move_ad = false;
			Vector3 ad_v = Util.vec_cross(forward,new Vector3(0,1,0));
			ad_v.Normalize();
			ad_v = Util.vec_scale(ad_v,MOVE_SPEED);
			
			if (Input.GetKey(KeyCode.A)) {
				move_ad = true;
			} else if (Input.GetKey(KeyCode.D)) {
				move_ad = true;
				ad_v = Util.vec_scale(ad_v,-1);
			}
			
			if (move_ws && move_ad) {
				neu_vel = Util.vec_add(ws_v,ad_v);
			} else if (move_ws) {
				neu_vel = ws_v;
			} else if (move_ad) {
				neu_vel = ad_v;
			}
		}
		
		if (on_ground()) {
			neu_vel = Util.vec_scale(neu_vel,0.9f);
		}
		
		if (Math.Abs(neu_vel.x) < 0.2f) neu_vel.x = 0;
		if (Math.Abs(neu_vel.z) < 0.2f) neu_vel.z = 0;
		_body.velocity = neu_vel;
	}
	
	Vector3 _xy_angle = Vector3.zero;
	static float MAX_X_ANGLE = 45;
	static float FPS_LOOK_SCALE = 2.5f;
	
	void fps_turn() {
		if (!Input.mousePresent) {
			Screen.showCursor = true;
			Screen.lockCursor = false;
			return;
		}
		
		_xy_angle.y += Input.GetAxis("Mouse X") * FPS_LOOK_SCALE;
		_xy_angle.x -= Input.GetAxis("Mouse Y") * FPS_LOOK_SCALE;
		
		if (Math.Abs(_xy_angle.x) > MAX_X_ANGLE) {
			_xy_angle.x = Util.sig(_xy_angle.x) * MAX_X_ANGLE;
		}
		
		gameObject.transform.rotation = Quaternion.Euler(_xy_angle);
	}
	
	Dictionary<int,Vector3> _instanceid_to_collision_normal = new Dictionary<int, Vector3>();
	
	public bool on_ground() {
		Vector3 up = new Vector3(0,1,0);
		foreach(int instanceid in _instanceid_to_collision_normal.Keys) {
			Vector3 collision_n = _instanceid_to_collision_normal[instanceid];
			float dot = up.x * collision_n.x + up.y * collision_n.y + up.z * collision_n.z;
			dot /= (up.magnitude * collision_n.magnitude);
			float angle_r = Mathf.Acos(dot);
			if (angle_r < 0.7) {
				return true;
			}
		}
		return false;
	}
	
	void OnCollisionEnter(Collision col) {
		ContactPoint contact = col.contacts[0];
		_instanceid_to_collision_normal[col.collider.GetInstanceID()] = contact.normal;
	}
	
	void OnCollisionExit(Collision col) {
		if (_instanceid_to_collision_normal.ContainsKey(col.collider.GetInstanceID()))
			_instanceid_to_collision_normal.Remove(col.collider.GetInstanceID());
	}
}
