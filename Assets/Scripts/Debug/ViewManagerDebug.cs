using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Views;

public class ViewManagerDebug : MonoBehaviour {

	ViewManager views;
	public Text current;
	public Text previous;
	public Text inputText;

	public void Init (ViewManager views) {
		#if SHOW_DEBUG_INFO
		this.views = views;
		#endif
	}

	#if SHOW_DEBUG_INFO
	void Update () {
		if (views == null) return;
		current.text = "Current view: " + views.CurrView;
		previous.text = "Previous view: " + views.PrevView;
	}
	#endif

	public void OnSubmit () {
		// TODO: use GameInstanceManager to skip to view
	}
}
