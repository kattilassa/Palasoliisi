// MainMenuController.cs
// Description: Controls the main menu in the PalaSoliisi game.
// Handles navigation to other scenes such as new game, options menu, exit game, and credits.
// Author:Kati Savolainen and OnCreditsPressed method by Mimmi Tamminen.

using Godot;
using System;

namespace PalaSoliisi
{
	public partial class MainMenuController : Control
	{

		[Export] private Button _newGameButton = null;
		[Export] private Button _optionsButton = null;
		[Export] private Button _quitButton = null;
		[Export] private Button _creditsButton = null;

		private SceneTree _mainMenuSceneTree = null;

		public override void _Ready()
		{
			base._Ready();

			_mainMenuSceneTree = GetTree();
			if (_mainMenuSceneTree == null)
			{
				GD.PrintErr("SceneTree not found!");
			}

			// Connect each button's Pressed signal to its corresponding callback.
			_newGameButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnNewGamePressed)));

			_optionsButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnOptionsPressed)));

			_quitButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnQuitPressed)));

			_creditsButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnCreditsPressed)));
		}

		// Called when the New Game button is pressed and starts the game.
		private void OnNewGamePressed()
		{
			GD.Print("New game pressed");
			_mainMenuSceneTree.ChangeSceneToFile("res://Levels/Level1.tscn");
		}
		// Called when the Options button is pressed and navigates to the options menu.
		private void OnOptionsPressed()
		{
		  _mainMenuSceneTree.ChangeSceneToFile("res://Code/UI/OptionsMenu.tscn");
		}
		// Called when the exit game button is pressed and exits the game.
		private void OnQuitPressed()
		{
			_mainMenuSceneTree.Quit();
		}
		// Called when the credits button is pressed and loads the credits scene.
		private void OnCreditsPressed()
		{
			GD.Print("Credits pressed");
			_mainMenuSceneTree.ChangeSceneToFile("res://Levels/Credits.tscn");
		}
	}
}