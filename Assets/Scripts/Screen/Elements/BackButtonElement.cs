using UnityEngine;

public class BackButtonElement : ButtonElement {

	// First argument is the screen to "go back" to
	// Second argument is optional and allows you to carry out any additional actions on going back
	public BackButtonElement (string prevScreen, System.Action onPress=null) : 
		base ("Back", () => { 
			screen.GotoScreen (prevScreen);
			if (onPress != null)
				onPress ();
		}) {}
}