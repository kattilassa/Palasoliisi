// Bernand.cs
//
// Controls the movement and animation of the in-game character.
// Uses NavigationAgent2D for pathfinding.
// Character movement by Kati Savolainen.
//
// Movement is paused by Level.cs during mini-games.
// StopMovement and StartMovement methods added by Mimmi Tamminen.

using Godot;
using PalaSoliisi;
using System;
public partial class Bernand : CharacterBody2D
{
    // Movement speed of (Bernard) the character.
    [Export]
    public int Speed { get; set; } = 400;

    private Vector2 _target;
    //Target position where the character should move.
    //Set on mouse click, used by the pathfinding agent.
    public bool _isPaused = false;
    //Public bool when to pause/resume char movement.
    //This is controlled in Level.cs StartMiniGame method

    private AnimationPlayer walkingAnimation; //Handles walking animation for the character

    private NavigationAgent2D agent; //Used for character pathfinding

    public override void _Ready()
    {
         // Get references to the NavigationAgent2D and AnimationPlayer in the scene
        agent = GetNode<NavigationAgent2D>("NavigationAgent2D");
        walkingAnimation = GetNode<AnimationPlayer>("AnimationPlayer");

    }

    public override void _Process(double delta)
    {
        //Stop character movement if paused by the minigame or if
        //NavigationAgent2D is missing/empty, or path is finished.
        if (_isPaused || agent == null || agent.IsNavigationFinished())
        {
            Velocity = Vector2.Zero;
            walkingAnimation.Stop();
            //Stops the walking animation when path is finished.
            return;
        }

       //Calculate the direction and speed toward the next path point and move the character.
        Vector2 nextPathPos = agent.GetNextPathPosition();
        Vector2 direction = (nextPathPos - GlobalPosition).Normalized();
        Velocity = direction * Speed;
        MoveAndSlide();
        //Play walking animation if it's not already playing.
        if (!walkingAnimation.IsPlaying())
        {
            walkingAnimation.Play("walking");
        }
    }

    public override void _Input(InputEvent @event)
    {
         //Ignore user input if a mini-game or dialogue is running.
         //or if the UI is pressed/active.
        if (Level.Current._isMiniGameRunning || Level.Current._isDialogueRunning || Level.Current._UIpressed)
            return;

        // Handle mouse click to set a new target position.
        if (@event is InputEventMouseButton mouseEvent
            && mouseEvent.ButtonIndex == MouseButton.Left
            && !mouseEvent.Pressed)
        {
            _target = GetGlobalMousePosition();
            agent.TargetPosition = _target;
            //Update the agent's target for pathfinding and agent calculates the path.
        }
    }
    // Stops the character from moving.
    // This is called from the Level.cs file from the method StartMiniGame.
    public void StopMovement()
    {
        _isPaused = true;
        Velocity = Vector2.Zero;
    }
    // Allows the character to move.
    // This is called from the Level.cs file from the method OnMiniGameCompleted.
    public void StartMovement()
    {
        _isPaused = false;
    }
}
