using Godot;
using System;

namespace PalaSoliisi
{
	public partial class OptionsMenu : Control
	{

		[Export] private Button _MenuButton = null;
		//[Export] private DialogWindow _optionsWindow = null;

		private SceneTree _optionsSceneTree = null;

		public override void _Ready()
		{
			base._Ready();

			_optionsSceneTree = GetTree();
			if (_optionsSceneTree == null)
			{
				GD.PrintErr("SceneTree not found!");
			}

			_MenuButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnMenuPressed)));
		}

		private void OnMenuPressed()
		{
			_optionsSceneTree.ChangeSceneToFile("res://Code/UI/MainMenu.tscn");
		}
	}
}