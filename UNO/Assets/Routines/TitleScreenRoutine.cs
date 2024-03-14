using LuumieEngine.Classes;
using LuumieEngine.SceneManagement;
using UNO.Assets.Screens;

namespace UNO.Assets.Routines;

public class TitleScreenRoutine : Routine
{
    protected override void OnKeyPress(ConsoleKeyInfo keyInfo)
    {
        SceneManager.ChangeScene(typeof(MainMenuScreen));
    }
}