using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour {
	public static EnemyManager inst;
	
	public List<Enemy> _enemies = new List<Enemy>();

	void Start () {
		inst = this;
	}

	public int _spawn_timer = 150;
	int _spawn_ct = 0;
	
	void Update () {
		for (int i = _enemies.Count-1; i >= 0; i--) {
			Enemy b = _enemies[i];
			if (b.should_remove()) {
				_enemies.RemoveAt(i);
				b.do_remove();
			}
		}

		_spawn_ct--;

		if (_spawn_ct <= 0) {
			_spawn_ct = _spawn_timer;
			_spawn_timer-=5;
			Vector3 pos = S3KControl.inst.gameObject.transform.position;
			float theta = Util.rand_range(-3.14f,3.14f);

			pos.x += 40.0f * Mathf.Cos (theta);
			pos.y += Util.rand_range(10,30);
			pos.z += 40.0f * Mathf.Sin (theta);
			add_enemy(pos);
		}
	}

	List<string> _models = new List<string>() {
		"models/thing1",
		"models/thing2"
	};

	public void add_enemy(Vector3 position) {
		GameObject enemy_object = (GameObject)Instantiate(Resources.Load("Enemy"));
		enemy_object.transform.parent = gameObject.transform;
		enemy_object.AddComponent<Enemy>();


		string tar_model = _models[(int)Util.rand_range(0,_models.Count)];
		GameObject enemy_model = (GameObject)Instantiate(Resources.Load(tar_model));
		enemy_model.transform.parent = enemy_object.transform;
		enemy_model.transform.localPosition = Vector3.zero;

		Enemy neu_enemy = enemy_object.GetComponent<Enemy>();
		neu_enemy.start(position,enemy_object);
		_enemies.Add(neu_enemy);
	}
}
