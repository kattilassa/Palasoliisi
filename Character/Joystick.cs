using Godot;
using System;

public partial class Joystick : TextureRect
{
    private Vector2 _startPosition;
    private Vector2 _direction = Vector2.Zero;
    private bool _isDragging = false;

    [Export] public float DeadZone = 10f;
    [Export] public float MaxDistance = 50f;

    public override void _Ready()
    {
        _startPosition = Position;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventScreenTouch touch)
        {
            HandleTouchInput(touch.Position, touch.Pressed);
        }
        else if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed)
        {
            HandleTouchInput(mouseButton.Position, mouseButton.Pressed);
        }
        else if (@event is InputEventMouseMotion mouseMotion && _isDragging)
        {
            HandleTouchInput(mouseMotion.Position, true);
        }
    }

    private void HandleTouchInput(Vector2 position, bool pressed)
    {
        if (pressed)
        {
            _isDragging = true;

            // Liikkuminen kiinteällä etäisyydellä aina sen mukaan, mihin suuntaan painetaan
            _direction = (position - _startPosition).LimitLength(MaxDistance).Normalized();
            Position = _startPosition + _direction * MaxDistance;
        }
        else
        {
            _isDragging = false;
            _direction = Vector2.Zero;
            Position = _startPosition;
        }
    }

    public Vector2 GetDirection()
    {
        return _direction;
    }
}
