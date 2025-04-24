using Godot;
using System;

namespace PalaSoliisi
{
	public partial class PopUp : Control
	{
		[Export] private Button _okButton = null;

		public override void _Ready()
		{
			Level.Current._isDialogueRunning = true;
			_okButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnOkButtonPressed)));
		}

		/// <summary>
		/// When button is clicked close how to play pop-up and start the game
		/// </summary>
		public void OnOkButtonPressed()
		{
			Level.Current._howToPlay.Hide();
			Level.Current._isDialogueRunning = false;
			Level.Current._animationPlayer.Play("walking");
			Level.Current.Dialogue();

		}
	}
}
