using Godot;
using PalaSoliisi;
using System;

public partial class Bernand : CharacterBody2D
{
	[Export]
    public int Speed { get; set; } = 400;
    private Vector2 _target;
    public bool _isPaused = false;

    public override void _Process(double delta)
    {
        if (_isPaused)
        {
            return;
        }
        else
        {
            Velocity = Position.DirectionTo(_target) * Speed;
            //LookAt(_target);
            if (Position.DistanceTo(_target) > 10)
            {
                MoveAndSlide();
            }
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (Level.Current._isMiniGameRunning || Level.Current._isDialogueRunning)
        {
            return;
        }
        if (Level.Current._UIpressed)
        {
                return;
        }
        else if (@event is InputEventMouseButton mouseEvent
            && mouseEvent.ButtonIndex == MouseButton.Left
            && !mouseEvent.Pressed)
        {
            _target = GetGlobalMousePosition();
        }
            //if (@event is InputEventScreenTouch touchEvent
             //   && touchEvent.Pressed
            //    && touchEvent.Index == 0)
            //{
              //  _target = GetGlobalMousePosition();
            //}
	}

    public void StopMovement()
    {
        this._isPaused = true;
    }

    public void StartMovement()
    {
        this._isPaused = false;
    }
}