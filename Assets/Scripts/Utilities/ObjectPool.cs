﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

public class ObjectPool {

	static readonly Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool> ();
	Stack<MonoBehaviour> inactive = new Stack<MonoBehaviour> ();
	List<MonoBehaviour> active = new List<MonoBehaviour> ();

	MonoBehaviour prefab;

	public void Init<T> (string id) where T : MonoBehaviour {
		prefab = PoolIOHandler.LoadPrefab<T> (id);
	}

	static ObjectPool GetPool (string id) {
		return GetPool<MonoBehaviour> (id);
	}

	static ObjectPool GetPool<T> () where T : MonoBehaviour {
		return GetPool<T> (typeof (T).Name);
	}

	static ObjectPool GetPool<T> (string id) where T : MonoBehaviour {
		ObjectPool op;
		if (pools.TryGetValue (id, out op)) {
			return op;
		} else {
			return CreatePool<T> (id);
		}
	}

	static ObjectPool CreatePool<T> (string id) where T : MonoBehaviour {
		ObjectPool newPool = new ObjectPool ();
		newPool.Init<T> (id);
		pools.Add (id, newPool);
		return newPool;
	}

	MonoBehaviour CreateInstance () {

		MonoBehaviour m;

		if (inactive.Count > 0) {
			m = inactive.Pop ();
		} else {
			try {
				m = MonoBehaviour.Instantiate (prefab) as MonoBehaviour;
			} catch {
				throw new System.Exception ("Could not find a prefab of type " + prefab);
			}
		}

		active.Add (m);
		m.gameObject.SetActive (true);

		return m;
	}

	void ReleaseInstance (MonoBehaviour instance) {
		instance.gameObject.SetActive (false);
		active.Remove (instance);
		inactive.Push (instance);
	}

	public static MonoBehaviour Instantiate (string id, Vector3 position=new Vector3 (), Quaternion rotation=new Quaternion ()) {
		Debug.LogWarning ("Unstable - use generic methods instead");
		MonoBehaviour m = GetPool (id).CreateInstance ();
		m.transform.position = position;
		m.transform.localRotation = rotation;
		return m;
	}

	public static T Instantiate<T> (Vector3 position=new Vector3 (), Quaternion rotation=new Quaternion ()) where T : MonoBehaviour {
		T t = GetPool<T> ().CreateInstance ().GetComponent<T> () as T;
		t.transform.position = position;
		t.transform.localRotation = rotation;
		return t;
	}

	// TODO: these appear to break if you've been using generic methods for e.g. instantiation (is it because (Clone) is being added to the name?)
	public static void Destroy (string id) {
		Debug.LogWarning ("Unstable - use generic methods instead");
		ObjectPool op = GetPool (id);
		if (op.active.Count > 0)
			op.ReleaseInstance (op.active[0]);
	}

	// TODO: these appear to break if you've been using generic methods for e.g. instantiation (is it because (Clone) is being added to the name?)
	public static void Destroy (GameObject go) {
		Debug.LogWarning ("Unstable - use generic methods instead");
		MonoBehaviour m = go.GetComponent<MonoBehaviour> ();
		ObjectPool op = GetPool (m.GetType ().Name);
		op.ReleaseInstance (m);
	}

	// TODO: these appear to break if you've been using generic methods for e.g. instantiation (is it because (Clone) is being added to the name?)
	public static void Destroy (Transform t) {
		Debug.LogWarning ("Unstable - use generic methods instead");
		MonoBehaviour m = t.GetComponent<MonoBehaviour> ();
		ObjectPool op = GetPool (m.GetType ().Name);
		op.ReleaseInstance (m);
	}

	public static void Destroy<T> (T t) where T : MonoBehaviour {
		GetPool<T> ().ReleaseInstance (t);
	}

	public static void Destroy<T> (Transform t) where T : MonoBehaviour {
		GetPool<T> ().ReleaseInstance (t.GetComponent<MonoBehaviour> ());
	}

	public static void Destroy<T> (GameObject go) where T : MonoBehaviour {
		GetPool<T> ().ReleaseInstance (go.GetComponent<MonoBehaviour> ());
	}

	// TODO: these appear to break if you've been using generic methods for e.g. instantiation (is it because (Clone) is being added to the name?)
	public static void DestroyChildren (Transform t) {

		List<Transform> children = new List<Transform> ();
		foreach (Transform child in t) children.Add (child);

		for (int i = 0; i < children.Count; i ++)
			Destroy (children[i]);
	}

	public static void DestroyChildren<T> (Transform t, System.Action<T> onDestroy=null) where T : MonoBehaviour {

		List<Transform> children = new List<Transform> ();
		foreach (Transform child in t) children.Add (child);

		for (int i = 0; i < children.Count; i ++) {
			if (onDestroy != null)
				onDestroy (children[i].GetComponent<T> ());
			Destroy<T> (children[i]);
		}
	}

	public static List<T> GetActiveInstances<T> () where T : MonoBehaviour {
		return GetPool<T> ().active.ConvertAll (x => (T)x);
	}
}

public static class PoolIOHandler {

	static string ApplicationPath {
		get { return Application.dataPath; }
	}

	static string ResourcesPath {
		get { return ApplicationPath + "/Resources/Prefabs/"; }
	}

	static string Path {
		get { return "Prefabs/"; }
	}

	/**
	  * Loads a prefab with the given id
	  * First tries to load the prefab from Resources
	  * Failing that, searches for the prefab in the project and copies it into the Resources folder
	  * Failing that, tries to create a new prefab with the given type T
	 */
	public static MonoBehaviour LoadPrefab<T> (string id) where T : MonoBehaviour {
		
		#if UNITY_EDITOR

		MonoBehaviour prefab = LoadMonoBehaviour (id);
		if (prefab == null) {
			string projectPath = FindPrefabDirectory (id, ApplicationPath);
			if (projectPath != "") {
				string p = "Assets" + projectPath.Replace (Application.dataPath, "");
				AssetDatabase.CopyAsset (p, "Assets/Resources/Prefabs/" + id + ".prefab");
				AssetDatabase.Refresh ();
			} else {
				AddPrefab<T> (id);
			}
			return LoadMonoBehaviour (id);
		} else {
			return prefab;
		}

		#else

		try {
			return LoadMonoBehaviour (id);
		} catch {
			throw new System.Exception ("No prefab named '" + id + "' exists in the Resources directory");
		}

		#endif
	}

	static MonoBehaviour LoadMonoBehaviour (string id) {
		return Resources.Load (Path + id, typeof (MonoBehaviour)) as MonoBehaviour;
	}

	#if UNITY_EDITOR
	
	[MenuItem ("Object Pool/Refresh Prefabs")]
	static void RefreshResources () {

		// Removes and replaces all the prefabs in the Resources directory
		// This will be very slow for a project with many prefabs...

		string[] files = GetPrefabsInResources ();
		foreach (string f in files) {

			string fileName = f.Replace (ResourcesPath, "").Replace (".prefab", "");
			string projectPath = FindPrefabDirectory (fileName, ApplicationPath);

			AssetDatabase.DeleteAsset ("Assets" + f.Replace (Application.dataPath, ""));

			string p = "Assets" + projectPath.Replace (Application.dataPath, "");

			AssetDatabase.CopyAsset (p, "Assets/Resources/Prefabs/" + fileName + ".prefab");
			AssetDatabase.Refresh ();
		}
	}
	
	public static void DeletePrefabsInResources () {
		string[] files = GetPrefabsInResources ();
		foreach (string f in files)
			AssetDatabase.DeleteAsset ("Assets" + f.Replace (Application.dataPath, ""));
	}

	public static string[] GetPrefabsInResources () {
		return Directory.GetFiles (ResourcesPath, "*.prefab");
	}

	static string FindPrefabDirectory (string id, string path) {

		string[] directories = Directory.GetDirectories (path);

		// TODO: http://docs.unity3d.com/ScriptReference/AssetDatabase.FindAssets.html
		foreach (string d in directories) {
			string[] files = Directory.GetFiles (d, "*.prefab");
			foreach (string f in files) {
				if (f.Contains (id + ".prefab"))
					return f;
			}
			string s = FindPrefabDirectory (id, d);
			if (s != "") return s;
		}

		return "";
	}

	static void AddPrefab<T> (string id) where T : MonoBehaviour {
		GameObject go = new GameObject (id);
		PrefabUtility.CreatePrefab ("Assets/Resources/Prefabs/" + id + ".prefab", go);
		Object.DestroyImmediate (go);
	}
	#endif
}
