using UnityEngine;

public class BackButtonElement : ButtonElement {
	public BackButtonElement (string prevScreen) : base ("Back", () => { screen.GotoScreen (prevScreen); }) {}
}