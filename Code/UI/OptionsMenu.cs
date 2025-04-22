// OptionsMenu.cs
// This script handles the functionality of the options menu in the PalaSoliisi game.
// From options menu you can return to main menu or adjust audio settings.
// Author: 	Kati Savolainen
using Godot;
using System;

namespace PalaSoliisi
{
	public partial class OptionsMenu : Control
	{
		// Reference to the button that returns to the main menu, set through the Godot editor.
		[Export] private Button _MenuButton = null;
		// Reference to the current SceneTree, used for changing scenes.
		private SceneTree _optionsSceneTree = null;

		public override void _Ready()
		{
			base._Ready();
		// Get the SceneTree for later use.
			_optionsSceneTree = GetTree();
			if (_optionsSceneTree == null)
			{
				GD.PrintErr("SceneTree not found!");
			}
			// Connect the Menu button's pressed signal to the OnMenuPressed handler.
			_MenuButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnMenuPressed)));
		}
		// This method is called when the menu button is pressed.
		// It changes the current scene back to the Main Menu scene.
		private void OnMenuPressed()
		{
			_optionsSceneTree.ChangeSceneToFile("res://Code/UI/MainMenu.tscn");
		}
	}
}