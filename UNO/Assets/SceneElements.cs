using LuumieEngine.Classes;
using LuumieEngine.Components;
using LuumieEngine.Structs;
using UNO.Assets.Domain;
using UNO.Assets.Routines;

namespace UNO.Assets;

public static class SceneElements
{
    public const string FilesDir = "../../../../UNO/Assets/ASCII/";

    private static GameEntity Image(string path, ConsoleColor color = ConsoleColor.White)
    {
        var e = GameEntity.Empty;
        var c = e.AddComponent<AsciiImage>();
        c.Path = path;
        c.Color = color;
        return e;
    }

    public static GameEntity Logo(string name)
    {
        return Image($"{FilesDir}Logos/{name}.txt");
    }

    public static GameEntity BigText(string name)
    {
        return Image($"{FilesDir}Text/{name}.txt");
    }

    public static GameEntity Card(string? name, ConsoleColor color)
    {
        name ??= "Unknown";
        return Image($"{FilesDir}Cards/{name}.txt", color);
    }

    public static GameEntity Card(Card card)
    {
        return Image($"{FilesDir}Cards/Deck/{card.Type.ToString()}.txt", card.ConsoleColor);
    }

    public static GameEntity Framerate()
    {
        var fps = GameEntity.Empty;
        fps.Name = "Framerate";
        fps.Transform.LocalPosition = new Vector2Int(
            Console.WindowWidth / 2 - 3, 
            Console.WindowHeight / 2 - 1);
        
        // Disabled for now, check if this unloads correctly
        // Listener.WindowSizeChanged += () =>
        // {
        //     fps.Transform.LocalPosition = new Vector2Int(
        //         Console.WindowWidth / 2 - 3, 
        //         Console.WindowHeight / 2 - 1);
        // };
        
        fps.AddComponent<Text>();
        fps.AddComponent<Framerate>();
        
        return fps;
    }

    public static GameEntity EntityCount()
    {
        var count = GameEntity.Empty;
        count.Name = "EntityCount";
        count.Transform.LocalPosition = new Vector2Int(
            Console.WindowWidth / 2 - 3, 
            Console.WindowHeight / 2 - 3);
        
        // Disabled for now, check if this unloads correctly
        // Listener.WindowSizeChanged += () =>
        // {
        //     count.Transform.LocalPosition = new Vector2Int(
        //         Console.WindowWidth / 2 - 3, 
        //         Console.WindowHeight / 2 - 3);
        // };
        
        count.AddComponent<Text>();
        count.AddComponent<EntityCount>();

        return count;
    }

    public static GameEntity Header(string text, string? subtext = null)
    {
        var e = GameEntity.Empty;
        e.Name = "Header";
        e.Transform.LocalPosition = new Vector2Int(
            -Console.WindowWidth / 2 + 14, 
            Console.WindowHeight / 2 - 6);
        
        // Disabled for now, check if this unloads correctly
        // Listener.WindowSizeChanged += () =>
        // {
        //     if (e.Transform != null) e.Transform.LocalPosition = new Vector2Int(
        //         -Console.WindowWidth / 2 + 14,
        //         Console.WindowHeight / 2 - 6);
        // };
        
        e.AddChild(Logo("LogoMedium"));
        
        var main = BigText(text);
        var img = main.GetComponent<AsciiImage>();
        main.Transform.LocalPosition = new Vector2Int(img.Size.X / 2 + 15, 3);
        
        e.AddChild(main);

        if (subtext == null) return e;
        
        var sub = BigText(subtext);
        img = sub.GetComponent<AsciiImage>();
        sub.Transform.LocalPosition = new Vector2Int(img.Size.X / 2 + 15, -2);
        
        e.AddChild(sub);
        
        return e;
    }

    public static GameEntity ConfirmationPrompt(UiNavigation? mainNav, string message, Action action)
    {
        var pane = GameEntity.Empty;
        pane.Name = "ConfirmationPane";
        pane.Transform.LocalPosition = new Vector2Int(0, -5);
        pane.ActiveSelf = false;
        pane.Transform.Depth = 8;
        var background = pane.AddComponent<Background>();
        background.Size = new Vector2Int(120, 30);
        var prompt = pane.AddComponent<Prompt>();
        prompt.MainNav = mainNav;

        var box = GameEntity.Empty;
        var b = box.AddComponent<Box>();
        b.Size = new Vector2Int(120, 30);
        b.Color = ConsoleColor.DarkGray;
        pane.AddChild(box);
        
        var t = GameEntity.Empty;
        t.Transform.LocalPosition = new Vector2Int(0, 10);
        var text = t.AddComponent<Text>();
        text.Content = message;
        pane.AddChild(t);

        var yes = Card("Yes", ConsoleColor.White);
        yes.Name = "yesButton";
        yes.Transform.LocalPosition = new Vector2Int(40, -7);
        var yesButton = yes.AddComponent<Button>();
        yesButton.Graphic = yes.GetComponent<AsciiImage>();
        yesButton.HighlightColor = ConsoleColor.Green;
        yesButton.Action = () =>
        {
            prompt.Hide();
            action();
        };
        pane.AddChild(yes);

        var no = Card("No", ConsoleColor.White);
        no.Name = "noButton";
        no.Transform.LocalPosition = new Vector2Int(-40, -7);
        var noButton = no.AddComponent<Button>();
        noButton.Graphic = no.GetComponent<AsciiImage>();
        noButton.HighlightColor = ConsoleColor.Red;
        noButton.Action = prompt.Hide;
        noButton.Right = yesButton;
        pane.AddChild(no);

        prompt.First = noButton;

        return pane;
    }
}