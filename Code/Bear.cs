using Godot;
using System;

namespace PalaSoliisi
{
	public partial class Bear : Node2D
	{
        [Export] private float _speed = 50; // Nopeus, jolla hahmo liikkuu kohti kohdetta
        [Export] private Vector2I _startPosition = new Vector2I(5 ,5);

        private Vector2 _targetPosition; // Tavoite, johon halutaan liikkua

        public CellOccupierType Type
		{
			get { return CellOccupierType.Bear; }
		}

        public override void _Ready()
        {
            if (Level.Current.Grid.GetWorldPosition(_startPosition, out Vector2 worldPosition))
			{
				Position = worldPosition;
                _targetPosition = worldPosition;
			}
        }

        public override void _Process(double delta)
        {
                Move(delta);
        }

        // Input-eventit (hiiren klikkaus)
        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseButton mouseEvent
                && mouseEvent.ButtonIndex == MouseButton.Left
                && !mouseEvent.Pressed)
            {
                Vector2 clickPos = GetGlobalMousePosition();

                if (Level.Current.Grid.IsCellClicked(clickPos, out Vector2I gridCoord))
                {
                    GD.Print($"Solua klikattu: {gridCoord}");
                        // Suorita halutut toimenpiteet klikatulle solulle
                    if (Level.Current.Grid.GetWorldPosition(gridCoord, out Vector2 cellCenter))
                    {
                        _targetPosition = cellCenter;
                    }
                }
                else
                {
                    GD.Print("Klikkaus ei osunut soluun.");
                }
            }
        }

        public void Move(double delta)
        {
                // Liikkeen laskenta suoraan _targetPositionia kohti
            Vector2 direction = _targetPosition - GlobalPosition;
            float distance = direction.Length();

            if (distance > 1) // Jos etäisyys on tarpeeksi suuri, liikutaan
            {
                direction = direction.Normalized();
                GlobalPosition += direction * (float)delta * _speed;
            }
            else
            {
                // Kun ollaan lähellä kohdetta, asetetaan sijainti ruudukon mukaiseksi
                // Muutetaan _targetPosition grid koordinaateiksi (tässä oletetaan, että
                // hiiren klikkaus tuottaa ruudun keskikoordinaatin, mutta jos ei, niin muunnetaan)
                Vector2I gridPos = (Vector2I)_targetPosition;
                if (Level.Current.Grid.GetWorldPosition(gridPos, out Vector2 worldPosition))
                {
                    GlobalPosition = worldPosition;
                }
            }
        }

	}
}