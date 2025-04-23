using Godot;
using System;

namespace PalaSoliisi.UI
{
    public partial class ScoreUIControl : Node
    {
        [Export] private Label _scoreLabel = null;

        /// <summary>
        /// Set amount of found clues in UI label
        /// </summary>
        /// <param name="score">How many clues currently found</param>
        public void SetScore(int score)
        {
            _scoreLabel.Text = $"Clues found: {score}/3";
        }
    }
}
