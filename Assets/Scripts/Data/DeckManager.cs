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

	public void Init () {
		Game.Dispatcher.AddListener ("SetDeck", SetDeck);
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
}
