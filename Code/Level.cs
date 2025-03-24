using Godot;
using PalaSoliisi.UI;
using System;
using System.Threading.Tasks;

namespace PalaSoliisi
{
	public partial class Level : Node2D
	{
		private static Level _current = null;
		private SceneTree _inGameSceneTree = null;
		public bool _showInGameMenu = false;
		public bool _showDialogue = false;
		public bool _settingsClose = false;
		public bool _UIpressed = false;
		public bool _isMiniGameRunning = false;
		public bool _isDialogueRunning = false;
		private Control _UI;
		private Control _inGameMenu;
		[Export] private TextureButton _settingsButton = null;
		[Export] private TextureButton _articleButton = null;
		[Export] private TextureButton _computerButton = null;
		[Export] private TextureButton _phoneButton = null;
		[Export] private Button _menuButton = null;
		[Export] private string _miniGameScenePath = "res://Levels/MiniGame.tscn";
		private SceneTree _optionsSceneTree = null;
		private Bernand _bernand = null;
		public bool isRunning;
		public bool callAnswered = false;
		public static Level Current
		{
			get { return _current; }
		}
		[Export] private string _articleScenePath = "res://Levels/Collectables/Article.tscn";
		[Export] private ScoreUIControl _scoreUIControl = null;

        private Grid _grid = null;

		private PackedScene _articleScene = null;
		private PackedScene _obstacleScene = null;
		private PackedScene _dialogueScene = null;
		private PackedScene _miniGameScene = null;
		private MiniGame _miniGame = null;
		private int _articlePieces = 0;
		private int _miniGamesPlayed = 0;

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
			}
		}

		public Grid Grid
		{
			get { return _grid; }
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
			base._Ready();
			_inGameSceneTree = GetTree();
			_menuButton.Hide();
			_UI = GetNode<Control>("UI");
			_inGameMenu = GetNode<Control>("UI/InGameMenu");
			_dialogueBox = GetNode("DialogueBox");
			_dialogueBubble = GetNode("DialogueBubble");
			_inGameMenu.Hide();
			_bernand = GetNode<Bernand>("Camera2D/Bernand");
			 _dialogueBox.Connect("dialogue_ended", new Callable(this, nameof(OnDialogueEnded)));
			 _dialogueBox.Connect("dialogue_started", new Callable(this, nameof(OnDialogueStarted)));
			  _dialogueBubble.Connect("dialogue_started", new Callable(this, nameof(OnDialogueBubbleStarted)));
			_menuButton.Connect(Button.SignalName.Pressed, new Callable(this, nameof(OnMenuPressed)));
				_settingsButton.Connect("gui_input", new Callable(this, nameof(OnSettingsGuiInput)));

				_articleButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnArticlePressed)));

			_computerButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnComputerPressed)));

				_phoneButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnPhonePressed)));
			ResetGame();
		}
		public void ResetGame()
		{
			ArticlePieces= 0;
			_articleButton.Hide();
			Dialogue();
		}
		public void OnSettingsPressed()
		{
			_UIpressed = true;
			if (_showInGameMenu)
			{
				GetTree().Paused = false;
				_showInGameMenu = false;
				_inGameMenu.Hide();
				_menuButton.Hide();
				Movement();
			}
			else
			{
				GetTree().Paused = true;
				_showInGameMenu = true;
				_inGameMenu.Show();
				_menuButton.Show();
				Movement();
			}

		}
		private async void OnArticlePressed()
		{
			bool firstButton = true;
			bool secondButton = true;
			bool thirdButton = true;
			await timerAsync(1);
			//if(!_showInGameMenu)
			//{
			//ArticlePieces++;
			//_scoreUIControl.SetScore(_articlePieces);
			//}
			//else
			//{
			//	return;
			//}
			if(_articlePieces == 0 && firstButton)
			{
				firstButton=false;
				_articleButton.GlobalPosition = new Vector2(-67, -32);
			}
			else if (_articlePieces == 1 && secondButton)
			{
				secondButton=false;
				_articleButton.GlobalPosition = new Vector2(-107, 65);
			}
			else if (_articlePieces >= 2 && thirdButton)
			{
				thirdButton=false;
				_articleButton.Hide();
				string startId = "computer";
				_dialogueBox.Call("start", startId);
			}

			// Start minigame when article collected
			StartMiniGame();

		}

		/// <summary>
		/// Toggle UI visibility
		/// </summary>
		/// <param name="visible">True if UI visible</param>
		private void UIVisible(bool visible)
		{
			_UI.Visible = visible;
		}

		private void OnComputerPressed()
		{
			if(!_showInGameMenu && !_isDialogueRunning)
			{
				GD.Print("Tietokonetta painettu");

				if (_articlePieces==3)
				{
				string startId = "quiz";
				_dialogueBox.Call("start", startId);
				}
				else if (!_isDialogueRunning)
				{
				Random rnd = new Random();
				int randomDialogue = rnd.Next(1, 6);
				String startID = $"{randomDialogue}";

				switch (randomDialogue)
				{
					case 1:
					_dialogueBubble.Call("start", $"{randomDialogue}");
					break;
					case 2:
					_dialogueBubble.Call("start", $"{randomDialogue}");
					break;
					case 3:
					_dialogueBubble.Call("start", $"{randomDialogue}");
					break;
					case 4:
					_dialogueBubble.Call("start", $"{randomDialogue}");
					break;
					case 5:
					_dialogueBubble.Call("start", $"{randomDialogue}");
					break;
				}
			}
			}
		}

		private async Task timerAsync(float time)
		{
		 await ToSignal(GetTree().CreateTimer(time), "timeout");
		}
		private async void Movement()
		{
		await timerAsync(1);
		 _UIpressed=false;
		}
		public void OnDialogueEnded()
		{
				_isDialogueRunning = false;
		}
		public async void OnDialogueBubbleEnded()
		{
			    await timerAsync(1);
		  		_dialogueBubble.Call("_on_dialogue_ended");
				_isDialogueRunning = false;
		}
		public async void OnDialogueBubbleStarted(string id)
		{
				_isDialogueRunning = true;
			    await timerAsync(1);
		  		_dialogueBubble.Call("_on_dialogue_ended");
				_isDialogueRunning = false;
		}
		private void OnDialogueStarted(string id)
		{
			_isDialogueRunning = true;
		}
			private async void OnPhonePressed()
		{
			if(!_showInGameMenu & _articlePieces==0 & !callAnswered)
			{
				callAnswered=true;
				await timerAsync(1);
				_dialogueBox.Call("start", "phone");
				_articleButton.Show();
			}
			else if (callAnswered)
			{
				string startId = "first";
				_dialogueBubble.Call("start", startId);
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
		public void OnMenuPressed()
		{
			_inGameSceneTree.ChangeSceneToFile("res://Code/UI/MainMenu.tscn");
			GetTree().Paused = false;
		}
		public void Dialogue()
		{
			string startId = (string)_dialogueBox.Get("start_id");
			_dialogueBox.Call("start", startId);
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
			UIVisible(false);
			_bernand.Hide();
			_bernand.StopMovement();
			_settingsButton.Hide();
			_isMiniGameRunning = true;

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
			ArticlePieces++;
			_scoreUIControl.SetScore(_articlePieces);
			// Wait 1 sec before closing minigame window
			await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
			// Close minigame
			_miniGame.QueueFree();
			_miniGame = null;

			// Unpause game and set UI and character back to visible
			GetTree().Paused = false;
			UIVisible(true);
			_bernand.Show();
			_bernand.StartMovement();
			_settingsButton.Show();
			_isMiniGameRunning = false;
		}
	}
}