using Godot;
using System;

namespace PalaSoliisi
{
	public partial class Bear : Node2D
	{
        private Vector2 targetPosition; // Tavoite, johon halutaan liikkua
        private float speed = 200f; // Nopeus, jolla hahmo liikkuu kohti kohdetta

        public override void _Ready()
        {
            targetPosition = GlobalPosition; // Aluksi tavoite on hahmon nykyinen sijainti
        }

        public override void _Process(double delta)
        {
            // Liikkuminen kohti targetPositionia
            Vector2 direction = targetPosition - GlobalPosition;
            if (direction.Length() > 1) // Jos etäisyys kohteeseen on suurempi kuin 1, liikutaan
            {
                direction = direction.Normalized(); // Normalisoi suunta
                GlobalPosition += direction * speed * (float)delta; // Liikutetaan hahmoa kohti kohdetta
            }
        }

        // Input-eventit (hiiren klikkaus)
        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseButton mouseEvent
                && mouseEvent.ButtonIndex == MouseButton.Left
                && !mouseEvent.Pressed)
            {
                targetPosition = GetGlobalMousePosition();
                GD.Print($"Hiiren klikkaus sijainnissa: {targetPosition}");
            }
        }

	}
}