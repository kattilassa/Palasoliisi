using Godot;
using System;
using System.Collections.Generic;

namespace PalaSoliisi
{
	public partial class MiniGame : Node2D
	{
		[Export] private string _card1ScenePath = "res://Levels/Collectables/Card1.tscn";
		[Export] private string _card2ScenePath = "res://Levels/Collectables/Card2.tscn";
		[Export] private string _cardBack1ScenePath = "res://Levels/Collectables/CardBack1.tscn";

		private PackedScene _card1Scene = null;
		private PackedScene _card2Scene = null;
		private PackedScene _cardBack1Scene = null;

		private static MiniGame _current = null;
		private Grid _grid = null;
		private Card1 _card1 = null;
		private Card1 _card2 = null;
		private CardBack _cardBack1 = null;

		private List<Card> _placedCards = new List<Card>();
		private List<CardBack> _placedCardBacks = new List<CardBack>();

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
			// Vapautetaan aiemmin asetetut kortit
			foreach (var card in _placedCards)
			{
				Grid.ReleaseCell(card.GridPosition);
				card.QueueFree();
			}
			_placedCards.Clear();

			// Ladataan korttien scenet, jos niitä ei ole jo ladattu
			Dictionary<string, PackedScene> cardScenes = new Dictionary<string, PackedScene>
			{
				{ "card1", _card1Scene ?? ResourceLoader.Load<PackedScene>(_card1ScenePath) },
				{ "card2", _card2Scene ?? ResourceLoader.Load<PackedScene>(_card2ScenePath) }
			};

			foreach (var key in cardScenes.Keys)
			{
				if (cardScenes[key] == null)
				{
					GD.PrintErr($"Can't load {key} scene!");
					return;
				}
			}

			// Luodaan ja asetetaan kortit
			foreach (var scene in cardScenes.Values)
			{
				for (int i = 0; i < 2; i++) // Jokaisesta korttityypistä 2 kappaletta
				{
					Card card = scene.Instantiate<Card>();
					AddChild(card);

					Cell freeCell = Grid.GetRandomFreeCell();
					if (Grid.OccupyCell(card, freeCell.GridPosition))
					{
						card.SetPosition(freeCell.GridPosition);
						_placedCards.Add(card);
					}
				}
			}

			CoverCards();
		}

		private void CoverCards()
		{
			// TODO: CardBack asettettujen korttien päälle
			// Vapautetaan aiemmat CardBackit
			foreach (var cardBack in _placedCardBacks)
			{
				cardBack.QueueFree();
			}
			_placedCardBacks.Clear();

			// Ladataan CardBackin Scene, jos sitä ei ole vielä ladattu
			if (_cardBack1Scene == null)
			{
				_cardBack1Scene = ResourceLoader.Load<PackedScene>(_cardBack1ScenePath);
				if (_cardBack1Scene == null)
				{
					GD.PrintErr("Can't load CardBack scene!");
					return;
				}
			}

			// Käydään läpi vain asetetut kortit ja peitetään ne CardBackilla
			foreach (var card in _placedCards)
			{
				CardBack cardBack = _cardBack1Scene.Instantiate<CardBack>();
				AddChild(cardBack);

				// Asetetaan CardBack samalle paikalle kuin kortti
				cardBack.SetPosition(card.GridPosition);
				_placedCardBacks.Add(cardBack);
			}
		}
	}
}
