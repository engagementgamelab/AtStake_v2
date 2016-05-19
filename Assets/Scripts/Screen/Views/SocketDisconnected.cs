using UnityEngine;
using System.Collections;

namespace Views {

	public class SocketDisconnected : View {

		protected override void OnInitElements () {
			Elements.Add ("close_app", new TextElement (Model.Text["close_app"]));
		}

		public override void OnDisconnect () {}
	}
}