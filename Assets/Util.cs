using System;
using UnityEngine;


public class Util{
	
	public static System.Random rand = new System.Random();
	
	public static float rand_range(float min, float max) {
		float r = (float)rand.NextDouble();
		return (max-min)*r + min;
	}
	
	public static float vec_dist(Vector3 a, Vector3 b) {
		return (float)Math.Abs(Math.Sqrt(Math.Pow(a.x-b.x,2)+Math.Pow(a.y-b.y,2)+Math.Pow(a.z-b.z,2)));
	}
	
	public static Vector3 vec_sub(Vector3 a, Vector3 b) {
		return new Vector3(a.x-b.x,a.y-b.y,a.z-b.z);
	}
	
	public static Vector3 vec_cross(Vector3 v1,Vector3 a) {
		float x1, y1, z1;
		x1 = (v1.y*a.z) - (a.y*v1.z);
		y1 = -((v1.x*a.z) - (v1.z*a.x));
		z1 = (v1.x*a.y) - (a.x*v1.y);
		return new Vector3(x1,y1,z1);
	}

	static Vector3 _valv = new Vector3();
	public static Vector3 valv(float x) {
		_valv.x = x;
		_valv.y = x;
		_valv.z = x;
		return _valv;
	}
	
	public static GameObject FindInHierarchy(GameObject root, string name)
	{
		if (root == null || root.name == name)
		{
			return root;
		}
		
		Transform child = root.transform.Find(name);
		if (child != null)
		{
			return child.gameObject;
		}
		
		int numChildren = root.transform.childCount;
		for (int i = 0; i < numChildren; i++)
		{
			GameObject go = FindInHierarchy(root.transform.GetChild(i).gameObject, name);
			if (go != null)
			{
				return go;
			}
		}
		
		return null;
	}
	
	public static Vector3 vec_add(Vector3 a, Vector3 b) {
		Vector3 v = new Vector3();
		v.x = a.x + b.x;
		v.y = a.y + b.y;
		v.z = a.z + b.z;
		return v;
	}
	
	public static Vector3 vec_scale(Vector3 v,float f) {
		v.x *= f;
		v.y *= f;
		v.z *= f;
		return v;
	}
	
	public static float clampf(float val, float min, float max) {
		if (val > max) {
			return max;
		} else if (val < min) {
			return min;
		} else {
			return val;
		}
	}
	
	public static float sig(float n) {
		if (n > 0) {
			return 1;
		} else if (n < 0) {
			return -1;
		} else {
			return 0;
		}
	}
	
	public static float rad2deg = 57.29f;
	public static float deg2rad = 0.017f;
	
	
	static public bool collider_contains_pt ( Collider test, Vector3 point) {
		Vector3    center;
		Vector3    direction;
		Ray        ray;
		RaycastHit hitInfo;
		bool       hit;
		center = test.bounds.center;
		direction = center - point;
		ray = new Ray(point, direction);
		hit = test.Raycast(ray, out hitInfo, direction.magnitude);
		return !hit;
	}
}


