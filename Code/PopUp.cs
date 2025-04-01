using Godot;
using System;

namespace PalaSoliisi
{
	public partial class PopUp : Node
	{
		[Export] private Button _okButton = null;

		public override void _Ready()
		{
			_okButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnOkButtonPressed)));
		}

		private void OnOkButtonPressed()
		{
			// Pois / piilota ??
			//this.Hide();
			this.QueueFree();
			Level.Current.Dialogue();
		}
	}
}
