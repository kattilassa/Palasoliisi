using Godot;

namespace PalaSoliisi
{
    public partial class Gamename : Label
    {
        public override void _Ready()
        {
            StartBouncing();
        }

        private void StartBouncing()
        {
            Vector2 startPos = Position;
            Vector2 endPos = startPos + new Vector2(0, -10);

            var tween = CreateTween();


            tween.TweenProperty(this, "position", endPos, 1.0)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.InOut);


            tween.TweenProperty(this, "position", startPos, 0.6)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.InOut);

            tween.SetLoops();
        }
    }
}
