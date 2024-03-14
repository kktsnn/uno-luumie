using LuumieEngine.Classes;
using LuumieEngine.Components;
using LuumieEngine.SceneManagement;
using LuumieEngine.Structs;
using UNO.Assets.Routines;
using UNO.Assets.Routines.Game;

namespace UNO.Assets.Screens;

// TODO Add "At least one player to play"
// TODO Make card green when startable
public class MatchSetupScreen : Scene
{
    public MatchSetupScreen()
    {
        AddEntity(SceneElements.Header("MatchSetup"));
        AddEntity(SceneElements.Framerate());

        var e = AddEntity(GameEntity.Empty);
        e.AddComponent<MatchSetupRoutine>();
        var mainSystem = e.AddComponent<UiNavigation>();
        
        var play = SceneElements.Card("Play", ConsoleColor.Red);
        play.Transform.LocalPosition = new Vector2Int(70, -5);
        var playButton = play.AddComponent<Button>();
        playButton.Graphic = play.GetComponent<AsciiImage>();
        playButton.HighlightColor = ConsoleColor.DarkRed;

        e.AddChild(play);
        
        var sText = SceneElements.BigText("Settings");
        sText.Transform.LocalPosition = new Vector2Int(15, 15);
        sText.Transform.Depth = 4;
        
        e.AddChild(sText);
        
        var settings = GameEntity.Empty;
        settings.Transform.LocalPosition = new Vector2Int(15, -5);
        settings.AddComponent<UiNavigation>().EnabledSelf = false;
        var sBox = settings.AddComponent<Box>();
        sBox.Size = new Vector2Int(50, 41);
        sBox.Color = ConsoleColor.DarkGray;
        var sView = settings.AddComponent<SettingsView>();
        sView.PlayButton = playButton;
        
        e.AddChild(settings);
        
        var pText = SceneElements.BigText("Players");
        pText.Transform.LocalPosition = new Vector2Int(-45, 15);
        pText.Transform.Depth = 4;
        
        e.AddChild(pText);
        
        var players = GameEntity.Empty;
        players.Transform.LocalPosition = new Vector2Int(-45, -5);
        players.AddComponent<UiNavigation>().EnabledSelf = false;
        var pBox = players.AddComponent<Box>();
        pBox.Size = new Vector2Int(50, 41);
        pBox.Color = ConsoleColor.DarkGray;
        var pView = players.AddComponent<PlayersView>();
        pView.MainNav = mainSystem;
        pView.SView = sView;
        
        e.AddChild(players);
        
        playButton.Action = () =>
        {
            if (pView.Players.Count == 0) return;
            GameController.StartNewGame(pView.Players, sView.Rules);
            SceneManager.ChangeScene(typeof(GameScreen));
        };
    }
}