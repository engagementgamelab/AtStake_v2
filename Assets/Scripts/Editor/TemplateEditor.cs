#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Templates;

public class TemplateEditor : EditorWindow {

	TemplateManager Manager {
		get { return GetObject<TemplateManager> (); }
	}

	TemplateContainer Container {
		get { return GetObject<TemplateContainer> (); }
	}

	Template[] Templates {
		get { return Container.templates; }
	}

	DebugInfoContainer DebugInfo {
		get { return Manager.Transform.GetChild (0).GetChildren<DebugInfoContainer> ()[0]; }
	}

	string[] templateNames;
	string[] TemplateNames {
		get { 
			if (templateNames == null)
				templateNames = Templates.ToList<Template> ().ConvertAll (x => x.name).ToArray<string> ();
			return templateNames;
		}
	}

	int currTemplateIdx = 0;
	int prevTemplateIdx = -1;
	bool showDebug = false;
	bool prevShowDebug = true;

	static TemplateEditor instance;

	[MenuItem ("Window/Template Editor")]
	static void Init () {
		if (instance == null) {
			instance = CreateInstance<TemplateEditor> ();
			EditorWindow editorWindow = GetWindow<TemplateEditor> ();
			editorWindow.autoRepaintOnSceneChange = true;
			editorWindow.Show ();
			TemplateEditor.instance.BeginEdit ();
		}
	}

	void OnGUI () {

		if (Application.isPlaying) {
			GUILayout.Label ("Editor disabled in play mode");
			return;
		}

		EditorGUILayout.HelpBox ("Use the dropdown menu to select a template to edit in the hierarchy. Note that templates won't look the same as they do in the game because styling gets applied at runtime. Remember to save your changes!", MessageType.Info);
		currTemplateIdx = EditorGUILayout.Popup ("Template", currTemplateIdx, TemplateNames);

		if (currTemplateIdx != prevTemplateIdx) {
			DeactivateTemplate ();
			ActivateTemplate (Templates[currTemplateIdx]);
			prevTemplateIdx = currTemplateIdx;
		}

		if (currTemplateIdx > -1 && currTemplateIdx < Templates.Length) { 
			GUILayout.Label ("Screen Elements:");
			foreach (string id in Templates[currTemplateIdx].ElementIds) {
				GUILayout.Label (id);
			}
		}

		showDebug = GUILayout.Toggle (showDebug, "Show debug info");
		if (showDebug != prevShowDebug) {
			DebugInfo.gameObject.SetActive (showDebug);
			prevShowDebug = showDebug;
		}

		GUILayout.BeginHorizontal ();
		if (GUILayout.Button ("Connect Container"))
			ConnectContainer ();
		if (GUILayout.Button ("Disconnect Container"))
			DisconnectContainer ();
		GUILayout.EndHorizontal ();

		if (GUILayout.Button ("Save changes")) {
			ApplyChanges ();
		}
	}

	void BeginEdit () {

		ConnectContainer ();

		if (currTemplateIdx == -1) {
			currTemplateIdx = 0;
			prevTemplateIdx = 0;
		}

		ActivateTemplate (Templates[currTemplateIdx]);
	}

	void ApplyChanges () {

		// Because TemplateContainer is a separate prefab, it needs to be unparented before the modifications are applied
		DisconnectContainer ();
		DeactivateTemplate ();
		ApplyModifications<TemplateManager> ();
		ApplyModifications<TemplateContainer> ();
		BeginEdit ();
	}

	void ActivateTemplate (Template template) {
		template.gameObject.SetActive (true);
		Selection.activeGameObject = template.gameObject;
	}

	void DeactivateTemplate () {
		foreach (Template template in Templates)
			template.gameObject.SetActive (false);
	}

	void ConnectContainer () {
		TemplatesContainer templates = Manager.Transform.GetAllChildren<TemplatesContainer> ()[0];
		Container.Parent = templates.Transform;
		Container.Transform.Reset ();
		RectTransform rect = Container.GetComponent<RectTransform> ();
		rect.sizeDelta = Vector2.zero;
		rect.anchoredPosition = Vector2.zero;
	}

	void DisconnectContainer () {
		Container.Parent = null;
		Selection.activeGameObject = Container.gameObject;
	}

	T GetObject<T> (string directory="UI") where T : MonoBehaviour {

		T[] objs = Object.FindObjectsOfType (typeof (T)) as T[];
		string objName = typeof (T).Name;

		// If none found in hierarchy, create a new object from the asset database
		if (objs.Length == 0) {
			GameObject prefab = AssetDatabase.LoadAssetAtPath ("Assets/Prefabs/" + directory + "/" + objName + ".prefab", typeof (GameObject)) as GameObject;
			GameObject go = PrefabUtility.InstantiatePrefab (prefab) as GameObject;
			go.transform.Reset ();
			objs = new T[] { go.GetComponent<T> () };
		}

		// Don't allow for there to be multiple instances in the same scene
		if (objs.Length > 1)
			throw new System.Exception ("You have more than one " + objName + " in the scene. This will create problems when using the TemplateEditor.");

		return objs[0];
	}

	void ApplyModifications<T> () where T : MonoBehaviour {

		T t = GetObject<T> ();

		PrefabUtility.ReconnectToLastPrefab (t.gameObject);

		var prefabType = PrefabUtility.GetPrefabType (t);
		var parentObj = t.transform.root.gameObject;
		var prefabParent = PrefabUtility.GetPrefabParent (t);

		if (prefabType == PrefabType.PrefabInstance)
			PrefabUtility.ReplacePrefab (parentObj, prefabParent, ReplacePrefabOptions.ConnectToPrefab);
	}
}
#endif