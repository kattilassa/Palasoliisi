using Godot;
using PalaSoliisi.UI;
using System;

namespace PalaSoliisi
{
	public partial class Level : Node2D
	{
		private static Level _current = null;
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
		private ProtoDialog _dialogue = null;

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
		public ProtoDialog Dialog
		{
			get { return _dialogue; }
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
			_inGameMenu = GetNode<Control>("InGameMenu");
			_inGameMenu.Hide();
			_grid = GetNode<Grid>("Grid");
			if (_grid == null)
			{
				GD.PrintErr("Gridiä ei löytynyt Levelin lapsinodeista!");
			}
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
			else
			{
				_articleButton.GlobalPosition = new Vector2(50, 50);
			}
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
	}
}