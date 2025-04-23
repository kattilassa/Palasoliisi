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

        /// <summary>
        /// When menu button clicked return to main menu
        /// </summary>
        public void OnMenuPressed()
        {
            _inGameSceneTree.ChangeSceneToFile("res://Code/UI/MainMenu.tscn");
        }

        /// <summary>
        /// Set test score to UI label
        /// </summary>
        /// <param name="score">Points from final quiz</param>
        public void SetTestScore(int score)
        {
            _testScore.Text = $"Test score: {score}/10";
        }

        /// <summary>
        /// Set minigame score to UI label
        /// </summary>
        /// <param name="score">How many turns used in minigame</param>
        public void SetMiniGameScore(int score)
        {
            _miniGameScore.Text = $"MiniGame turns used: {score}";
        }
    }
}
