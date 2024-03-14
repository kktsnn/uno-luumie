using LuumieEngine.Classes;
using LuumieEngine.Components;
using LuumieEngine.SceneManagement;
using LuumieEngine.Structs;
using UNO.Assets.Routines.Game;

namespace UNO.Assets.Screens;

public class GameScreen : Scene
{
    private UiNavigation _nav;
    public GameScreen()
    {
        AddEntity(SceneElements.Framerate());
        AddEntity(SceneElements.EntityCount());
        
        var main = AddEntity(GameEntity.Empty);
        _nav = main.AddComponent<UiNavigation>();
        var controller = main.AddComponent<GameController>();

        var info = AddEntity(GameEntity.Empty);
        info.Transform.LocalPosition = new Vector2Int(90, -25);
        var infoText = info.AddComponent<Text>();
        infoText.Content = "SPACE - Play/Turn cards\n\nX - Call UNO\n\nZ - Draw Card/End Turn";

        var hand = AddEntity(GameEntity.Empty);
        hand.Transform.LocalPosition = new Vector2Int(0, -25);
        var view = hand.AddComponent<HandView>();
        view.Nav = _nav;

        var pile = AddEntity(GameEntity.Empty);
        var discard = pile.AddComponent<DiscardPile>();

        var log = AddEntity(GameEntity.Empty);
        log.Transform.LocalPosition = new Vector2Int(-85, -23);
        var logWindow = log.AddComponent<Log>();
        log.AddComponent<Text>();
        
        var colorPrompt = AddEntity(ColorPrompt());
        colorPrompt.Transform.LocalPosition = new Vector2Int(40, 0);
        colorPrompt.ActiveSelf = false;

        var up = AddEntity(GameEntity.Empty);
        up.Transform.LocalPosition = new Vector2Int(0, 7);
        var upArrow = up.AddComponent<Arrow>();
        up.AddComponent<Text>();

        var down = AddEntity(GameEntity.Empty);
        down.Transform.LocalPosition = new Vector2Int(0, -8);
        var downArrow = down.AddComponent<Arrow>();
        downArrow.Direction = true;
        down.AddComponent<Text>();
        
        controller.View = view;
        controller.Discard = discard;
        controller.Log = logWindow;
        controller.PausePrompt = PauseScreen();
        controller.ColorPrompt = colorPrompt.AddComponent<Prompt>();

        controller.UpArrow = upArrow;
        controller.DownArrow = downArrow;

        view.Controller = controller;
    }

    private PausePrompt PauseScreen()
    {
        var pause = GameEntity.Empty;
        pause.Name = "Pause";
        pause.ActiveSelf = false;
        pause.Transform.Depth = 100;
        
        pause.AddChild(SceneElements.Header("Pause"));
        
        pause.AddComponent<Background>().Size = new Vector2Int(Console.WindowWidth, Console.WindowHeight);
        
        var prompt = pause.AddComponent<PausePrompt>();
        
        pause.AddChild(LoadInfo(prompt));

        var yes = SceneElements.Card("Quit", ConsoleColor.White);
        yes.Name = "yesButton";
        yes.Transform.LocalPosition = new Vector2Int(-75, 15);
        var yesButton = yes.AddComponent<Button>();
        yesButton.Graphic = yes.GetComponent<AsciiImage>();
        yesButton.HighlightColor = ConsoleColor.Green;
        yesButton.Action = () =>
        {
            prompt.Hide();
            SceneManager.ChangeScene(typeof(MainMenuScreen));
        };
        pause.AddChild(yes);

        var no = SceneElements.Card("Return", ConsoleColor.White);
        no.Name = "noButton";
        no.Transform.LocalPosition = new Vector2Int(-75, -15);
        var noButton = no.AddComponent<Button>();
        noButton.Graphic = no.GetComponent<AsciiImage>();
        noButton.HighlightColor = ConsoleColor.Red;
        noButton.Action = prompt.Hide;
        noButton.Up = yesButton;
        pause.AddChild(no);

        prompt.First = noButton;
        prompt.MainNav = _nav;

        AddEntity(pause);

        return prompt;
    }

    private GameEntity LoadInfo(PausePrompt p)
    {
        var e = GameEntity.Empty;
        e.Transform.LocalPosition = new Vector2Int(50, -25);
        
        p.Text = e.AddComponent<Text>();
        p.Text.Alignment = ETextAlignment.Left; 
        p.Text.LocalPosition = new Vector2Int(0, 30);
            
        p.Image = e.AddComponent<AsciiImage>();
        p.Image.LocalPosition = new Vector2Int(0, 6);
            
        var box = e.AddComponent<Box>();
        box.Size = new Vector2Int(29, 50);
        box.LocalPosition = new Vector2Int(0, 24);

        return e;
    }

    private GameEntity ColorPrompt()
    {
        var e = GameEntity.Empty;
        e.AddComponent<Text>().Content = "/\\\n\n\n<        >\n\n\n\\/";

        var green = Box(ConsoleColor.Green);
        green.Transform.LocalPosition = new Vector2Int(0, 7);
        e.AddChild(green);

        var blue = Box(ConsoleColor.Blue);
        blue.Transform.LocalPosition = new Vector2Int(-10, 0);
        e.AddChild(blue);

        var yellow = Box(ConsoleColor.Yellow);
        yellow.Transform.LocalPosition = new Vector2Int(10, 0);
        e.AddChild(yellow);
        
        var red = Box(ConsoleColor.Red);
        red.Transform.LocalPosition = new Vector2Int(0, -7);
        e.AddChild(red);
        
        return e;
    }

    private GameEntity Box(ConsoleColor color)
    {
        var e = GameEntity.Empty;
        var background = e.AddComponent<Background>();
        background.Size = new Vector2Int(4, 3);
        background.Color = color;

        var box = e.AddComponent<Box>();
        box.Size = new Vector2Int(6, 5);
        box.Color = color;

        return e;
    }
}