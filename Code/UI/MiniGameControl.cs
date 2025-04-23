using Godot;
using System;

namespace PalaSoliisi.UI
{
    public partial class MiniGameControl : Node
    {
        [Export] private Label _pairsScore = null;
        [Export] private Label _turnsScore = null;

        /// <summary>
        /// Set parr score in UI label
        /// </summary>
        /// <param name="score">How manu pair found</param>
        public void SetPairsScore(int score)
        {
            _pairsScore.Text = $"Pairs found: {score}";
        }

        /// <summary>
        /// Set turns score in UI label
        /// </summary>
        /// <param name="score">How many turns used</param>
        public void SetTurnsScore(int score)
        {
            _turnsScore.Text = $"Turns taken: {score}";
        }
    }
}
