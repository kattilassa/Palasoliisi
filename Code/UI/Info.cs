using Godot;
namespace PalaSoliisi;
public partial class Info : Button
{
    private const string Url = "https://pja.edu.pl/en/badania-naukowe/infotester4education/";

    public override void _Ready()
    {
        Pressed += OnButtonPressed;
    }

    private void OnButtonPressed()
    {
        OS.ShellOpen(Url);
    }
}
