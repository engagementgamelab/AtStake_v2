using UnityEngine;
using System.Collections;

namespace Models {

	public class Settings {
		public int[] PlayerCountRange { get; set; }
	}

	public class Player {
		public string Name { get; set; }
		public int CoinCount { get; set; }
		public Role Role { get; set; }
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

	public class Deck {
		public string Id { get; set; }
		public string[] Questions { get; set; }
		public Role[] Roles { get; set; }
	}
}