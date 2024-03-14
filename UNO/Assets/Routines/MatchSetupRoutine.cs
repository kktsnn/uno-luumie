using LuumieEngine.Classes;
using LuumieEngine.Components;
using LuumieEngine.SceneManagement;
using UNO.Assets.Screens;

namespace UNO.Assets.Routines;

public class MatchSetupRoutine : Routine
{
    protected override void OnKeyPress(ConsoleKeyInfo keyInfo)
    {
        if (keyInfo.Key is ConsoleKey.Escape && UiNavigation.Active)
            SceneManager.ChangeScene(typeof(MainMenuScreen));
    }
}