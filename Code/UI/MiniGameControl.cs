using Godot;
using System;

namespace PalaSoliisi.UI
{
    public partial class MiniGameControl : Node
    {
        [Export] private Label _pairsScore = null;
        [Export] private Label _turnsScore = null;

        public void SetPairsScore(int score)
        {
            _pairsScore.Text = $"Pairs found: {score}";
        }

        public void SetTurnsScore(int score)
        {
            _turnsScore.Text = $"Turns taken: {score}";
        }
    }
}
