using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Models;

public class DeckManager : GameInstanceBehaviour {

	public List<string> Names {
		get { return Decks.ConvertAll (x => x.Name); }
	}

	public Deck Deck { get; private set; }
	
	public string Name {
		get { return Deck.Name; }
	}

	public List<Role> Roles {
		get { 
			List<Role> roles = new List<Role> (Deck.Roles); 
			roles.Add (new Role () { Title = "Decider" });
			return roles;
		}
	}

	public List<string> RoleTitles {
		get { return Roles.ConvertAll (x => x.Title); }
	}

	List<Deck> decks;
	List<Deck> Decks {
		get {
			if (decks == null) {
				decks = new List<Deck> (DataManager.GetDecks ());
			}
			return decks;
		}
	}

	public PlayerAgendaItem CurrentAgendaItem { get; private set; }

	Queue<PlayerAgendaItem> agendaItems;

	public void Init () {
		Game.Dispatcher.AddListener ("SetDeck", SetDeck);
		Game.Dispatcher.AddListener ("SetAgendaItem", SetAgendaItem);
	}

	public void SetDeck (NetworkMessage msg) {
		Deck = Decks.Find (x => x.Name == msg.str1);
	}

	public Role GetRole (string title) {
		return Roles.Find (x => x.Title == title);
	}

	public string GetQuestion () {
		return DataManager.GetQuestions (Game.Decks.Name)[Game.Rounds.Current];
	}

	public void ShuffleAgendaItems (Dictionary<string, Player> players) {

		List<PlayerAgendaItem> items = new List<PlayerAgendaItem> ();
		foreach (var player in players) {
			Role r = player.Value.Role;
			for (int i = 0; i < r.AgendaItems.Length; i ++) {
				AgendaItem item = r.AgendaItems[i];
				items.Add (new PlayerAgendaItem (player.Key, item.Description, item.Reward, i));
			}
		}

		items.Shuffle<PlayerAgendaItem> ();
		agendaItems = new Queue<PlayerAgendaItem> (items);
	}

	public bool NextAgendaItem () {

		if (agendaItems.Count == 0)
			return false;

		PlayerAgendaItem i = agendaItems.Dequeue ();
		Game.Dispatcher.ScheduleMessage ("SetAgendaItem", i.Player, i.Index);
		return true;
	}

	public void SetAgendaItem (NetworkMessage msg) {
		Player p = Game.Manager.Players[msg.str1];
		AgendaItem i = p.Role.AgendaItems[msg.val];
		CurrentAgendaItem = new PlayerAgendaItem (p.Name, i.Description, i.Reward, msg.val);
	}
}
