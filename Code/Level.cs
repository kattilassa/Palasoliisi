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
		public bool _isGameFinished = false;
		public bool _isDialogueRunning = false;
		public bool _isQuizDone = false;
		[Export] public Label _clueLabel;
		private Control _UI;
		public Control _inGameMenu;
		[Export] private TextureButton _curtainButton = null;
		[Export] private TextureButton _settingsButton = null;
		[Export] private TextureButton _articleButton = null;
		[Export] private TextureButton _computerButton = null;
		[Export] private TextureButton _fridgeButton = null;
		[Export] private TextureButton _tableButton = null;
		[Export] private TextureButton _phoneButton = null;
		[Export] private TextureButton _bedButton = null;
		[Export] private TextureButton _frogButton = null;
		[Export] private TextureButton _boardButton = null;
		[Export] private TextureButton _closetButton = null;
		[Export] private TextureButton _briefcaseButton = null;
		[Export] private TextureButton _dinnerTableButton = null;
		[Export] private Button _exitClueButton = null;
		[Export] private string _miniGameScenePath = "res://Levels/MiniGame.tscn";
		//[Export] private string _howToPlayScenePath = "res://Levels/HowToPlay.tscn";
		[Export] private string _goodEndScenePath = "res://Levels/GoodEnd.tscn";
		[Export] private string _badEndScenePath = "res://Levels/BadEnd.tscn";
		private SceneTree _optionsSceneTree = null;
		private Bernand _bernand = null;
		public bool isRunning;
		public AnimationPlayer _animationPlayer;
		private AudioStreamPlayer _soundPlayer;
        private AudioStream _ringtoneSound;
        private AudioStream _paperSound;
        private AudioStream _callEndedSound;
        private AudioStream _callStartedSound;
        private AudioStream _dingSound;
        private AudioStream _clickSound;
        private AudioStream _cabinetSound;
        private AudioStream _bedSound;
        private AudioStream _backpackSound;
        public bool callAnswered = false;
		public bool callGloria= false;
		public bool clueCollected= false;
		public static Level Current
		{
			get { return _current; }
		}
		[Export] private string _articleScenePath = "res://Levels/Collectables/Article.tscn";
		[Export] private string _finalQuizScenePath = "res://Levels/FinalQuiz.tscn";
		[Export] private ScoreUIControl _scoreUIControl = null;
		[Export] AudioStreamPlayer _ringtonePlayer;

        private Grid _grid = null;

		private PackedScene _articleScene = null;
		private PackedScene _obstacleScene = null;
		private PackedScene _dialogueScene = null;
		private PackedScene _miniGameScene = null;
		//private PackedScene _howToPlayScene = null;
		private PackedScene _finalQuizScene = null;
		private PackedScene _endScene = null;
		private MiniGame _miniGame = null;
		public Control _howToPlay = null;
		private Ending _ending = null;
		private int _articlePieces = 0;
		private int _miniGameTurns = 0;
		private int _testPoints = 0;
		public FinalQuiz _finalQuiz = null;
		private Article _article = null;
		private Obstacle _obstacle = null;
		private Node _dialogueBox;
		private Node _dialogueBubble;
		public bool _isAnswered = true;
		public TextureRect _phoneEffect;
		public TextureRect _clueCard;


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

		public int MiniGameTurns
		{
			get { return _miniGameTurns; }
			set
			{
				if (value < 0)
				{
					_miniGameTurns = 0;
				}
				else
				{
					_miniGameTurns = value;
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
			GetTree().Paused = false;
			_inGameSceneTree = GetTree();
			_UI = GetNode<Control>("UI");
			_howToPlay= GetNode<Control>("UI/howToPlay");
			_inGameMenu = GetNode<Control>("UI/InGameMenu");
			_dialogueBox = GetNode("UI/DialogueBox");
			_dialogueBubble = GetNode("DialogueBubble");
			_inGameMenu.Hide();
			_bernand = GetNode<Bernand>("Camera2D/Bernand");
			 _dialogueBox.Connect("dialogue_ended", new Callable(this, nameof(OnDialogueEnded)));
			 _dialogueBox.Connect("dialogue_started", new Callable(this, nameof(OnDialogueStarted)));
			  _dialogueBubble.Connect("dialogue_started", new Callable(this, nameof(OnDialogueBubbleStarted)));
			_settingsButton.Connect("gui_input", new Callable(this, nameof(OnSettingsGuiInput)));
			_phoneEffect = GetNode<TextureRect>("alarmed");
			_phoneEffect.Hide();
			_clueCard = GetNode<TextureRect>("UI/Clue");
			_clueLabel = GetNode<Label>("UI/Clue/clue");
			//_finalQuiz = GetNode<FinalQuiz>("Level/FinalQuiz.tscn");

				_articleButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnArticlePressed)));

				//_exitMenuButton.Connect(Button.SignalName.Pressed,
				//new Callable(this, nameof(OnSettingsPressed)));

				_exitClueButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(clueExit)));

			_computerButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnComputerPressed)));

				_phoneButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnPhonePressed)));

				_fridgeButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnFridgePressed)));
				_tableButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnTablePressed)));
					_bedButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnBedPressed)));
				_frogButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnFrogPressed)));

				_boardButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnBoardPressed)));
//
				_curtainButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnCurtainPressed)));

				_closetButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnClosetPressed)));
				_animationPlayer = GetNode<AnimationPlayer>("piece/AnimationPlayer");
				_dinnerTableButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnDinnerTablePressed)));
				_animationPlayer = GetNode<AnimationPlayer>("piece/AnimationPlayer");




				_soundPlayer = GetNode<AudioStreamPlayer>("SoundPlayer");
				 _ringtoneSound = GD.Load<AudioStream>("res://music/phone-ringtone.mp3");
				 _paperSound = GD.Load<AudioStream>("res://music/paper.mp3");
				  _callEndedSound = GD.Load<AudioStream>("res://music/unavailable.mp3");
				  _callStartedSound = GD.Load<AudioStream>("res://music/phoneCall.mp3");
				 _dingSound = GD.Load<AudioStream>("res://music/ding.mp3");
				 _clickSound = GD.Load<AudioStream>("res://music/click.mp3");
				 _cabinetSound = GD.Load<AudioStream>("res://music/cabinet.mp3");
				 _bedSound = GD.Load<AudioStream>("res://music/bed.mp3");
				_backpackSound = GD.Load<AudioStream>("res://music/bed.mp3");


			ResetGame();
		}
        //
        public void OnCurtainPressed()
		{
			if(_finalQuiz.quizPoints<=0)
			{
				_testPoints = 0;
			}
			else
			{
				_testPoints = _finalQuiz.quizPoints;
			}

			// Pause game and hide UI and character
			if (_testPoints>=3)
			{
			GetTree().Paused = true;
			UIVisible(false);
			_bernand.Hide();
			_bernand.StopMovement();
			_settingsButton.Hide();
			_isGameFinished = true;


			// Make sure EndScene empty
			if (_ending != null)
			{
				_ending.QueueFree();
				_ending = null;
			}

			if (_articlePieces == 3 && _testPoints<=3)
			{

				//if (_testPoints==3)
				//{
					//Initialize new ending
					_endScene = ResourceLoader.Load<PackedScene>(_goodEndScenePath);
					if (_endScene == null)
					{
						GD.PrintErr("End scene can't be found");
						return;
					}
				}//
				_ending = _endScene.Instantiate<Ending>();
				_ending.SetMiniGameScore(_miniGameTurns);
				_ending.SetTestScore(_testPoints);
				AddChild(_ending);
			}
			else if (_articlePieces==3 && _testPoints < 3)
			{
					//Initialize new ending
					_endScene = ResourceLoader.Load<PackedScene>(_badEndScenePath);
					if (_endScene == null)
					{
						GD.PrintErr("End scene can't be found");
						return;
					}

				_ending = _endScene.Instantiate<Ending>();
				_ending.SetMiniGameScore(_miniGameTurns);
				_ending.SetTestScore(_testPoints);
				AddChild(_ending);
			}
		}

        public void OnClosetPressed()
        {
            PlaySound(_cabinetSound);
        }

		 public void OnDinnerTablePressed()
        {
			if(_articlePieces==2)
			{
				_dialogueBubble.Call("start", "clue");
				_articleButton.Show();
			}
        }
		 public void OnBriefcasePressed()
        {
            PlaySound(_backpackSound);
        }

        public void OnBedPressed()
        {
				_dialogueBubble.Call("start", "bed");
        }
		 public void OnBoardPressed()
        {
			_animationPlayer.Stop();
			PlaySound(_paperSound);
			_dialogueBubble.Call("start", "later");

			if(clueCollected)
			{
				StartMiniGame();
			}
        }

        public void OnFrogPressed()
        {
            PlaySound(_dingSound);
        }

        public void ResetGame()
		{
			ArticlePieces= 0;
			_articleButton.Hide();
			_howToPlay.Show();
			// Dialogue();
		}

		public void OnFridgePressed()
		{
			PlaySound(_cabinetSound);
			GD.Print("jääkaappi");
			if (!clueCollected && callGloria &&_articlePieces == 1)
			{
				//_dialogueBubble.Call("start", "honey");
				_dialogueBubble.Call("start", "honeyfound");
				_articleButton.Show();
			}
			else
			{
				//_dialogueBubble.Call("start", "fridge");
			}
		}

		public void OnTablePressed()
		{
			PlaySound(_cabinetSound);
			GD.Print("pöytä");
			GD.Print("jääkaappi");
			if (callGloria &&_articlePieces == 1)
			{
				//_dialogueBubble.Call("start", "honeyfound");
				//_articleButton.Show();
			}
			else
			{
				_dialogueBubble.Call("start", "honeyhere");
			}
		}
		/*
		 public void OnSettingsPressed()
		{
			_UIpressed = true;
			if (_showInGameMenu)
			{
				GetTree().Paused = false;
				_showInGameMenu = false;
				_inGameMenu.Hide();
				_menuButton.Hide();
				_exitMenuButton.Hide();
				Movement();
			}
			else
			{
				GetTree().Paused = true;
				_showInGameMenu = true;
				_inGameMenu.Show();
				_menuButton.Show();
				_exitMenuButton.Show();
				Movement();
			}
		}
	*/
		private async void OnArticlePressed()
		{
			bool firstButton = true;
			bool secondButton = true;
			bool thirdButton = true;
			await timerAsync(1);
			PlaySound(_paperSound);
			_dialogueBubble.Call("start","cluefound");
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
			{ _articleButton.Hide();
				firstButton=false;
				_articleButton.GlobalPosition = new Vector2(87, -91);
				_animationPlayer.Play("reminder");
			}
			else if (_articlePieces == 1 && secondButton)
			{
				_animationPlayer.Stop();
				_articleButton.Hide();
				secondButton=false;
				_articleButton.GlobalPosition = new Vector2(91, 61);
			}
			else if (_articlePieces >= 2 && thirdButton)
			{
				thirdButton=false;
				_articleButton.Hide();
			}
			clueCollected= true;
			_animationPlayer.Play("clueboard");
			// Start minigame when article collected
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
			PlaySound(_clickSound);
			if(!_showInGameMenu && !_isDialogueRunning)
			{
				GD.Print("Tietokonetta painettu");

				if (_articlePieces==3)
				{
					//dialogueStarter("quiz");
					finalQuiz();
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
		public async void Movement()
		{
		await timerAsync(1);
		 _UIpressed=false;
		}
		public void OnDialogueEnded()
		{
				_isDialogueRunning = false;
				if(_articlePieces==0 && _isAnswered)
				{
					_isAnswered = false;
					_phoneEffect.Visible = true;
					Ringing();
				}

				if(!callGloria && _articlePieces==1)
				{
					_animationPlayer.Play("phoneAnimation");
				}

				if (callGloria &&_articlePieces == 1)
				{
					clueAnimation();
				}
		}
		public async void clueAnimation()
		{
				if(_articlePieces==1 && !clueCollected)
				{
			    await timerAsync(15);
		  		_animationPlayer.Play("fridgeAnimation");
				}
				if(_articlePieces==2 && !clueCollected)
				{
			    await timerAsync(15);
		  		_animationPlayer.Play("dinnerTable");
				}
				if(clueCollected)
				{
					_animationPlayer.Stop();
				}
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
			    await timerAsync(2);
		  		_dialogueBubble.Call("_on_dialogue_ended");
				_isDialogueRunning = false;
		}
		private void OnDialogueStarted(string id)
		{
			_isDialogueRunning = true;
		}
		private void DialogueAnimationSkipper()
		{
			if(_isDialogueRunning);
			{
			}
		}
			private async void OnPhonePressed()
		{
			_animationPlayer.Stop();
			_phoneEffect.Hide();
			if (_ringtonePlayer.Playing)
			{
				_ringtonePlayer.Stop();
			}
			if(!_showInGameMenu & _articlePieces==0 & !callAnswered)
			{
				callAnswered=true;
				await timerAsync(1);
				dialogueStarter("phone");
				//_dialogueBox.Call("start", "phone");
				await timerAsync(5);
				_articleButton.Show();
				_animationPlayer.Play("flying_magazine");
				PlaySound(_paperSound);
				await timerAsync(5);
				_animationPlayer.Play("reminder");
			}
			else if (!callGloria && _articlePieces==1)
			{
				callGloria=true;
				PlaySound(_callStartedSound);
				await timerAsync(2);
				_soundPlayer.Stop();
				dialogueStarter("callGloria");
			}
		}

			public void OnSettingsGuiInput(InputEvent @event)
		{
			if (@event is InputEventMouseButton mb)
			{
				if (mb.ButtonIndex == MouseButton.Left && mb.Pressed)
				{
					InGameMenu.OnSettingsPressed();
				}
			}
		}
		public void OnMenuPressed()
		{
			GetTree().Paused = false;
			_inGameSceneTree.ChangeSceneToFile("res://Code/UI/MainMenu.tscn");
		}
		public void Dialogue()
		{
			string startId = (string)_dialogueBox.Get("start_id");
			_dialogueBox.Call("start", startId);
		}
		public void Ringing()
		{
			_animationPlayer.Play("alarmed");
			_ringtonePlayer.Play();
			//PlaySound(_ringtoneSound);
			return;
		}

		public void PlaySound(AudioStream sound)
		{
			// Check if the _soundPlayer is initialized and if the sound parameter is not null
			if (_soundPlayer != null && sound != null)
			{
				// Assign the sound stream to the AudioStreamPlayer
				_soundPlayer.Stream = sound;

				// Play the sound
				_soundPlayer.Play();
			}
		}

		public void callEnded()
		{
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

		 private void showClueLabel()
		{
			_clueCard.Show();
			_isDialogueRunning = true;

			if (_articlePieces==2)
			{

			_clueLabel.Text = @"Hello dear villagers!

			Bear scientists say: The honey making process is strict and we don’t let any foreign objects get in.
			Worry not this honey is safe to consume!

			-CredibleSource.com-
			Signed by, The Mayor.";
			}
			//Find facts about the Mayor’s honey business certificated by scientists from Honey Science Inc.
			else if (_articlePieces==3)
			{
			_clueLabel.Text = @"GLORIA REVEALS THE TRUTH!
				Village mayor is actually a robot in a mechanical bear suit trying to turn us into robots by eating microchips!

				Sources: Gloria, neighbor, various people from gloriareveals.com
				Author: Gloria Graves
				www.gloriareveals.bear";
			}
		}

		private void clueExit()
		{

			_clueCard.Hide();
			_isDialogueRunning = false;
			if(_articlePieces==1)
			{
				dialogueStarter("firstClue");
			}
			else if (_articlePieces==2)
			{
				dialogueStarter("secondClue");
			}
			else if(_articlePieces==3)
			{
				dialogueStarter("thirdClue");
			}
		}
		public void finalDialogue()
		{
			if (_isQuizDone == true)
			{
				dialogueStarter("final");
			}
		}
		private void finalQuiz()
		{
			if (_finalQuiz != null)
			{
				_finalQuiz.QueueFree();
				_finalQuiz = null;
			}

			if (_finalQuizScene == null)
			{
				//Initialize new minigame
				_finalQuizScene = ResourceLoader.Load<PackedScene>(_finalQuizScenePath);
				if (_finalQuizScene == null)
				{
					GD.PrintErr("MiniGame can't be found");
					return;
				}
			}
			_finalQuiz = _finalQuizScene.Instantiate<FinalQuiz>();
			AddChild(_finalQuiz);
		}
		private void dialogueStarter(string dialogueID)
		{
				_dialogueBox.Call("start", dialogueID);
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
			clueCollected=false;
			_miniGameTurns += _miniGame.TurnsTaken;
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
			showClueLabel();
		}
	}
}