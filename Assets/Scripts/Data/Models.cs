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

    }

    public class GameData {

    	public Settings Settings { get; set; }
    	public Deck[] Decks { get; set; }

    }

	public class Settings {
		public int[] PlayerCountRange { get; set; }
		public int PotCoinCount { get; set; }
		public int PlayerStartCoinCount { get; set; }
		public int DeciderStartCoinCount { get; set; }
		public int RewardOne { get; set; }
		public int RewardTwo { get; set; }
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
}