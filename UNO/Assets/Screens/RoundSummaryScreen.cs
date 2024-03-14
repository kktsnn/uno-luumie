using LuumieEngine.Classes;
using LuumieEngine.Components;
using LuumieEngine.SceneManagement;
using LuumieEngine.Structs;
using UNO.Assets.Routines.Game;

namespace UNO.Assets.Screens;

public class RoundSummaryScreen : Scene
{
    private static GameEngine Engine => GameController.Engine;
    
    public RoundSummaryScreen()
    {
        AddEntity(SceneElements.Header("RoundSummary"));
        AddEntity(SceneElements.Framerate());

        var rules = AddEntity(GameEntity.Empty);
        rules.Transform.LocalPosition = new Vector2Int(-75, 15);
        var rulesText = rules.AddComponent<Text>();
        rulesText.Content = Engine.State.Rules.PrettyString();
        rulesText.Alignment = ETextAlignment.Left;

        var players = AddEntity(PlayerView());
        var nav = players.AddComponent<UiNavigation>();

        var next = AddEntity(SceneElements.Card("Next", ConsoleColor.Green));
        next.Transform.LocalPosition = new Vector2Int(70, -20);
        var nextButton = AddMovement(next.AddComponent<Button>(), next.AddComponent<Movement>());
        nextButton.Action = () =>
        {
            GameController.StartRound();
            SceneManager.ChangeScene(typeof(GameScreen));
        };

        var quit = AddEntity(SceneElements.Card("Quit", ConsoleColor.Red));
        quit.Transform.LocalPosition = new Vector2Int(80, -20);
        quit.Transform.Depth = 0;
        var quitButton = AddMovement(quit.AddComponent<Button>(), quit.AddComponent<Movement>());
        quitButton.Action = () =>
        {
            if (Engine.MatchOver())
                GameController.Repository.Delete(Engine.State.GameId);
            SceneManager.ChangeScene(typeof(MainMenuScreen));
        };
        
        if (Engine.MatchOver())
        {
            next.ActiveSelf = false;
            nav.Current = quitButton;
        }
        else nav.Current = quitButton.Left = nextButton;
    }

    private GameEntity PlayerView()
    {
        var e = GameEntity.Empty;

        const int skip = 20;
        var offset = -skip * (Engine.Players.Count - 1) / 2;
        foreach (var p in Engine.Players)
        {
            var c = SceneElements.Card(null, ConsoleColor.White);
            c.Transform.LocalPosition = new Vector2Int(offset, 0);
            
            var name = c.AddComponent<Text>();
            name.Content = p.Name;
            name.LocalPosition = new Vector2Int(0, 7);

            var points = c.AddComponent<Text>();
            points.Content = p.Points.ToString();
            points.LocalPosition = new Vector2Int(0, -7);

            if (p == Engine.ActivePlayer)
            {
                c.GetComponent<AsciiImage>().Color = ConsoleColor.DarkYellow;
                points.Content += $" (+{Engine.RoundPoints()})";
            }
            
            e.AddChild(c);

            offset += 20;
        }
        
        return e;
    }
    
    private static Button AddMovement(Button b, Movement m)
    {
        b.Selected += () => { m.Offset(new Vector2Int(0, 3)); };
        b.Deselected += () => { m.Offset(new Vector2Int(0, -3)); };
        return b;
    }
}