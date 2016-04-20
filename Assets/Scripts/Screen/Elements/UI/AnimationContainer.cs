using UnityEngine;
using System.Collections;

public class AnimationContainer : MB {

	public void Reset () {
		ObjectPool.DestroyChildren<AnimElementUI> (Transform);
	}

	public void RunCoinToAvatarAnimation (string coinCount, string avatarColor) {

		// Introduce the coin
		AnimElementUI coin = CreateAnimation ();
		coin.SpriteName = "coin";
		coin.Text = "+" + coinCount;
		coin.Size = new Vector2 (50, 50);
		coin.LocalPosition = new Vector3 (-50, 25, 0);
		coin.Animate (new UIAnimator.Expand (0.5f));

		Co.WaitForSeconds (1f, () => {

			// Introduce the avatar
			Vector3 deciderPosition = new Vector3 (50, 25f, 0);
			AnimElementUI decider = CreateAnimation ();
			decider.AvatarName = avatarColor;
			decider.Size = new Vector2 (75, 75);
			decider.LocalPosition = deciderPosition;
			decider.Animate (new UIAnimator.Expand (0.5f));

			Co.WaitForSeconds (1f, () => {

				// Move the coin to the avatar and shrink out
				coin.Animate (new UIAnimator.Move (1f, deciderPosition, () => {
					coin.Destroy ();
					decider.Animate (new UIAnimator.Shrink (0.5f, () => {
						decider.Destroy ();
					}));
				}));
			});
		});
	}

	AnimElementUI CreateAnimation (Vector3 position=new Vector3()) {
		return AnimElementUI.Create (Transform, position);
	}
}
