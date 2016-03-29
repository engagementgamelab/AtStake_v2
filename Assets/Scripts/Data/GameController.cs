using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Models;
using JsonFx.Json;

public class GameController : GameInstanceBehaviour {

	// -- Public properties

	/// <summary>
	/// Gets the data models for this round's roles
	/// </summary>
	public PlayerRole[] Roles {
		get { 

			// In order to keep the message size small, bio and agenda items aren't included in the instance data,
			// so these fields are populated from the deck data the first time they're referenced

			PlayerRole[] roles = CurrentRound.Roles; 
			Role[] deckRoles = Game.Decks.Deck.Roles;

			foreach (PlayerRole role in roles) {
				if (role.Title != "Decider" && string.IsNullOrEmpty (role.Bio)) {
					Role r = System.Array.Find (deckRoles, x => x.Title == role.Title);
					role.Bio = r.Bio;
					role.AgendaItems = r.AgendaItems;
				}
			}
			return roles;
		}
	}

	/// <summary>
	/// Gets the data model associated with this player's role
	/// </summary>
	public PlayerRole Role {
		get { 
			if (!DataLoaded)
				return null;
			try {
				return System.Array.Find (Roles, x => x.PlayerName == Game.Name); 
			} catch {
				throw new System.Exception ("Could not find a role for the player " + Game.Name);
			}
		}
	}

	/// <summary>
	/// Gets the data model associated with the current Decider
	/// </summary>
	public Player Decider {
		get {
			if (!DataLoaded)
				return null;
			string deciderName = System.Array.Find (Roles, x => x.Title == "Decider").PlayerName;
			return FindPlayer (deciderName);
		}
	}

	/// <summary>
	/// Gets the name of the current Decider
	/// </summary>
	public string DeciderName {
		get { return Decider == null ? "" : Decider.Name; }
	}

	/// <summary>
	/// Gets the question for the current round
	/// </summary>
	public string Question {
		get { return Game.Decks.Deck.Questions[instance.RoundIndex]; }
	}

	/// <summary>
	/// Gets the data models of the players
	/// </summary>
	public Player[] Players {
		get { return instance.Players; }
	}

	/// <summary>
	/// Gets the data model of the winning player
	/// </summary>
	public Player Winner {
		get {
			if (string.IsNullOrEmpty (WinnerName))
				return null;
			return System.Array.Find (Players, x => x.Name == WinnerName);
		}
	}

	/// <summary>
	/// Gets/sets the name of the round's winner
	/// </summary>
	public string WinnerName {
		get {
			if (!DataLoaded)
				return "";
			return CurrentRound.Winner;
		}
		set { CurrentRound.Winner = value; }
	}

	/// <summary>
	/// Gets the number of players, excluding this one
	/// </summary>
	public int PeerCount {
		get { return PlayerCount-1; }
	}

	/// <summary>
	/// Gets the names of all players, excluding this one
	/// </summary>
	public List<string> PeerNames {
		get { return PlayerNames.FindAll (x => x != Game.Name); }
	}

	/// <summary>
	/// Gets/sets the amount of coins in the pot
	/// </summary>
	public int Pot {
		get { return DataLoaded ? instance.Pot : 0; }
		set { instance.Pot = value; }
	}	

	/// <summary>
	/// Gets/sets this player's coin count
	/// </summary>
	public int CoinCount {
		get { return DataLoaded ? FindPlayer (Game.Name).CoinCount : 0; }
		set { FindPlayer (Game.Name).CoinCount = value; }
	}

	/// <summary>
	/// Gets the round number we're currently on
	/// </summary>
	public int RoundNumber {
		get {
			if (!DataLoaded)
				return -1;
			return instance.RoundIndex;
		}
	}

	/// <summary>
	/// Gets the data model for the current round
	/// </summary>
	public Round CurrentRound {
		get { 
			if (roundItr.Position > instance.Rounds.Length-1)
				return null;
			return instance.Rounds[roundItr.Position];
		}
	}

	/// <summary>
	/// Gets the name of the player currently pitching
	/// </summary>
	public string CurrentPitcher {
		get { 
			if (pitchItr.Position > CurrentRound.PitchOrder.Length-1)
				return "";
			return CurrentRound.PitchOrder[pitchItr.Position]; 
		}
	}

	/// <summary>
	/// Gets the data model for the current agenda item
	/// </summary>
	public PlayerAgendaItem CurrentAgendaItem {
		get {
			
			if (agendaItemItr.Position > CurrentRound.AgendaItemOrder.Length-1)
				return null;

			// In order to keep the message size small, agenda item data isn't included in the instance data,
			// so these fields are populated from the deck data as they're referenced

			int[] item = CurrentRound.AgendaItemOrder[agendaItemItr.Position];
			string playerName = Players[item[0]].Name;
			AgendaItem agendaItem = System.Array.Find (Roles, x => x.PlayerName == playerName)
				.AgendaItems[item[1]];

			return new PlayerAgendaItem () {
				PlayerName = playerName,
				Description = agendaItem.Description,
				Reward = agendaItem.Reward
			};
		}
	}

	/// <summary>
	/// Returns true if the instance data has been loaded
	/// </summary>
	public bool DataLoaded {
		get { return instance != null; }
	}

	// -- Private properties

	List<string> PlayerNames {
		get { return Players.ToList ().ConvertAll (x => x.Name); }
	}

	int PlayerCount {
		get { return Players.Length; }
	}

	bool Hosting {
		get { return Game.Multiplayer.Hosting;}
	}

	// Data model for this game instance
	InstanceData instance;

	// Used to send the serialized model data to clients
	List<byte[]> receivedChunks = new List<byte[]> ();
	const int maxDataSize = 1000;

	// Iterators
	ArrayIterator roundItr; 
	ArrayIterator pitchItr;
	ArrayIterator agendaItemItr;

	public void Init () {

		Game.Dispatcher.AddListener ("StartGame", InitializeInstanceData);
		Game.Dispatcher.AddListener ("InstanceDataLoaded", LoadInstanceData);

		roundItr = new ArrayIterator ("round", Game, (int position) => { instance.RoundIndex = position; });
		pitchItr = new ArrayIterator ("pitch", Game, (int position) => { CurrentRound.PitchIndex = position; });
		agendaItemItr = new ArrayIterator ("agenda_item", Game, (int position) => { CurrentRound.AgendaItemIndex = position; });
		Debug.Log ("heard");
	}

	public void Reset () {
		instance = null;
	}

	public Player FindPlayer (string playerName) {
		try {
			return System.Array.Find (Players, x => x.Name == playerName);
		} catch {
			throw new System.Exception ("Could not find a player with the name '" + playerName + "'");
		}
	}

	public void SetWinner (string winnerName) {
		WinnerName = winnerName;
	}

	public bool NextRound () {
		roundItr.Next (); 
		return roundItr.Position <= instance.Rounds.Length-1;
	}

	public bool NextPitch () { 
		pitchItr.Next (); 
		return pitchItr.Position <= CurrentRound.PitchOrder.Length-1;
	}

	public bool NextAgendaItem () {
		agendaItemItr.Next (); 
		return agendaItemItr.Position <= CurrentRound.AgendaItemOrder.Length-1;
	}

	void Setup () {
		instance = new InstanceData ();
		try {
			instance.DeckName = Game.Decks.Deck.Name;
		} catch {
			throw new System.Exception ("Failed to setup game controller because a deck has not been chosen.");
		}
		try {
			instance.Players = Game.Manager.Players.ToArray ();
		} catch {
			throw new System.Exception ("Failed to setup game controller because the players have not been set.");
		}
		PopulateData ();
		SendData ();
	}

	void StartRound () {
		NextRound ();
	}

	void PopulateData () {
		
		if (!Hosting) return;

		// Populates data for all rounds in the instance of this game
		// Sets questions and randomly assigns roles, making sure that every player is the decider
		// Randomly sets the order players will pitch their proposals
		// Randomly sets the order agenda items appear when bonus points are being awarded
		
		Round[] rounds = new Round[3];
		List<string> deciderOrder = PlayerNames.ToShuffled ();

		for (int i = 0; i < rounds.Length; i ++) {

			// Create the round and set the question
			rounds[i] = new Round ();

			// -- Roles

			// Randomize the roles
			Queue<Role> deckRoles = new Queue<Role> (Game.Decks.Deck.Roles.ToList<Role> ().ToShuffled ());
			PlayerRole[] roles = new PlayerRole[PlayerCount];

			for (int j = 0; j < roles.Length; j ++) {

				// Create the role
				string playerName = PlayerNames[j];
				roles[j] = new PlayerRole ();
				roles[j].PlayerName = playerName;

				// Check if this player is the Decider for this round
				if (deciderOrder[i] == playerName) {
					roles[j].Title = "Decider";
				} else {
					Role role = deckRoles.Dequeue ();
					roles[j].Title = role.Title;
					roles[j].AgendaItems = new AgendaItem[role.AgendaItems.Length];
				}
			}

			rounds[i].Roles = roles;


			// -- Pitch

			// Randomly set the pitch order
			rounds[i].PitchOrder = PlayerNames
				.ToShuffled ()
				.FindAll (x => x != rounds[i].Decider)
				.ToArray<string> ();


			// -- Agenda Items

			// Randomly set the agenda item order
			List<int[]> items2 = new List<int[]> ();
			for (int j = 0; j < roles.Length; j ++) {
				PlayerRole r = roles[j];
				if (r.Title == "Decider")
					continue;

				int playerIndex = System.Array.FindIndex (Players, x => x.Name == r.PlayerName);
				for (int k = 0; k < r.AgendaItems.Length; k ++) {
					items2.Add (new int[] { playerIndex, k });
				}
			}

			rounds[i].AgendaItemOrder = items2.ToShuffled ().ToArray<int[]> ();
		}

		instance.Rounds = rounds;

		// PrintData ();
	}

	void SendData () {
		
		// (Host) Serialize the data, compress the resultant string, and send the data in chunks not larger than maxDataSize

		string data = JsonWriter.Serialize (instance);
		byte[] compressed = CLZF2.Compress (System.Text.Encoding.UTF8.GetBytes (data));
		int chunkCount = Mathf.CeilToInt (compressed.Length / maxDataSize);

		List<byte>[] chunks = new List<byte>[chunkCount];
		int chunkPosition = 0;

		foreach (byte b in compressed) {

			if (chunks[chunkPosition] == null)
				chunks[chunkPosition] = new List<byte> ();

			chunks[chunkPosition].Add (b);

			if (chunkPosition < chunks.Length-1)
				chunkPosition ++;
			else
				chunkPosition = 0;
		}

		for (int i = chunks.Length-1; i >= 0; i --) {
			Game.Dispatcher.ScheduleMessage ("InstanceDataLoaded", i, chunks[i].ToArray<byte> ());
		}
	}

	void InitializeInstanceData (MasterMsgTypes.GenericMessage msg) {
		if (Hosting && instance == null) 
			Setup ();
	}

	void LoadInstanceData (MasterMsgTypes.GenericMessage msg) {

		if (Hosting) return;

		// (Clients) Receive chunks. Once all chunks have been received, reassemble, decompress, and deserialize the data

		receivedChunks.Add (msg.bytes);

		if (msg.val == 0) {

			int byteCount = 0;
			int chunkCount = receivedChunks.Count;
			foreach (byte[] chunk in receivedChunks)
				byteCount += chunk.Length;

			byte[] decompressed = new byte[byteCount];
			receivedChunks.Reverse ();

			for (int i = 0; i < chunkCount; i ++) {
				byte[] chunk = receivedChunks[i];
				for (int j = 0; j < chunk.Length; j ++) {
					decompressed[i + j * chunkCount] = chunk[j];
				}
			}

			byte[] d = CLZF2.Decompress (decompressed);
			string data = System.Text.Encoding.UTF8.GetString (d, 0, d.Length);
			
			instance = JsonReader.Deserialize<InstanceData> (data);
			// PrintData ();
		}
	}

	void PrintData () {
		for (int i = 0; i < instance.Rounds.Length; i ++) {
			Round r = instance.Rounds[i];
			Debug.Log ("----------------");
			Debug.Log ("round " + i);
			// Debug.Log ("Question: " + r.Question);
			Debug.Log ("Roles:");
			for (int j = 0; j < r.Roles.Length; j ++) {
				PlayerRole role = r.Roles[j];
				Debug.Log (role.PlayerName + ": " + role.Title);
			}
			Debug.Log ("Pitch order:");
			for (int j = 0; j < r.PitchOrder.Length; j ++) {
				Debug.Log (j + ": " + r.PitchOrder[j]);
			}
		}
	}

	class ArrayIterator {

		readonly string id;
		int position = 0;

		public int Position {
			get { return position; }
		}

		GameInstance game;
		System.Action<int> onNext;

		public ArrayIterator (string id, GameInstance game, System.Action<int> onNext) {
			this.id = id;
			this.game = game;
			this.onNext = onNext;
			game.Dispatcher.AddListener ("_Next", OnNext);
		}

		public void Next () {
			position ++;
			game.Dispatcher.ScheduleMessage ("_Next", id, position);
		}

		void OnNext (MasterMsgTypes.GenericMessage msg) {
			if (msg.str1 == id) {
				position = msg.val;
				onNext (position);
			}
		}
	}
}
