using Godot;
using PalaSoliisi;
using System;

public partial class Bernand : CharacterBody2D
{
	[Export]
    public int Speed { get; set; } = 400;
    private Vector2 _target;

    public override void _Input(InputEvent @event)
    {

		if (Level.Current._UIpressed || Level.Current._showInGameMenu)
		{
			Level.Current._UIpressed = false;
			return;
		}
		if (@event is InputEventMouseButton mouseEvent
                && mouseEvent.ButtonIndex == MouseButton.Left
                && !mouseEvent.Pressed)
        {
            _target = GetGlobalMousePosition();
        }

		if (@event is InputEventScreenTouch touchEvent
			&& touchEvent.Pressed
			&& touchEvent.Index == 0)
		{
			_target = GetGlobalMousePosition();
		}
	}
    public override void _PhysicsProcess(double delta)
    {
        Velocity = Position.DirectionTo(_target) * Speed;
        //LookAt(_target);
        if (Position.DistanceTo(_target) > 10)
        {
            MoveAndSlide();
        }
    }
}