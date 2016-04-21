using UnityEngine;
using System.Collections;

namespace Views {

	public class ViewData {}

	public class PotData : ViewData {
		public int PotCount { get; set; }
		public int DeciderCoinCount { get; set; }
		public int PlayerCoinCount { get; set; }
		public string DeciderAvatarColor { get; set; }
		public string PlayerAvatarColor { get; set; }
		public bool IsDecider { get; set; }
	}

	public class AgendaItemResultData : ViewData {
		public int CoinCount { get; set; }
		public string PlayerAvatarColor { get; set; }
	}
}