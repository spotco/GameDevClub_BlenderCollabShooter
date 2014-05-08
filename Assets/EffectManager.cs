using UnityEngine;
using System.Collections.Generic;

public class EffectManager : MonoBehaviour {
	
	public static EffectManager inst;
	
	public List<Effect> effects = new List<Effect>();
	
	void Start () {
		inst = this;
	}
	
	void Update () {
		for (int i = effects.Count-1; i >= 0; i--) {
			Effect b = effects[i];
			b.update();
			if (b.should_remove()) {
				effects.RemoveAt(i);
				b.do_remove();
			}
		}
	}
	
	public void add_effect(Effect e) {
		effects.Add(e);
	}
}

public class Effect {
	
	GameObject obj;
	int ct;
	
	public Effect(string resc, Vector3 pos, int _ct) {
		obj = (GameObject)EffectManager.Instantiate(Resources.Load(resc));
		obj.transform.position = pos;
		ct = _ct;
		obj.transform.parent = EffectManager.inst.gameObject.transform;
	}
	
	public Effect set_rotation(Vector3 v) {
		obj.transform.eulerAngles = v;
		return this;
	}

	public Effect set_scale(float sc) {
		obj.transform.localScale = Util.valv(sc);
		return this;
	}
	
	public void update() {
		ct--;
	}
	
	public bool should_remove() {
		return ct <= 0;
	}
	
	public void do_remove() {
		GameObject.Destroy(obj);
		obj = null;
	}
	
}
