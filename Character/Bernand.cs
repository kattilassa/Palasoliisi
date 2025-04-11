using Godot;
using PalaSoliisi;
using System;

public partial class Bernand : CharacterBody2D
{
    [Export]
    public int Speed { get; set; } = 400;

    private Vector2 _target;
    public bool _isPaused = false;
    private AnimationPlayer walkingAnimation;

    private NavigationAgent2D agent;

    public override void _Ready()
    {
        agent = GetNode<NavigationAgent2D>("NavigationAgent2D");
        walkingAnimation = GetNode<AnimationPlayer>("AnimationPlayer");

    }

    public override void _Process(double delta)
    {
        if (_isPaused || agent == null || agent.IsNavigationFinished())
        {
            Velocity = Vector2.Zero;
            walkingAnimation.Stop();
            return;
        }

        // Get next direction and move
        Vector2 nextPathPos = agent.GetNextPathPosition();
        Vector2 direction = (nextPathPos - GlobalPosition).Normalized();
        Velocity = direction * Speed;
        MoveAndSlide();
        if (!walkingAnimation.IsPlaying())
        {
            walkingAnimation.Play("walking");
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (Level.Current._isMiniGameRunning || Level.Current._isDialogueRunning || Level.Current._UIpressed)
            return;

        if (@event is InputEventMouseButton mouseEvent
            && mouseEvent.ButtonIndex == MouseButton.Left
            && !mouseEvent.Pressed)
        {
            _target = GetGlobalMousePosition();
            agent.TargetPosition = _target; // agent calculates a path
        }
    }

    public void StopMovement()
    {
        _isPaused = true;
        Velocity = Vector2.Zero;
    }

    public void StartMovement()
    {
        _isPaused = false;
    }
}
