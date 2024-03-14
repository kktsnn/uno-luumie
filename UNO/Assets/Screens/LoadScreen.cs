using LuumieEngine.Classes;
using LuumieEngine.Components;
using LuumieEngine.SceneManagement;
using LuumieEngine.Structs;
using UNO.Assets.Routines;
using UNO.Assets.Routines.Game;

namespace UNO.Assets.Screens;

public class LoadScreen : Scene
{
    public LoadScreen()
    {
        AddEntity(SceneElements.Header("Load"));
        AddEntity(SceneElements.Framerate());

        var e = AddEntity(GameEntity.Empty);
        var uiNav = e.AddComponent<UiNavigation>();
        
        var loads = e.AddComponent<LoadScreenRoutine>();
        loads.Repository = GameController.Repository;
        e.Transform.LocalPosition = new Vector2Int(0, -29);

        var l = AddEntity(GameEntity.Empty);
        var lWall = l.AddComponent<Background>();
        lWall.Size = new Vector2Int(29, 52);
        lWall.Transform.Depth = 4;
        l.Transform.Position = new Vector2Int(-120, -5);
        
        var r = AddEntity(GameEntity.Empty);
        var rWall = r.AddComponent<Background>();
        rWall.Size = new Vector2Int(29, 52);
        rWall.Transform.Depth = 4;
        r.Transform.Position = new Vector2Int(120, -5);
        
        loads.Confirmation = AddEntity(SceneElements.ConfirmationPrompt(
            uiNav,
            "You are about to delete a save\n\nAre You Sure?",
            loads.DeleteCurrent)).GetComponent<Prompt>();
    }
}
