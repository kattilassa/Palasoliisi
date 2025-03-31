using Godot;
using System;

namespace PalaSoliisi
{
    public partial class Ending : Node
    {
        [Export] private Label _testScore = null;
        [Export] private Label _miniGameScore = null;

        public void SetTestScore(int score)
        {
            _testScore.Text = $"Test score: {score}";
        }

        public void SetMiniGameScore(int score)
        {
            _miniGameScore.Text = $"MiniGame turns used: {score}";
        }
    }
}
