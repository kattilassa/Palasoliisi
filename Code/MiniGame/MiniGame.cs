using Godot;
using System;

namespace PalaSoliisi
{
	public partial class MiniGame : Node2D
	{
		[Export] private string _card1ScenePath = "res://Levels/Collectables/Card1.tscn";

		private PackedScene _card1Scene = null;

		private static MiniGame _current = null;
		private Grid _grid = null;
		private Card1 _card1 = null;

		public static MiniGame Current
		{
			get { return _current; }
		}

		public MiniGame()
		{
			_current = this;
		}
		public Grid Grid
		{
			get { return _grid; }
		}

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			_grid = GetNode<Grid>("Grid");
			if (_grid == null)
			{
				GD.PrintErr("Gridiä ei löytynyt Levelin lapsinodeista!");
			}

			PlaceCards();
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

		private void PlaceCards()
		{
			// TODO: Korttien asettaminen gridille
			if (_card1 != null)
			{
				Grid.ReleaseCell(_card1.GridPosition);

				_card1.QueueFree();
				_card1 = null;
			}

			if (_card1Scene == null)
			{
				_card1Scene = ResourceLoader.Load<PackedScene>(_card1ScenePath);
				if (_card1Scene == null)
				{
					GD.PrintErr("Can't load apple scene!");
					return;
				}
			}

			for (int i = 0 ; i < 2 ; i++)
			{
				_card1 = _card1Scene.Instantiate<Card1>();
				AddChild(_card1);

				Cell freeCell = Grid.GetRandomFreeCell();
				if (Grid.OccupyCell(_card1, freeCell.GridPosition))
				{
					_card1.SetPosition(freeCell.GridPosition);
				}
			}
		}
	}
}
