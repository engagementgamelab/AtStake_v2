using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Views {

	// The initial screen on loading the game

	public class Start : View {

		ConnectionStatus Status {
			get { return Game.Multiplayer.ConnectionStatus; }
		}

		bool Connected {
			get { return Status == ConnectionStatus.Success; }
		}

		ButtonElement submitButton;

		protected override void OnInitElements () {
			
			Elements.Add ("logo", new ImageElement ("logo"));
			Elements.Add ("connection_failed", new TextElement (GetText ("connection_failed")) { Active = !Connected });

			submitButton = new ButtonElement (GetButton ("submit"), () => { 
				if (Game.Manager.Name != "" && Connected)
					GotoView ("hostjoin");
			}) {
				#if !SINGLE_SCREEN
				Interactable = false 
				#endif
			};

			Elements.Add ("submit", submitButton);

			Elements.Add ("input", new InputElement ("your name", (string name) => {
				#if !SINGLE_SCREEN
				submitButton.Interactable = name != "" && Connected;
				#endif
			}, (string name) => {
				Game.Manager.Name = name;

				// This allows the name to be submitted by pressing "done" on the ios/android keyboard
				if (name != "" && Connected)
					GotoView ("hostjoin");
			}));
		}

		protected override void OnShow () {
			Game.Multiplayer.onUpdateConnectionStatus += OnUpdateConnectionStatus;
		}

		protected override void OnHide () {
			Game.Multiplayer.onUpdateConnectionStatus -= OnUpdateConnectionStatus;
		}

		void OnUpdateConnectionStatus (ConnectionStatus status) {
			bool failed = status == ConnectionStatus.Fail;
			Elements["connection_failed"].Active = failed;
			submitButton.Interactable = !failed;
		}
	}
}