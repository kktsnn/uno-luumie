using LuumieEngine;
using LuumieEngine.Classes;
using LuumieEngine.Components;
using LuumieEngine.SceneManagement;
using LuumieEngine.Structs;
using UNO.Assets.Domain;
using UNO.Assets.Routines;
using UNO.Assets.Routines.Game;

namespace UNO.Assets.Screens;

public class MainMenuScreen : Scene
{
    public MainMenuScreen()
    {
        AddEntity(SceneElements.Header("MainMenu"));
        AddEntity(SceneElements.Framerate());

        var uiSystem = AddUiSystem();
        
        var menu = AddEntity(GameEntity.Empty);
        menu.Transform.LocalPosition = new Vector2Int(0, -8);
        var routine = menu.AddComponent<MainMenuRoutine>();
        routine.Confirmation = AddEntity(SceneElements.ConfirmationPrompt(
            uiSystem,
            "You Are About to Quit\n\nAre You Sure?",
            LuumieManager.Quit)).GetComponent<Prompt>();

        var info = GameEntity.Empty;
        info.Transform.LocalPosition = new Vector2Int(0, -15);
        var text = info.AddComponent<Text>();
        
        var solo = SceneElements.Card("Solo", ConsoleColor.Red);
        solo.Transform.LocalPosition = new Vector2Int(-12, 0);
        solo.Transform.Depth = 0;
        var soloButton = AddMovement(solo.AddComponent<Button>(), solo.AddComponent<Movement>());
        soloButton.Selected += () => { text.Content = "Classic UNO experience"; };
        soloButton.Action = () =>
        {
            List<Player> players = new ()
            {
                new Player(EPlayerType.User, "Player"),
                new Player(EPlayerType.Bot, Player.GenerateBotName()),
                new Player(EPlayerType.Bot, Player.GenerateBotName()),
                new Player(EPlayerType.Bot, Player.GenerateBotName())
            };
            GameController.StartNewGame(players, new GameRules());
            SceneManager.ChangeScene(typeof(GameScreen));
        };
        
        uiSystem.Current = soloButton;
        
        var play = SceneElements.Card("Play", ConsoleColor.Green);
        var playButton = AddMovement(play.AddComponent<Button>(), play.AddComponent<Movement>());
        playButton.Selected += () => { text.Content = "Start a new game"; };
        playButton.Action = () => { SceneManager.ChangeScene(typeof(MatchSetupScreen)); };
        playButton.Left = soloButton;

        var load = SceneElements.Card("Load", ConsoleColor.Blue);
        load.Transform.LocalPosition = new Vector2Int(12, 0);
        load.Transform.Depth = 2;
        var loadButton = AddMovement(load.AddComponent<Button>(), load.AddComponent<Movement>());
        loadButton.Selected += () => { text.Content = "Load a save state"; }; 
        loadButton.Action = () => { SceneManager.ChangeScene(typeof(LoadScreen)); };
        loadButton.Left = playButton;
        
        menu.AddChild(solo);
        menu.AddChild(play);
        menu.AddChild(load);
        menu.AddChild(info);
    }

    private static Button AddMovement(Button b, Movement m)
    {
        b.Selected += () => { m.Offset(new Vector2Int(0, 3)); };
        b.Deselected += () => { m.Offset(new Vector2Int(0, -3)); };
        return b;
    }
}