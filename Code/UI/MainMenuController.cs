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

			// Tämä rivi aloittaa painikkeen Pressed-signaalin kuuntelun.
			_newGameButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnNewGamePressed)));

			_optionsButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnOptionsPressed)));

			_quitButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnQuitPressed)));

			_creditsButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnCreditsPressed)));
		}

		private void OnNewGamePressed()
		{
			GD.Print("New game pressed");
			_mainMenuSceneTree.ChangeSceneToFile("res://Levels/Level1.tscn");
		}

		private void OnOptionsPressed()
		{
		  _mainMenuSceneTree.ChangeSceneToFile("res://Code/UI/OptionsMenu.tscn");
		}

		private void OnQuitPressed()
		{
			_mainMenuSceneTree.Quit();
		}

		private void OnCreditsPressed()
		{
			GD.Print("Credits pressed");
			_mainMenuSceneTree.ChangeSceneToFile("res://Levels/Credits.tscn");
		}
	}
}