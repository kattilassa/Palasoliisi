using Godot;
using System;

namespace PalaSoliisi.UI
{
    public partial class ScoreUIControl : Node
    {
        [Export] private Label _scoreLabel = null;

        public void SetScore(int score)
        {
            _scoreLabel.Text = $"Clues found: {score}/3";
        }
    }
}
