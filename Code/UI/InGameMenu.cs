// InGameMenu.cs
// Manages the in-game menu and pauses the game.
// Author: Kati Savolainen
using Godot;
using PalaSoliisi;
using System;

public partial class InGameMenu : CenterContainer
{
	// References to UI buttons, assigned via the Godot editor.
	[Export] private TextureButton _exitMenuButton = null;
	[Export] private Button _menuButton = null;
	// Reference to the SceneTree.
	private SceneTree _inGameSceneTree = null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Get the SceneTree instance for this scene.
		_inGameSceneTree = GetTree();
		// Connect the menu button to the OnMenuPressed method.
		_menuButton.Connect(Button.SignalName.Pressed, new Callable(this, nameof(OnMenuPressed)));

		// Connect the exit to menu button to the OnSettingsPressed method.
		_exitMenuButton.Connect(Button.SignalName.Pressed,
		new Callable(this, nameof(OnSettingsPressed)));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Called when the exit in-game menu button or settings button is pressed.
	// Pauses or unpauses the game and shows/hides the in-game menu.
	public static void OnSettingsPressed()
		{
			Level.Current._UIpressed = true;
			// Flag indicating if the UI was pressed so character doesn't move.

			// Toggle in-game menu visibility and pause state.
			if (Level.Current._showInGameMenu)
			{
				Level.Current.GetTree().Paused = false; //Unpause the game.
				Level.Current._showInGameMenu = false;
				Level.Current._inGameMenu.Hide(); //Hide in-game settings.
				Level.Current.Movement(); //wait 1 sec before character can move.
			}
			else
			{
				Level.Current.GetTree().Paused = true; //Pause the game.
				Level.Current._showInGameMenu = true;
				Level.Current._inGameMenu.Show(); //Show in-game settings.
				Level.Current.Movement(); //wait 1 sec before character can move.
			}
		}

	// Called when the back to the main menu button is pressed.
	// Changes the scene to the main menu and unpauses the game.
	public void OnMenuPressed()
		{
			GetTree().Paused = false;
			_inGameSceneTree.ChangeSceneToFile("res://Code/UI/MainMenu.tscn");
		}
}
