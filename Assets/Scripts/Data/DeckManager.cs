using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Models;

public class DeckManager : GameInstanceBehaviour {

	public List<string> Names {
		get { return Decks.ConvertAll (x => x.Name); }
	}

	public Deck Deck { get; private set; }
	public Role Role { get; private set; }

	public List<Role> Roles {
		get { return new List<Role> (Deck.Roles); }
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

	public void Init () {
		Game.Dispatcher.AddListener ("SetDeck", SetDeck);
		Game.Dispatcher.AddListener ("SetRole", SetRole);
	}

	public void SetDeck (NetworkMessage msg) {
		Deck = Decks.Find (x => x.Name == msg.str1);
	}

	public void SetRole (NetworkMessage msg) {
		Role = Roles.Find (x => x.Title == msg.str1);
	}
}
