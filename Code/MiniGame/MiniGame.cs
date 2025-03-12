using Godot;
using System;
using System.Collections.Generic;
using PalaSoliisi.UI;

namespace PalaSoliisi
{
	/// <summary>
	/// Memory card game.
	/// </summary>
	public partial class MiniGame : Node2D
	{
		// Game completed
		[Signal]
		public delegate void MiniGameCompletedEventHandler();

		[Export] private string _card1ScenePath = "res://Levels/Collectables/Card1.tscn";
		[Export] private string _card2ScenePath = "res://Levels/Collectables/Card2.tscn";
		[Export] private string _card3ScenePath = "res://Levels/Collectables/Card3.tscn";
		[Export] private string _card4ScenePath = "res://Levels/Collectables/Card4.tscn";
		[Export] private string _cardBack1ScenePath = "res://Levels/Collectables/CardBack1.tscn";
		[Export] private MiniGameControl _miniGameControl = null;
		[Export] private TextureButton _settingsButton = null;

		private PackedScene _card1Scene = null;
		private PackedScene _card2Scene = null;
		private PackedScene _card3Scene = null;
		private PackedScene _card4Scene = null;
		private PackedScene _cardBack1Scene = null;

		private static MiniGame _current = null;
		private Grid _grid = null;
		private Control _inGameMenu;
		public bool _showInGameMenu = false;
		private bool _settingsClose = false;

		// Score of matching pairs found
		private int _pairsFound = 0;
		// Score of turns taken
		private int _turnsTaken = 0;

		private Card1 _card1 = null;
		private Card2 _card2 = null;
		private Card3 _card3 = null;
		private Card4 _card4 = null;
		private CardBack _cardBack1 = null;

		// List of cards placed in the grid
		private List<Card> _placedCards = new List<Card>();
		private List<CardBack> _placedCardBacks = new List<CardBack>();

		// List of cards that are flipped within a turn
		private List<Card> _turnedCards = new List<Card>();
		private List<CardBack> _turnedCardBacks = new List<CardBack>();
		private List<Card> _matchedCards = new List<Card>();

		public static MiniGame Current
		{
			get { return _current; }
		}
		public Grid Grid
		{
			get { return _grid; }
		}

		public MiniGame()
		{
			_current = this;
		}

		// Set score to UI
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

					// Send signal when all pairs have been found
					if (_pairsFound == 4)
            		{
        	        	EmitSignal(nameof(MiniGameCompleted));
        	    	}
				}

				if (_miniGameControl != null)
				{
					_miniGameControl.SetPairsScore(_pairsFound);
				}
			}
		}
		// Set score to UI
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

		public override void _Ready()
		{
			_inGameMenu = GetNode<Control>("UI/InGameMenu");
			_inGameMenu.Hide();

			// Initializes Grid
			_grid = GetNode<Grid>("Grid");
			if (_grid == null)
			{
				GD.PrintErr("Grid could not be found in nodetree!");
			}

			_settingsButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnSettingsPressed)));

			ResetMiniGame();

		}

		public override void _Input(InputEvent @event)
        {
        	if (@event is InputEventScreenTouch touchEvent
			&& touchEvent.Index == 0
			&& touchEvent.Pressed)
            {
                Vector2 clickPos = GetViewport().GetCanvasTransform().AffineInverse() * touchEvent.Position;

                if (Grid.IsCellClicked(clickPos, out Vector2I gridCoord))
                {
                    GD.Print($"Clicked cell: {gridCoord}");

					// Finds CardBack that is located in the clicked cell
					CardBack clickedCardBack = _placedCardBacks.Find(cb => cb.GridPosition == gridCoord);

					if (_turnedCardBacks.Count < 2)
					{
						if (clickedCardBack != null)
						{
							// Turn the clicked card
							clickedCardBack.Turn();

							// Finds Card that is located in the clicked cell
							Card turnedCard = _placedCards.Find(c => c.GridPosition == gridCoord);

							if(!_turnedCards.Contains(turnedCard) && !_matchedCards.Contains(turnedCard))
							{
								if (turnedCard != null && clickedCardBack != null)
								{
									// Add to the list of turned cards
									_turnedCards.Add(turnedCard);
									_turnedCardBacks.Add(clickedCardBack);

									// When 2 cards gave been clicked check if matching pair
									if (_turnedCards.Count == 2)
									{
										CheckPair();
									}
								}
							}
							else
							{
								GD.Print("Already turned card");
							}
						}
						else
						{
							GD.Print("Clickes cell does not have CardBack");
						}
					}
					else
					{
						GD.Print("Too many cards turned");
					}
                }
                else
                {
                    GD.Print("Clicked outside of grid boundaries");
                }
            }
        }

		/// <summary>
		/// Pauses game and shows InGameMenu when settings button pressed.
		/// </summary>
		private void OnSettingsPressed()
		{
			// If game already paused, continues game
			if (_showInGameMenu)
			{
				GetTree().Paused = false;
				_showInGameMenu = false;
				_inGameMenu.Hide();
			}
			// Pause game
			else
			{
				GetTree().Paused = true;
				_showInGameMenu = true;
				_inGameMenu.Show();
			}

		}

		/// <summary>
		/// Reset MiniGame. Resets score for found pairs and taken turns. Places new cards
		/// on grid.
		/// </summary>
		private void ResetMiniGame()
		{
			GetTree().Paused = false;

			// Reset scores
			PairsFound = 0;
			TurnsTaken = 0;

			_turnedCards.Clear();
			_turnedCardBacks.Clear();

			// Place new cards
			PlaceCards();
		}

		/// <summary>
		/// Places new cards on grid in random order
		/// </summary>
		private void PlaceCards()
		{
			// Release previously placed cards
			foreach (var card in _placedCards)
			{
				Grid.ReleaseCell(card.GridPosition);
				card.QueueFree();
			}
			_placedCards.Clear();

			// Load Card scenes for different types of cards
			Dictionary<string, PackedScene> cardScenes = new Dictionary<string, PackedScene>
			{
				{ "card1", _card1Scene ?? ResourceLoader.Load<PackedScene>(_card1ScenePath) },
				{ "card2", _card2Scene ?? ResourceLoader.Load<PackedScene>(_card2ScenePath) },
				{ "card3", _card3Scene ?? ResourceLoader.Load<PackedScene>(_card3ScenePath) },
				{ "card4", _card4Scene ?? ResourceLoader.Load<PackedScene>(_card4ScenePath) }
			};

			foreach (var key in cardScenes.Keys)
			{
				// Chack that all cardscenes can be loaded
				if (cardScenes[key] == null)
				{
					GD.PrintErr($"Can't load {key} scene!");
					return;
				}
			}

			// Create new cards and set random positions
			foreach (var scene in cardScenes.Values)
			{
				// 2 of each cardtype
				for (int i = 0; i < 2; i++)
				{
					// Initialize Card
					Card card = scene.Instantiate<Card>();
					AddChild(card);

					// Get random free position from grid and set position
					Cell freeCell = Grid.GetRandomFreeCell();

					if (Grid.OccupyCell(card, freeCell.GridPosition))
					{
						card.SetPosition(freeCell.GridPosition);
						// Add placed Card in a list to keep track of cards in grid
						_placedCards.Add(card);
					}
				}
			}

			// Cover cards with CardBack
			CoverCards();
		}

		/// <summary>
		/// Places CardBack on top of every Card in grid
		/// </summary>
		private void CoverCards()
		{
			// Release previously placed cards
			foreach (var cardBack in _placedCardBacks)
			{
				cardBack.QueueFree();
			}
			_placedCardBacks.Clear();

			// Load CardBack scene
			if (_cardBack1Scene == null)
			{
				_cardBack1Scene = ResourceLoader.Load<PackedScene>(_cardBack1ScenePath);
				if (_cardBack1Scene == null)
				{
					GD.PrintErr("Can't load CardBack scene!");
					return;
				}
			}

			// Check placed cards position and place CardBack on top of it
			foreach (var card in _placedCards)
			{
				// Initialize CardBack
				CardBack cardBack = _cardBack1Scene.Instantiate<CardBack>();
				AddChild(cardBack);

				// Set CardBack in the same position as Card
				cardBack.SetPosition(card.GridPosition);
				// Add placed CardBack in a list to keep track of cards in grid
				_placedCardBacks.Add(cardBack);
			}
		}

		/// <summary>
		/// Check if clicked pair matches
		/// </summary>
		private async void CheckPair()
		{
			// Execute method only when 2 cells have been clikced
			if (_turnedCards.Count != 2)
			{
				return;
			}
			else
			{
				Card card1 = _turnedCards[0];
				Card card2 = _turnedCards[1];

				// Check if matching pair
				if (card1.GetType() == card2.GetType())
				{
					GD.Print("Pair found!");
					// Keep score of number of pairs found
					PairsFound++;
					// Add pair to a list to keep track so they can't be chosen again
					_matchedCards.Add(card1);
					_matchedCards.Add(card2);
				}
				// If pair does not match cover cards again
				else
				{
					GD.Print("Pair not found.");
					// Wait for a second before covering cards
					await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
					// Covers only selected pair
					TurnPair();
				}

				// Keeps score of number of turns taken
				TurnsTaken++;

				// Clear lists for the next pair
				_turnedCards.Clear();
				_turnedCardBacks.Clear();
			}
		}

		/// <summary>
		/// Cover only the previously selected 2 cards if not matching
		/// </summary>
		private void TurnPair()
		{
			// Check if 2 cards turned
			if (_turnedCardBacks.Count != 2)
				return;

			// Cover cards in list
			for (int i = 0 ; i < _turnedCardBacks.Count ; i++)
			{
				_turnedCardBacks[i].Cover();
			}
		}
	}
}
