using Godot;
using System;

namespace PalaSoliisi
{
public partial class FinalQuiz : Node2D

{
	private Node _dialogueBox;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
			_dialogueBox = GetNode("Control/DialogueBox");
			string startId = (string)_dialogueBox.Get("start_id");
			_dialogueBox.Call("start", startId);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
}
