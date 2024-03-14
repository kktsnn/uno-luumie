using LuumieEngine.Classes;
using LuumieEngine.Components;
using LuumieEngine.SceneManagement;
using LuumieEngine.Structs;
using UNO.Assets.Routines;

namespace UNO.Assets.Screens;

public class TitleScreen : Scene
{
    public TitleScreen()
    {
        var logo = AddEntity(SceneElements.Logo("LogoBig"));
        logo.Transform.LocalPosition = new Vector2Int(0, 6);
        
        var text = AddEntity(GameEntity.Empty);
        text.Transform.LocalPosition = new Vector2Int(0, -25);
        var t = text.AddComponent<Text>();
        t.Content = "Press Any Key To Continue...";

        var listener = AddEntity(GameEntity.Empty);
        listener.AddComponent<TitleScreenRoutine>();
    }
}