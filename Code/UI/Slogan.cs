using Godot;

namespace PalaSoliisi
{
    public partial class Slogan : Label
    {
        public override void _Ready()
        {
            StartSizeBounce();
        }

        private void StartSizeBounce()
        {
            var tween = CreateTween(); // Luodaan Tween dynaamisesti

            // Tekstin alkuperäinen koko (156x36 px)
            Vector2 normalSize = new Vector2(1, 1); // Tämä on normaali koko
            // Suurempi koko, jossa kasvaa 20%
            Vector2 largerSize = new Vector2(1.2f, 1.2f); // 20% isompi

            // Määritetään pivotin alkuperäinen sijainti (keskipiste)
            PivotOffset = new Vector2(0.5f, 0.5f);

            // Animoidaan koon kasvaminen ja pieneminen
            tween.TweenProperty(this, "scale", largerSize, 0.6)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.InOut);

            tween.TweenProperty(this, "scale", normalSize, 0.6)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.InOut);

            tween.SetLoops(); // Toistetaan animaatio loputtomasti
        }
    }
}
