using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// A base class for all UI elements to inherit from.
// Allows for quick access to commonly used components and callbacks.

public class UIElement : MB {

	public delegate void OnButtonPress ();

	RectTransform rectTransform = null;
	public RectTransform RectTransform {
		get {
			if (rectTransform == null) {
				rectTransform = GetComponent<RectTransform> ();
			}
			return rectTransform;
		}
	}

	Image image = null;
	public Image Image {
		get {
			if (image == null) {
				image = GetComponent<Image> ();
			}
			return image;
		}
	}

	public Sprite Sprite {
		get { return Image.sprite; }
		set { Image.sprite = value; }
	}

	Button button = null;
	public Button Button {
		get {
			if (button == null) {
				button = GetComponent<Button> ();
			}
			return button;
		}
	}

	InputField inputField = null;
	protected InputField InputField {
		get {
			if (inputField == null) {
				inputField = GetComponent<InputField> ();
			}
			return inputField;
		}
	}

	Text text = null;
	public Text Text {
		get {
			if (text == null) {
				if (Button != null) {
					text = GetChildComponent<Text> (0);
				} else if (InputField != null) {
					text = InputField.textComponent;
				} else {
					text = GetComponent<Text> ();
				}
			}
			return text;
		}
	}

	Text placeholder = null;
	public Text Placeholder {
		get {
			if (placeholder == null) {
				if (InputField != null) {
					placeholder = InputField.placeholder as Text;
				} else {
					throw new System.Exception ("No placeholder could be found because " + this + " is not an InputField");
				}
			}
			return placeholder;
		}
	}

	FontStyle style = FontStyle.Normal;
	public FontStyle Style {
		get { return Text.fontStyle; }
		set { Text.fontStyle = value; }
	}

	Animator animator;
	public Animator Animator {
		get {
			if (animator == null)
				animator = GetComponent<Animator> ();
			return animator;
		}
	}

	public bool Interactable {
		get { return Button.interactable; }
		set { Button.interactable = value; }
	}

	protected T GetChildComponent<T> (int childIndex) where T : MonoBehaviour {
		return RectTransform.GetChild (childIndex).GetComponent<T> () as T;
	}

	protected T GetChildComponent<T> (string name) where T : MonoBehaviour {
		foreach (RectTransform t in RectTransform) {
			if (t.name == name)
				return t.GetComponent<T> () as T;	
		}
		throw new System.Exception ("No child named '" + name + "' exists on " + this);
	}

	/**
	 *	Button listeners
	 */

	public void AddButtonListener (System.Action action) {
		Button.onClick.AddListener (() => { action (); });
	}

	public void RemoveButtonListener (System.Action action) {
		Button.onClick.RemoveListener (() => { action (); });
	}

	public void RemoveButtonListeners () {
		Button.onClick.RemoveAllListeners ();
	}

	/**
	 *	InputField listeners
	 */

	public void AddEndEditListener (System.Action<string> action) {
		InputField.onEndEdit.AddListener ((string s) => { action (s); });
	}

	public void AddValueChangedListener (System.Action<string> action) {
		InputField.onValueChanged.AddListener ((string s) => { action (s); });
	}

	public void RemoveInputFieldListeners () {
		InputField.onEndEdit.RemoveAllListeners ();
		InputField.onValueChanged.RemoveAllListeners ();
	}
}
