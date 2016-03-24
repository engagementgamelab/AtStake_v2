using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Models;
using JsonFx.Json;

public class GameController : GameInstanceBehaviour {

	InstanceData instance;

	public PlayerRole[] Roles {
		get { return CurrentRound.Roles; }
	}

	public PlayerRole Role {
		get { 
			if (instance == null)
				return null;
			try {
				return System.Array.Find (Roles, x => x.PlayerName == Game.Name); 
			} catch {
				throw new System.Exception ("Could not find a role for the player " + Game.Name);
			}
		}
	}

	public bool DataLoaded {
		get { return instance != null; }
	}

	Round CurrentRound {
		get { return instance.Rounds[instance.RoundIndex]; }
	}

	List<string> PlayerNames {
		get { return instance.Players.ToList ().ConvertAll (x => x.Name); }
	}

	int PlayerCount {
		get { return instance.Players.Length; }
	}

	bool Hosting {
		get { return Game.Multiplayer.Hosting;}
	}

	List<byte[]> receivedChunks = new List<byte[]> ();
	const int maxDataSize = 1000;

	public void Init () {
		Game.Dispatcher.AddListener ("StartGame", InitializeInstanceData);
		Game.Dispatcher.AddListener ("PreGotoView", GotoView);
		Game.Dispatcher.AddListener ("InstanceDataLoaded", LoadInstanceData);
	}

	public void Reset () {
		instance = null;
	}

	void Setup () {
		instance = new InstanceData ();
		try {
			instance.DeckName = Game.Decks.Deck.Name;
		} catch {
			throw new System.Exception ("Failed to setup game controller because a deck has not been chosen.");
		}
		try {
			instance.Players = Game.Manager.Players.Values.ToArray ();
		} catch {
			throw new System.Exception ("Failed to setup game controller because the players have not been set.");
		}
		PopulateRoundData ();
		SendData ();
	}

	void PopulateRoundData () {
		
		if (!Hosting) return;

		// Populates data for all rounds in the instance of this game
		// Sets questions and randomly assigns roles, making sure that every player is the decider
		// Randomly sets the order players will pitch their proposals
		// Randomly sets the order agenda items appear when bonus points are being awarded
		
		Round[] rounds = new Round[3];
		string[] questions = DataManager.GetQuestions (instance.DeckName);
		List<string> deciderOrder = PlayerNames.ToShuffled ();

		for (int i = 0; i < rounds.Length; i ++) {

			// Create the round and set the question
			rounds[i] = new Round ();
			rounds[i].Question = questions[i];

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
					roles[j].Bio = role.Bio;
					roles[j].AgendaItems = role.AgendaItems;
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
			List<PlayerAgendaItem2> items = new List<PlayerAgendaItem2> ();
			for (int j = 0; j < roles.Length; j ++) {
				PlayerRole r = roles[j];
				if (r.Title == "Decider")
					continue;

				for (int k = 0; k < r.AgendaItems.Length; k ++) {

					AgendaItem item = r.AgendaItems[k];

					items.Add (new PlayerAgendaItem2 () {
						PlayerName = r.PlayerName,
						Description = item.Description,
						Reward = item.Reward,
						Index = k
					});
				}
			}

			rounds[i].AgendaItemOrder = items.ToShuffled ().ToArray<PlayerAgendaItem2> ();
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

	void GotoView (MasterMsgTypes.GenericMessage msg) {
		// if (msg.str1 == "roles") {
			// if (Hosting && instance == null) 
				// Setup ();
		// }
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
			Debug.Log ("Question: " + r.Question);
			Debug.Log ("Roles:");
			for (int j = 0; j < r.Roles.Length; j ++) {
				PlayerRole role = r.Roles[j];
				Debug.Log (role.PlayerName + ": " + role.Title);
			}
			Debug.Log ("Pitch order:");
			for (int j = 0; j < r.PitchOrder.Length; j ++) {
				Debug.Log (j + ": " + r.PitchOrder[j]);
			}
			Debug.Log ("Agenda item order:");
			for (int j = 0; j < r.AgendaItemOrder.Length; j ++) {
				PlayerAgendaItem2 item = r.AgendaItemOrder[j];
				Debug.Log (item.Index + ": " + item.PlayerName + " - " + item.Description + " (" + item.Reward + ")");
			}
		}
	}
}
