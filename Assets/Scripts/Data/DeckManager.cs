using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Models;

//// <summary>
/// Interfaces with DataManager to retrieve the available decks; keeps track of which deck is being used for this game instance
/// </summary>
public class DeckManager : GameInstanceBehaviour {

	/// <summary>
	/// Gets a list of the names of available decks
	/// </summary>
	public List<string> Names {
		get { return Decks.ConvertAll (x => x.Name); }
	}

	/// <summary>
	/// Gets the deck currently being used in this game
	/// </summary>
	public Deck Deck { get; private set; }

	/// <summary>
	/// Gets a list of the roles in the current deck
	/// </summary>
	public List<Role> Roles {
		get { 
			List<Role> roles = new List<Role> (Deck.Roles); 
			roles.Add (new Role () { Title = "Decider" });
			return roles;
		}
	}

	/// <summary>
	/// Gets a list of available decks
	/// </summary>
	List<Deck> Decks {
		get {
			if (decks == null)
				decks = new List<Deck> (DataManager.GetDecks ());
			return decks;
		}
	}
	List<Deck> decks;

	public void Init () {
		Game.Dispatcher.AddListener ("SetDeck", SetDeck);
	}

	public void Reset () {
		Deck = null;
	}

	/// <summary>
	/// Set the deck to use in this game
	/// </summary>
	public void SetDeck (NetMessage msg) {
		Deck = Decks.Find (x => x.Name == msg.str1);
	}
}
