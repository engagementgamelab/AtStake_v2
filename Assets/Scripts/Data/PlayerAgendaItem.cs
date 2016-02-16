using UnityEngine;
using System.Collections;

public class PlayerAgendaItem {

	public readonly string Player;
	public readonly string Description;
	public readonly int Reward;
	public readonly int Index;

	public PlayerAgendaItem (string player, string description, int reward, int index) {
		Player = player;
		Description = description;
		Reward = reward;
		Index = index;
	}
}