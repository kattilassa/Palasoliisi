using Godot;
using PalaSoliisi;
using System;

public partial class InGameMenu : CenterContainer
{
	[Export] private TextureButton _exitMenuButton = null;
	[Export] private Button _menuButton = null;
	private SceneTree _inGameSceneTree = null;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_inGameSceneTree = GetTree();
		_menuButton.Connect(Button.SignalName.Pressed, new Callable(this, nameof(OnMenuPressed)));

		_exitMenuButton.Connect(Button.SignalName.Pressed,
		new Callable(this, nameof(OnSettingsPressed)));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public static void OnSettingsPressed()
		{
			Level.Current._UIpressed = true;
			if (Level.Current._showInGameMenu)
			{
				Level.Current.GetTree().Paused = false;
				Level.Current._showInGameMenu = false;
				Level.Current._inGameMenu.Hide();
				Level.Current.Movement();
			}
			else
			{
				Level.Current.GetTree().Paused = true;
				Level.Current._showInGameMenu = true;
				Level.Current._inGameMenu.Show();
				Level.Current.Movement();
			}
		}
	public void OnMenuPressed()
		{
			GetTree().Paused = false;
			_inGameSceneTree.ChangeSceneToFile("res://Code/UI/MainMenu.tscn");
		}
}
