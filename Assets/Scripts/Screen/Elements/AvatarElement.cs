using UnityEngine;

public class AvatarElement : ScreenElement {

	public readonly string Name;
	public readonly string Role;
	public readonly string Color;
	public readonly int CoinCount = -1;

	public AvatarElement (string name, string color, string role="") {
		Name = name;
		Color = color;
		Role = role;
	}

	public AvatarElement (string name, string color, int coinCount) : this (name, color) {
		CoinCount = coinCount;
	}
}
