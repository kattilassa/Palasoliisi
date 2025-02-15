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
        targetPosition = Position; // Aluksi tavoite on hahmon nykyinen sijainti
    }

    public override void _Process(double delta)
    {
        // Liikkuminen kohti targetPositionia
        Vector2 direction = targetPosition - Position;
        if (direction.Length() > 1) // Jos etäisyys kohteeseen on suurempi kuin 1, liikutaan
        {
            direction = direction.Normalized(); // Normalisoi suunta
            Position += direction * speed * (float)delta; // Liikutetaan hahmoa kohti kohdetta
        }
    }

    // Input-eventit (hiiren klikkaus)
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
        {
            // Jos hiirellä klikataan, määritellään uusi tavoite
            targetPosition = mouseButton.Position;
        }
        else if (@event is InputEventScreenTouch touch && touch.Pressed)
        {
            // Jos kosketusnäytöllä klikataan, määritellään uusi tavoite
            targetPosition = touch.Position;
        }
    }

	}
}