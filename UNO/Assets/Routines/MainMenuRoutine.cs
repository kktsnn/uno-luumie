using LuumieEngine.Classes;
using LuumieEngine.Components;

namespace UNO.Assets.Routines;

public class MainMenuRoutine : Routine
{
    public Prompt Confirmation = null!;

    protected override void OnKeyPress(ConsoleKeyInfo keyInfo)
    {
        if (keyInfo.Key != ConsoleKey.Escape) return;
        if (Confirmation.Enabled) Confirmation.Hide();
        else Confirmation.Show();
    }
}