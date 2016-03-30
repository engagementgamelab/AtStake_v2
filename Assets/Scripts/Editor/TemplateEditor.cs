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
	int prevTemplateIdx = 0;
	Template currentTemplate;
	bool showDebug = false;
	bool prevShowDebug = true;

	static TemplateEditor instance;

	[MenuItem ("Window/Template Editor")]
	static void Init () {
		if (instance == null) {
			instance = new TemplateEditor ();
			EditorWindow editorWindow = GetWindow<TemplateEditor> ();
			editorWindow.autoRepaintOnSceneChange = true;
			editorWindow.Show ();
			TemplateEditor.instance.BeginEdit ();
		}
	}

	void OnGUI () {

		EditorGUILayout.HelpBox ("Use the dropdown menu to select a template to edit in the hierarchy. Note that templates won't look the same as they do in the game because styling gets applied at runtime. Remember to save your changes!", MessageType.Info);
		currTemplateIdx = EditorGUILayout.Popup ("Template", currTemplateIdx, TemplateNames);

		if (currTemplateIdx != prevTemplateIdx) {
			DeactivateTemplate ();
			ActivateTemplate (Templates[currTemplateIdx]);
			prevTemplateIdx = currTemplateIdx;
		}

		showDebug = GUILayout.Toggle (showDebug, "Show debug info");
		if (showDebug != prevShowDebug) {
			DebugInfo.gameObject.SetActive (showDebug);
			prevShowDebug = showDebug;
		}

		if (GUILayout.Button ("Save changes")) {
			ApplyChanges ();
		}
	}

	void BeginEdit () {
		TemplatesContainer templates = Manager.Transform.GetAllChildren<TemplatesContainer> ()[0];
		Container.Parent = templates.Transform;
		Container.Transform.Reset ();
		RectTransform rect = Container.GetComponent<RectTransform> ();
		rect.sizeDelta = Vector2.zero;
		rect.anchoredPosition = Vector2.zero;
		ActivateTemplate (Templates[0]);
	}

	void ApplyChanges () {

		// Because TemplateContainer is a separate prefab, it needs to be unparented before the modifications are applied
		Container.Parent = null;
		DeactivateTemplate ();
		ApplyModifications<TemplateManager> ();
		ApplyModifications<TemplateContainer> ();
		BeginEdit ();
	}

	void ActivateTemplate (Template template) {
		currentTemplate = template;
		currentTemplate.gameObject.SetActive (true);
		Selection.activeGameObject = currentTemplate.gameObject;
	}

	void DeactivateTemplate () {
		if (currentTemplate != null) {
			currentTemplate.gameObject.SetActive (false);
			currentTemplate = null;
		}
	}

	T GetObject<T> (string directory="UI") where T : MonoBehaviour {
		T[] objs = Object.FindObjectsOfType (typeof (T)) as T[];
		string objName = typeof (T).Name;
		if (objs.Length == 0) {
			GameObject prefab = AssetDatabase.LoadAssetAtPath ("Assets/Prefabs/" + directory + "/" + objName + ".prefab", typeof (GameObject)) as GameObject;
			GameObject go = PrefabUtility.InstantiatePrefab (prefab) as GameObject;
			go.transform.Reset ();
			return go.GetComponent<T> ();
		}
		if (objs.Length > 1)
			throw new System.Exception ("You have more than one " + objName + " in the scene. This will create problems when using the TemplateEditor.");
		return objs[0];
	}

	void ApplyModifications<T> () where T : MonoBehaviour {
		T t = GetObject<T> ();
		var prefabType = PrefabUtility.GetPrefabType (t);
		var parentObj = t.transform.root.gameObject;
		var prefabParent = PrefabUtility.GetPrefabParent (t);
		if (prefabType == PrefabType.PrefabInstance)
			PrefabUtility.ReplacePrefab (parentObj, prefabParent, ReplacePrefabOptions.ConnectToPrefab);
	}
}
#endif