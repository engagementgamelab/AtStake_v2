using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Models {

	/// <summary>
    /// Stores game config schema.
    /// </summary>
    public class GameConfig {

        public GameEnvironment local { get; set; }
        public GameEnvironment development { get; set; }
        public GameEnvironment staging { get; set; }
        public GameEnvironment production { get; set; }

    }

    /// <summary>
    /// Stores game config environment schema.
    /// </summary>
    public class GameEnvironment {

        public string root { get; set; }
        public string authKey { get; set; }
        public string multiplayerServerIp { get; set; }
        public int multiplayerServerPort { get; set; }
        public int facilitatorPort { get; set; }

    }

    public class GameData {

    	public Settings Settings { get; set; }
    	public Deck[] Decks { get; set; }
    	public Screen[] Screens { get; set; }

    }

	public class Settings {
		public int[] PlayerCountRange { get; set; }
		public int PotCoinCount { get; set; }
		public int PlayerStartCoinCount { get; set; }
		public int DeciderStartCoinCount { get; set; }
		public int[] Rewards { get; set; }
		public int ExtraTimeCost { get; set; }
		public float ThinkSeconds { get; set; }
		public float PitchSeconds { get; set; }
		public float ExtraSeconds { get; set; }
		public float DeliberateSeconds { get; set; }
	}

	public class Player {
		public string Name { get; set; }
		public int CoinCount { get; set; }
		public Role Role { get; set; }
		public bool HasBeenDecider { get; set; }
	}

	public class Deck {
		public string Name { get; set; }
		public string Description { get; set; }
		public string[] Questions { get; set; }
		public Role[] Roles { get; set; }
	}
	
	public class Role {
		public string Title { get; set; }
		public string Bio { get; set; }
		public AgendaItem[] AgendaItems { get; set; }
	}

	public class AgendaItem {
		public string Description { get; set; }
		public int Reward { get; set; }
	}

	public class Screen {

		public string Symbol { get; set; }

		// Leave blank to not display a name
		public string DisplayName { get; set; }

		public Dictionary<string, string> Buttons { get; set; }		
		public Dictionary<string, string> Text { get; set; }

		// Instructions that everyone sees
		public string Instructions { get; set; }

		// Instructions that only the Decider sees
		public string DeciderInstructions { get; set; }

		// Instructions that only the players see (not the Decider)
		public string PlayerInstructions { get; set; }

		// Instructions that only the Host sees
		public string HostInstructions { get; set; }

		// Instructions that only clients see (not the host)
		public string ClientInstructions { get; set; }
	}

	// -- The following models are populated during a game instance

	public class InstanceData {
		public string DeckName { get; set; }
		public Player[] Players { get; set; }
		public Round[] Rounds { get; set; }
		public int Pot { get; set; }
		public int RoundIndex { get; set; }
	}

	public class Round {
		public string Question { get; set; }
		public PlayerRole[] Roles { get; set; }
		public string[] PitchOrder { get; set; }

		// could bring down the size of the model by making this a 2d int array where [playerindex, agendaitemindex]
		// then rebuild PlayerAgendaItem on load
		public PlayerAgendaItem2[] AgendaItemOrder { get; set; }
		public int AgendaItemIndex { get; set; }
		public int PitchIndex { get; set; }
		public string Winner { get; set; }

		public string Decider {
			get { return System.Array.Find (Roles, x => x.Title == "Decider").PlayerName; }
		}
	}

	public class PlayerRole : Role {
		public string PlayerName { get; set; }
	}

	public class PlayerAgendaItem2 : AgendaItem {
		public string PlayerName { get; set; }
		public int Index { get; set; }
	}
}