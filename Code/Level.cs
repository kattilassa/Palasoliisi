using Godot;
using PalaSoliisi.UI;
using System;
using System.Threading.Tasks;

namespace PalaSoliisi
{
	public partial class Level : Node2D
	{
		// Ui elements references
		[Export] public Label _clueLabel;
		[Export] private ScoreUIControl _scoreUIControl = null;
		public Control _inGameMenu;
		private Control _UI;
		public Control _howToPlay = null;


		// Texture buttons for interacting with objects and furniture in the game
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
		[Export] private TextureButton _ovenButton = null;

		[Export] private Button _exitClueButton = null;

		// Scene paths for the minigame and endings
		[Export] private string _miniGameScenePath = "res://Levels/MiniGame.tscn";
		[Export] private string _goodEndScenePath = "res://Levels/GoodEnd.tscn";
		[Export] private string _badEndScenePath = "res://Levels/BadEnd.tscn";
		[Export] private string _finalQuizScenePath = "res://Levels/FinalQuiz.tscn";

		private static Level _current = null;
		private SceneTree _inGameSceneTree = null;
		private SceneTree _optionsSceneTree = null;
		private Bernand _bernand = null;

		private MiniGame _miniGame = null;
		private Ending _ending = null;
		public FinalQuiz _finalQuiz = null;

		// Sound effects animation player
		[Export] AudioStreamPlayer _ringtonePlayer;
		private AudioStreamPlayer _soundPlayer;
        private AudioStream _ringtoneSound;
        private AudioStream _paperSound;
        private AudioStream _callStartedSound;
        private AudioStream _dingSound;
        private AudioStream _clickSound;
        private AudioStream _cabinetSound;
        private AudioStream _backpackSound;
		private AudioStream _ovenSound;
		private AudioStream _phoneSound;
		public AnimationPlayer _animationPlayer;

		private PackedScene _dialogueScene = null;
		private PackedScene _miniGameScene = null;
		private PackedScene _finalQuizScene = null;
		private PackedScene _endScene = null;

		private Node _dialogueBox;
		private Node _dialogueBubble;

		public TextureRect _phoneEffect;
		public TextureRect _clueCard;
		public TextureRect _computerOn;

		// Flags for UI visibility and game states
		public bool _showInGameMenu = false;
		public bool _showDialogue = false;
		public bool _settingsClose = false;
		public bool _UIpressed = false;
		public bool _isMiniGameRunning = false;
		public bool _isGameFinished = false;
		public bool _isDialogueRunning = false;
		public bool _isQuizDone = false;
		public bool callAnswered = false;
		public bool callGloria= false;
		public bool clueCollected= false;
		public bool _isAnswered = true;
		public bool isRunning;

		private int _articlePieces = 0;
		private int _miniGameTurns = 0;
		private int _testPoints = 0;

		public static Level Current
		{
			get { return _current; }
		}

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

		// Constructor
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
			_bernand = GetNode<Bernand>("Camera2D/Bernand");

			_inGameMenu.Hide();

			_dialogueBox.Connect("dialogue_ended", new Callable(this, nameof(OnDialogueEnded)));
			_dialogueBox.Connect("dialogue_started", new Callable(this, nameof(OnDialogueStarted)));
			_dialogueBubble.Connect("dialogue_started", new Callable(this, nameof(OnDialogueBubbleStarted)));
			_settingsButton.Connect("gui_input", new Callable(this, nameof(OnSettingsGuiInput)));

			_phoneEffect = GetNode<TextureRect>("alarmed");
			_phoneEffect.Hide();
			_computerOn = GetNode<TextureRect>("UI/ComputerButton/ComputerOn");
			_computerOn.Hide();

			_clueCard = GetNode<TextureRect>("UI/Clue");
			_clueLabel = GetNode<Label>("UI/Clue/clue");

			// Read input for when button or furniture clicked
			_articleButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnArticlePressed)));
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
			_curtainButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnCurtainPressed)));
			_ovenButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnOvenPressed)));
			_closetButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnClosetPressed)));
			_dinnerTableButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnDinnerTablePressed)));
			_animationPlayer = GetNode<AnimationPlayer>("piece/AnimationPlayer");

			// Animation player and sound effects
			_animationPlayer = GetNode<AnimationPlayer>("piece/AnimationPlayer");
			_soundPlayer = GetNode<AudioStreamPlayer>("SoundPlayer");
				_ringtoneSound = GD.Load<AudioStream>("res://music/phone-ringtone.mp3");
				_paperSound = GD.Load<AudioStream>("res://music/paper.mp3");
				_callStartedSound = GD.Load<AudioStream>("res://music/phoneCall.mp3");
				_dingSound = GD.Load<AudioStream>("res://music/ding.mp3");
				_clickSound = GD.Load<AudioStream>("res://music/click.mp3");
				_cabinetSound = GD.Load<AudioStream>("res://music/cabinet.mp3");
				_backpackSound = GD.Load<AudioStream>("res://music/backpack.mp3");
				_ovenSound = GD.Load<AudioStream>("res://music/oven-door.mp3");
				_phoneSound = GD.Load<AudioStream>("res://music/phone.mp3");

			// Reset game when scene opened
			ResetGame();
		}

		/// <summary>
		/// Reset game. Hide articles and reset score.
		/// </summary>
		public void ResetGame()
		{
			ArticlePieces= 0;
			_articleButton.Hide();
			_howToPlay.Show();
		}

		/// <summary>
		/// When curtain pressed after final quiz complete end game and initiate end scene
		/// </summary>
        public void OnCurtainPressed()
		{
			// If final quiz not complete initiate pop-up dialogue bubble
			if(!_isQuizDone)
			{
				_dialogueBubble.Call("start", "outside");
			}

			GD.Print("Curtain pressed");

			// After final quiz character can leave the room
			if(_isQuizDone == true)
			{
				// Pause game and hide UI and character
				GetTree().Paused = true;
				UIVisible(false);
				_bernand.Hide();
				_bernand.StopMovement();
				_settingsButton.Hide();
				_isGameFinished = true;

				// Set test score for UI
				if(_finalQuiz.quizPoints<=0)
				{
					_testPoints = 0;
				}
				else
				{
					_testPoints = _finalQuiz.quizPoints;
				}

				// Make sure EndScene empty
				if (_ending != null)
				{
					_ending.QueueFree();
					_ending = null;
				}

				// Good end scene
				if (_testPoints>=5)
				{
					// Initialize new ending
					_endScene = ResourceLoader.Load<PackedScene>(_goodEndScenePath);
					if (_endScene == null)
					{
						GD.PrintErr("End scene can't be found");
						return;
					}
				}

				// Bad end scene
				else if (_testPoints < 5)
				{
					// Initialize new ending
					_endScene = ResourceLoader.Load<PackedScene>(_badEndScenePath);
					if (_endScene == null)
					{
						GD.PrintErr("End scene can't be found");
						return;
					}
				}

				_ending = _endScene.Instantiate<Ending>();
				_ending.SetMiniGameScore(_miniGameTurns);
				_ending.SetTestScore(_testPoints);
				AddChild(_ending);
			}
		}

		/// <summary>
		/// When closet pressed play sound effect
		/// </summary>
        public void OnClosetPressed()
        {
            PlaySound(_cabinetSound);
        }

		/// <summary>
		/// When oven pressed play sound effect
		/// </summary>
 		public void OnOvenPressed()
        {
            PlaySound(_ovenSound);
        }

		/// <summary>
		/// Give final clue if 2 already found
		/// </summary>
		 public void OnDinnerTablePressed()
        {
			if(_articlePieces==2)
			{
				_dialogueBubble.Call("start", "clue");
				_articleButton.Show();
			}
        }

		/// <summary>
		/// When suitcase pressed play sound effect
		/// </summary>
		 public void OnBriefcasePressed()
        {
            PlaySound(_backpackSound);
        }

		/// <summary>
		/// When bed pressed initiate dialogue bubble
		/// </summary>
        public void OnBedPressed()
        {
				_dialogueBubble.Call("start", "bed");
        }

		/// <summary>
		/// Start minigame if clue is collected
		/// </summary>
		 public async void OnBoardPressed()
        {
			await timerAsync(2);
			_animationPlayer.Stop();
			PlaySound(_paperSound);
			_dialogueBubble.Call("start", "later");

			if(clueCollected)
			{
				StartMiniGame();
			}
        }

		/// <summary>
		/// When alarm clock pressed play sound effect
		/// </summary>
        public void OnFrogPressed()
        {
            PlaySound(_dingSound);
        }

		/// <summary>
		/// Give second clue if 1 already found
		/// </summary>
		public void OnFridgePressed()
		{
			PlaySound(_cabinetSound);
			GD.Print("jääkaappi");
			if (!clueCollected && callGloria &&_articlePieces == 1)
			{
				_dialogueBubble.Call("start", "honeyfound");
				_articleButton.Show();
			}
		}

		/// <summary>
		/// When sidetable pressed play sound effect and initiate dialogue bubble
		/// </summary>
		public void OnTablePressed()
		{
			PlaySound(_cabinetSound);
			GD.Print("pöytä");
			GD.Print("jääkaappi");

			_dialogueBubble.Call("start", "honeyhere");

		}

		/// <summary>
		/// When found article pressed start dialogue bubble and direct player to clue board
		/// </summary>
		private async void OnArticlePressed()
		{
			bool firstButton = true;
			bool secondButton = true;
			bool thirdButton = true;
			await timerAsync(1);
			PlaySound(_paperSound);
			_dialogueBubble.Call("start","cluefound");

			if(_articlePieces == 0 && firstButton)
			{ _articleButton.Hide();
				firstButton=false;
				_articleButton.GlobalPosition = new Vector2(87, -91);
				// Play animation for visual clue for the player
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
			// Play animation for visual clue for the player
			_animationPlayer.Play("clueboard");
		}

		/// <summary>
		/// Toggle UI visibility
		/// </summary>
		/// <param name="visible">True if UI visible</param>
		private void UIVisible(bool visible)
		{
			_UI.Visible = visible;
		}

		/// <summary>
		/// Start final quiz if all clues collected
		/// </summary>
		private async void OnComputerPressed()
		{
			await timerAsync(1);
			PlaySound(_clickSound);
			if(!_showInGameMenu && !_isDialogueRunning)
			{
				GD.Print("Tietokonetta painettu");

				if (_articlePieces>=3)
				{
					finalQuiz();
					_computerOn.Hide();
				}
				else if (!_isDialogueRunning)
				{
				Random rnd = new Random();
				int randomDialogue = rnd.Next(1, 6);
				String startID = $"{randomDialogue}";

				// Get random dialogue from Bernard (5 versions)
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

		/// <summary>
		/// After first dialogue start phone sound effect and animation
		/// </summary>
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
				else if (_articlePieces == 2)
				{
					clueAnimation();
				}
		}

		/// <summary>
		/// Animation on clue hiding places
		/// </summary>
		public void clueAnimation()
		{
				if(_articlePieces==1 && !clueCollected)
				{
		  		_animationPlayer.Play("fridgeAnimation");
				}
				else if (_articlePieces==2 && !clueCollected)
				{
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

		/// <summary>
		/// Start phone dialogue based on amount of clues found
		/// </summary>
		private async void OnPhonePressed()
		{
			PlaySound(_phoneSound);

			if (_ringtonePlayer.Playing)
			{
				_phoneEffect.Hide();
				_ringtonePlayer.Stop();
			}

			// Answer the phone to get the first clue
			if(!_showInGameMenu & _articlePieces==0 & !callAnswered)
			{
				callAnswered=true;

				await timerAsync(1);
				dialogueStarter("phone");

				await timerAsync(5);
				_articleButton.Show();
				_animationPlayer.Play("flying_magazine");
				PlaySound(_paperSound);

				await timerAsync(5);
				_animationPlayer.Play("reminder");
			}
			// When first clue found start dialogue on callinf gloria
			else if (!callGloria && _articlePieces==1)
			{
				_animationPlayer.Stop();

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

		/// <summary>
		/// Change scene back to main menu
		/// </summary>
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

		/// <summary>
		/// Show clue label when minigame solved. Change text based on the amount of clues found
		/// </summary>
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

			else if (_articlePieces==3)
			{
				_computerOn.Show();
			_clueLabel.Text = @"GLORIA REVEALS THE TRUTH!
				Village mayor is actually a robot in a mechanical bear suit trying to turn us into robots by eating microchips!

				Sources: Gloria, neighbor, various people from gloriareveals.com
				Author: Gloria Graves
				www.gloriareveals.bear";
			}
		}

		/// <summary>
		/// Close clue label and start dialogue
		/// </summary>
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

		/// <summary>
		/// Start dialogue to instruct player to pree the curtain and end the game
		/// </summary>
		public void finalDialogue()
		{
			if (_isQuizDone == true)
			{
				dialogueStarter("final");
				_animationPlayer.Play("curtainAnimation");

			}
		}

		/// <summary>
		/// Start final quiz scene
		/// </summary>
		private void finalQuiz()
		{
			// Make sure no previous scene open
			if (_finalQuiz != null)
			{
				_finalQuiz.QueueFree();
				_finalQuiz = null;
			}

			if (_finalQuizScene == null)
			{
				//Initialize new final quiz
				_finalQuizScene = ResourceLoader.Load<PackedScene>(_finalQuizScenePath);
				if (_finalQuizScene == null)
				{
					GD.PrintErr("MiniGame can't be found");
					return;
				}
			}

			// Open final quiz scene
			_finalQuiz = _finalQuizScene.Instantiate<FinalQuiz>();
			AddChild(_finalQuiz);
		}

		private void dialogueStarter(string dialogueID)
		{
				_dialogueBox.Call("start", dialogueID);
		}

		/// <summary>
		/// Opens MiniGame scene when article collected and clue board clicked
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