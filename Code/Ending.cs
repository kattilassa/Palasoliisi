using Godot;
using System;

namespace PalaSoliisi
{
    public partial class Ending : Node
    {
        [Export] private Button _menuButton = null;
        [Export] private Label _testScore = null;
        [Export] private Label _miniGameScore = null;
       private SceneTree _inGameSceneTree = null;

        public override void _Ready()
		{
            _inGameSceneTree = GetTree();
        	_menuButton.Connect(Button.SignalName.Pressed, new Callable(this, nameof(OnMenuPressed)));
        }
        public void OnMenuPressed()
        {
            _inGameSceneTree.ChangeSceneToFile("res://Code/UI/MainMenu.tscn");
        }
        public void SetTestScore(int score)
        {
            _testScore.Text = $"Test score: {score}/10";
        }

        public void SetMiniGameScore(int score)
        {
            _miniGameScore.Text = $"MiniGame turns used: {score}";
        }
    }
}
