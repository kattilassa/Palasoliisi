using Godot;
using PalaSoliisi.UI;
using System;

namespace PalaSoliisi
{
	public partial class Level : Node2D
	{
		private static Level _current = null;
		private Timer Timer;
		public bool _showInGameMenu = false;

		public bool _showDialogue = false;
		public bool _settingsClose = false;
		public bool _UIpressed = false;
		private Control _inGameMenu;
		[Export] private TextureButton _settingsButton = null;
		[Export] private TextureButton _articleButton = null;
		[Export] private TextureButton _computerButton = null;

		public static Level Current
		{
			get { return _current; }
		}
		[Export] private string _articleScenePath = "res://Levels/Collectables/Article.tscn";
		[Export] private ScoreUIControl _scoreUIControl = null;

        private Grid _grid = null;
		private PackedScene _bearScene = null;
		private PackedScene _articleScene = null;
		private PackedScene _obstacleScene = null;
		private PackedScene _dialogueScene = null;
		private int _articlePieces = 0;
		private Bear _bear = null;
		private Article _article = null;
		private Obstacle _obstacle = null;
		private Node _dialogueBox;
		private Node _dialogueBubble;


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

		public Grid Grid
		{
			get { return _grid; }
		}
		public Bear Bear
		{
			get { return _bear; }
		}
		public Article Article
		{
			get { return _article; }
		}
		  public CellOccupierType Obstacle
		{
			get { return CellOccupierType.Obstacle; }
		}

		// Rakentaja. Käytetään alustamaan olio.
		public Level()
		{
			_current = this;
		}
		public override void _Ready()
		{
			Timer = GetNode<Timer>("Timer");
			_inGameMenu = GetNode<Control>("UI/InGameMenu");
			_dialogueBox = GetNode("DialogueBox");
			_dialogueBubble = GetNode("DialogueBubble");
			_inGameMenu.Hide();
			//_grid = GetNode<Grid>("Grid");
			//if (_grid == null)
			//{
			//	GD.PrintErr("Gridiä ei löytynyt Levelin lapsinodeista!");
			//}
				_settingsButton.Connect("gui_input", new Callable(this, nameof(OnSettingsGuiInput)));

				_articleButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnArticlePressed)));

			_computerButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnComputerPressed)));

			ResetGame();
		}
		public void ResetGame()
		{
			ArticlePieces= 0;
			Dialogue();

		}
		public override void _Process(double delta)
		{
		}

		public void OnSettingsPressed()
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
			if(!_showInGameMenu)
			{
			ArticlePieces++;
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

				string startId = (string)_dialogueBox.Get("start_id");
				startId = "second";
				_dialogueBubble.Call("start", startId);
			}

		}

		private void OnComputerPressed()
		{
			if(!_showInGameMenu)
			{
				GD.Print("Tietokonetta painettu");

				if (_articlePieces==3)
				{
				string startId = "quiz";
				_dialogueBox.Call("start", startId);
				}
			}
		}

			public void OnSettingsGuiInput(InputEvent @event)
		{
			if (@event is InputEventMouseButton mb)
			{
				if (mb.ButtonIndex == MouseButton.Left && mb.Pressed)
				{
					OnSettingsPressed();
				}
			}
		}
		public void Dialogue()
		{
			string startId = (string)_dialogueBox.Get("start_id");
			_dialogueBox.Call("start", startId);

			_dialogueBubble.Call("start");

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
	}
}