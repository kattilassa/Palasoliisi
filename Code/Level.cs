using Godot;
using PalaSoliisi.UI;
using System;

namespace PalaSoliisi
{
	public partial class Level : Node2D
	{
		[Export] private TextureButton _settingsButton = null;
		[Export] private TextureButton _articleButton = null;
		[Export] private TextureButton _computerButton = null;
		[Export] private CharacterBody2D _bear = null;
		[Export] private ScoreUIControl _scoreUIControl = null;
		[Export] private string _articleScenePath = "res://Levels/Collectables/Article.tscn";
		[Export] private string _miniGameScenePath = "res://Levels/MiniGame.tscn";

		private PackedScene _articleScene = null;
		private PackedScene _obstacleScene = null;
		private PackedScene _dialogueScene = null;
		private PackedScene _miniGameScene = null;

		private static Level _current = null;
		private Article _article = null;
		private Obstacle _obstacle = null;
		private ProtoDialog _dialogue = null;
		private MiniGame _miniGame = null;
		private Grid _grid = null;

		private Control _inGameMenu;
		private Control _UI;

		public bool _showInGameMenu = false;
		public bool _showDialogue = false;
		public bool _settingsClose = false;
		public bool _UIpressed = false;

		private int _articlePieces = 0;
		private int _miniGamesPlayed = 0;

		public int ArticlePieces
		{
			get { return _articlePieces; }
			set
			{
				if (value < 0)
				{
					_articlePieces = 0;
				}
				else
				{
					_articlePieces = value;
				}

				if (_scoreUIControl != null)
				{
					_scoreUIControl.SetScore(_articlePieces);
				}
			}
		}
		public int MiniGamesPlayed
		{
			get { return _miniGamesPlayed; }
			set
			{
				if (value < 0)
				{
					_miniGamesPlayed = 0;
				}
				else
				{
					_miniGamesPlayed = value;
				}

				//if (_scoreUIControl != null)
				//{
				//	_scoreUIControl.SetScore(_articlePieces);
				//}
			}
		}

		public static Level Current
		{
			get { return _current; }
		}
		public Grid Grid
		{
			get { return _grid; }
		}
		public Article Article
		{
			get { return _article; }
		}
		public ProtoDialog Dialog
		{
			get { return _dialogue; }
		}
		  public CellOccupierType Obstacle
		{
			get { return CellOccupierType.Obstacle; }
		}

		// Constructor
		public Level()
		{
			_current = this;
		}

		public override void _Ready()
		{
			_UI = GetNode<Control>("UI");
			_inGameMenu = GetNode<Control>("UI/InGameMenu");
			_inGameMenu.Hide();
			//_grid = GetNode<Grid>("Grid");
			//if (_grid == null)
			//{
			//	GD.PrintErr("Gridiä ei löytynyt Levelin lapsinodeista!");
			//}
			_settingsButton.Connect("gui_input",
				new Callable(this, nameof(OnSettingsGuiInput)));

			_articleButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnArticlePressed)));

			_computerButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnComputerPressed)));

			ResetGame();
		}

		public void ResetGame()
		{
			ArticlePieces = 0;
			Dialogue();
		}

		public override void _Process(double delta)
		{
		}

		private void OnSettingsPressed()
		{
			_UIpressed = true;
			if (_showInGameMenu)
			{
				GetTree().Paused = false;
				_showInGameMenu = false;
				_inGameMenu.Hide();
			}
			else
			{
				GetTree().Paused = true;
				_showInGameMenu = true;
				_inGameMenu.Show();
			}

		}

		private void OnArticlePressed()
		{
			ArticlePieces++;

			if(!_showInGameMenu)
			{
				_scoreUIControl.SetScore(_articlePieces);
			}
			else
			{
				return;
			}
			if(_articlePieces == 1)
			{
				_articleButton.GlobalPosition = new Vector2(100, 100);
			}
			else if (_articlePieces == 2)
			{
				_articleButton.GlobalPosition = new Vector2(10, 10);
			}
			else if (_articlePieces == 3)
			{
				_articleButton.GlobalPosition = new Vector2(50, 50);
				_articleButton.Hide();
			}

			// Start minigame when article collected
			StartMiniGame();
		}

		private void OnComputerPressed()
		{
			if(!_showInGameMenu)
			{
				GD.Print("Tietokonetta painettu");
			}
		}

		public void OnSettingsGuiInput(InputEvent @event)
		{
			if (@event is InputEventScreenTouch touchEvent && touchEvent.Pressed)
			{
				OnSettingsPressed();
			}
		}

		/// <summary>
		/// Toggle UI visibility
		/// </summary>
		/// <param name="visible">True if UI visible</param>
		private void UIVisible(bool visible)
		{
			_UI.Visible = visible;
		}

		public void Dialogue()
		{
			var dialogueBox = GetNode<Control>("DialogueBox");
			dialogueBox.Call("start");
			var isRunning = (bool)dialogueBox.Call("is_running");
			 if (isRunning)
			{     _showDialogue = false;
			}
			else
			{
				_showDialogue = true;
			}

			return;
		}

		public void CreateArticles()
		{
			if (_article != null)
			{
				_article.QueueFree();
				_article = null;
			}

			if (_articleScene == null)
			{
				_articleScene = ResourceLoader.Load<PackedScene>(_articleScenePath);
				if (_articleScene == null)
				{
					GD.PrintErr("Articles can't be found");
					return;
				}
			}
			_article = _articleScene.Instantiate<Article>();
			AddChild(_article);

			Cell freeCell = Grid.GetRandomFreeCell();
			if (Grid.OccupyCell(_article, freeCell.GridPosition))
			{
				_article.SetPosition(freeCell.GridPosition);
			}
		}

		/// <summary>
		/// Opens MiniGame scene when article collected
		/// </summary>
		public void StartMiniGame()
		{
			// Pause game and hide UI and character
			GetTree().Paused = true;
			_bear.Hide();
			_settingsButton.Hide();
			UIVisible(false);

			// Delete previous minigame
			if (_miniGame != null)
			{
				_miniGame.QueueFree();
				_miniGame = null;
			}

			if (_miniGameScene == null)
			{
				//Initialize new minigame
				_miniGameScene = ResourceLoader.Load<PackedScene>(_miniGameScenePath);
				if (_miniGameScene == null)
				{
					GD.PrintErr("MiniGame can't be found");
					return;
				}
			}
			_miniGame = _miniGameScene.Instantiate<MiniGame>();
			AddChild(_miniGame);

			// When minigame completed close minigame
			_miniGame.Connect(nameof(MiniGame.MiniGameCompleted),
			new Callable(this, nameof(OnMiniGameCompleted)));
		}

		/// <summary>
		/// Close minigame when completed.
		/// </summary>
		public async void OnMiniGameCompleted()
		{
			MiniGamesPlayed++;

			// Wait 1 sec before closing minigame window
			await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
			// Close minigame
			_miniGame.QueueFree();
			_miniGame = null;

			// Unpause game and set UI and character back to visible
			GetTree().Paused = false;
			_bear.Show();
			_settingsButton.Show();
			UIVisible(true);
		}
	}
}