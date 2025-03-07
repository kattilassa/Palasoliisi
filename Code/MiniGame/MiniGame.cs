using Godot;
using System;
using System.Collections.Generic;
using PalaSoliisi.UI;

namespace PalaSoliisi
{
	public partial class MiniGame : Level
	{
		[Export] private string _card1ScenePath = "res://Levels/Collectables/Card1.tscn";
		[Export] private string _card2ScenePath = "res://Levels/Collectables/Card2.tscn";
		[Export] private string _card3ScenePath = "res://Levels/Collectables/Card3.tscn";
		[Export] private string _card4ScenePath = "res://Levels/Collectables/Card4.tscn";
		[Export] private string _cardBack1ScenePath = "res://Levels/Collectables/CardBack1.tscn";
		[Export] private MiniGameControl _miniGameControl = null;

		private PackedScene _card1Scene = null;
		private PackedScene _card2Scene = null;
		private PackedScene _card3Scene = null;
		private PackedScene _card4Scene = null;
		private PackedScene _cardBack1Scene = null;

		private static MiniGame _current = null;
		private Grid _grid = null;
		private int _pairsFound = 0;
		private int _turnsTaken = 0;

		private Card1 _card1 = null;
		private Card2 _card2 = null;
		private Card3 _card3 = null;
		private Card4 _card4 = null;
		private CardBack _cardBack1 = null;

		private List<Card> _placedCards = new List<Card>();
		private List<CardBack> _placedCardBacks = new List<CardBack>();
		private List<Card> _turnedCards = new List<Card>();
		private List<CardBack> _turnedCardBacks = new List<CardBack>();

		public MiniGame()
		{
			_current = this;
		}

		public int PairsFound
		{
			get { return _pairsFound; }
			set
			{
				if (value < 0)
				{
					_pairsFound = 0;
				}
				else
				{
					_pairsFound = value;
				}

				if (_miniGameControl != null)
				{
					_miniGameControl.SetPairsScore(_pairsFound);
				}
			}
		}
		public int TurnsTaken
		{
			get { return _turnsTaken; }
			set
			{
				if (value < 0)
				{
					_turnsTaken = 0;
				}
				else
				{
					_turnsTaken = value;
				}

				if (_miniGameControl != null)
				{
					_miniGameControl.SetTurnsScore(_turnsTaken);
				}
			}
		}

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			base._Ready(); // Kutsutaan Levelin alustusta

        	_grid = GetNode<Grid>("Grid");
			if (_grid == null)
			{
				GD.PrintErr("Gridiä ei löytynyt MiniGamein lapsinodeista!");
			}

			PairsFound = 0;
			TurnsTaken = 0;

			PlaceCards();
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

		public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseButton mouseEvent
                && mouseEvent.ButtonIndex == MouseButton.Left
                && !mouseEvent.Pressed)
            {
                Vector2 clickPos = GetGlobalMousePosition();

                if (Grid.IsCellClicked(clickPos, out Vector2I gridCoord))
                {
                    GD.Print($"Solua klikattu: {gridCoord}");

					// Etsitään kortti takapuoli, joka on tässä solussa
					CardBack clickedCardBack = _placedCardBacks.Find(cb => cb.GridPosition == gridCoord);

					if (clickedCardBack != null)
					{
						clickedCardBack.Turn(); // Käännä kortti

						Card turnedCard = _placedCards.Find(c => c.GridPosition == gridCoord);
						CardBack turnedCardBack = _placedCardBacks.Find(cb => cb.GridPosition == gridCoord);

						if (turnedCard != null && turnedCardBack != null)
						{
							_turnedCards.Add(turnedCard); // Lisää käännettyjen korttien listaan
							_turnedCardBacks.Add(turnedCardBack);

							if (_turnedCards.Count == 2) // Kun kaksi korttia on käännetty
							{
								CheckPair();
							}
						}
					}
					else
					{
						GD.Print("Klikattu solu ei sisällä CardBackia.");
					}
                }
                else
                {
                    GD.Print("Klikkaus ei osunut soluun.");
                }
            }
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
				{ "card2", _card2Scene ?? ResourceLoader.Load<PackedScene>(_card2ScenePath) },
				{ "card3", _card3Scene ?? ResourceLoader.Load<PackedScene>(_card3ScenePath) },
				{ "card4", _card4Scene ?? ResourceLoader.Load<PackedScene>(_card4ScenePath) }
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

		private async void CheckPair()
		{
			TurnsTaken++;

			if (_turnedCards.Count != 2)
				return;

			Card card1 = _turnedCards[0];
			Card card2 = _turnedCards[1];

			if (card1.GetType() == card2.GetType()) // Jos kortit ovat samaa tyyppiä
			{
				GD.Print("Pari löytyi!");
				PairsFound++;
			}
			else
			{
				GD.Print("Ei pari, käännetään takaisin.");
				// ?? 1 sek viive ??
				await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
				TurnPair();
			}

			_turnedCards.Clear(); // Tyhjennä lista seuraavaa paria varten
			_turnedCardBacks.Clear();
		}

		private void TurnPair()
		{
			if (_turnedCardBacks.Count != 2)
				return;

			for (int i = 0 ; i < _turnedCardBacks.Count ; i++)
			{
				_turnedCardBacks[i].Cover();
			}

			_turnedCardBacks.Clear(); // Tyhjennä lista seuraavaa paria varten
		}
	}
}
