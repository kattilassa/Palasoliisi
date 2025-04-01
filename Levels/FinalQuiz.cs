using Godot;
using System;
using System.Collections.Generic;

namespace PalaSoliisi
{
public partial class FinalQuiz : Node2D
{
	[Export] private Button _submitButton = null;
	private Node _dialogueBox;
	public int quizPoints;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
			_dialogueBox = GetNode("Control/DialogueBox");
			 _dialogueBox.Connect("variable_changed", new Callable(this, nameof(OnVariableChanged)));
			 _dialogueBox.Connect("dialogue_ended", new Callable(this, nameof(OnDialogueEnded)));
			string startId = (string)_dialogueBox.Get("start_id");
			_dialogueBox.Call("start", startId);
			_submitButton.Connect(Button.SignalName.Pressed,
				new Callable(this, nameof(OnSubmitPressed)));
	}


        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
	{
	}
       private void OnDialogueEnded()
        {
			_submitButton.Show();
        }

private void OnVariableChanged(string variableName, Variant value)
		{
			  GD.Print($"Variable Changed: {variableName} = {value}");
			  quizPoints = (int)value;
			  GD.Print($"Updated quizPoints: {quizPoints}");
		}
	public void OnSubmitPressed()
		{
			Level.Current._isQuizDone = true;
			this.QueueFree();
			Level.Current.finalDialogue();
		}
}
}
