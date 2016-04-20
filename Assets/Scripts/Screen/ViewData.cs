using UnityEngine;
using System.Collections;

namespace Views {

	public class ViewData {}

	public class PotData : ViewData {
		public int DeciderCoinCount { get; set; }
		public int PlayerCoinCount { get; set; }
		public string DeciderAvatarColor { get; set; }
	}
}