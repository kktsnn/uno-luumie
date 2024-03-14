using LuumieEngine.Classes;
using LuumieEngine.Components;
using LuumieEngine.Structs;
using UNO.Assets.Domain;

namespace UNO.Assets.Routines;

// TODO add info box (Edit and Delete);
public class PlayersView : Routine
{
    public List<Player> Players { get; } = new();
    public UiNavigation MainNav = null!;
    public SettingsView SView = null!;

    private GameEntity _addPrompt = null!;
    private GameEntity _addButton = null!;
    
    private int _current;
    private int _tab;

    protected override void Awake()
    {
        ShowPlayers();
        
        _addButton.GetComponent<Button>().OnSelect();
        
        MainNav.Next += Moved;
        MainNav.Current = _addButton.GetComponent<Button>();
    }

    private void Moved(EDirection dir)
    {
        switch (dir)
        {
            case EDirection.Up:
                _current--;
                break;
            case EDirection.Down:
                _current++;
                break;
            case EDirection.Left:
                _current = 0;
                _tab--;
                break;
            case EDirection.Right:
                _tab++;
                break;
            default:
                return;
        }
    }

    private void InitAddButton()
    {
        _addPrompt = GameEntity.Empty;
        _addPrompt.Transform.Depth = 2;
        
        var promptBox = _addPrompt.AddComponent<Box>();
        promptBox.Size = new Vector2Int(43, 3);
        promptBox.Color = ConsoleColor.DarkGreen;
        
        var nameText = _addPrompt.AddComponent<Text>();
        nameText.Content = "Name:";
        nameText.LocalPosition = new Vector2Int(-3, 0);
        nameText.EnabledSelf = false;
        
        var name = _addPrompt.AddComponent<InputField>();
        name.Graphic = nameText;
        name.HighlightColor = ConsoleColor.Green;
        name.Content = _addPrompt.AddComponent<Text>();
        name.Content.Alignment = ETextAlignment.Left;
        name.ClickColor = ConsoleColor.DarkGray;
        name.Content.LocalPosition = new Vector2Int(11, 0);
        name.EnabledSelf = false;
        
        var typeText = _addPrompt.AddComponent<Text>();
        typeText.Content = "Type:";
        typeText.LocalPosition = new Vector2Int(-17, 0);
        
        var type = _addPrompt.AddComponent<Dropdown>();
        type.Graphic = typeText;
        type.HighlightColor = ConsoleColor.Green;
        type.Content = type.AddComponent<Text>();
        type.Options = new[] { "USER", "BOT" };
        type.Content.LocalPosition = new Vector2Int(-10, 0);
        type.ClickColor = ConsoleColor.DarkGray;
        
        type.Confirmed += () =>
        {
            if (type.Current == "USER")
            {
                nameText.EnabledSelf = true;
                name.EnabledSelf = true;
                name.OnSelect();
                name.OnClick();
            }
            else
            {
                Players.Add(new Player(EPlayerType.Bot, Player.GenerateBotName()));
                ResetPrompt(name, type);
            }
            type.Select = 0;
        };
        type.Canceled += () =>
        {
            ResetPrompt(name, type);
        };
        
        _addPrompt.ActiveSelf = false;
        
        
        _addButton = GameEntity.Empty;
        _addButton.Name = "ADD BUTTON";
        var addText = _addButton.AddComponent<Text>();
        addText.Content = "Add Player";
        
        var aBox = _addButton.AddComponent<Box>();
        aBox.Size = new Vector2Int(43, 3);
        aBox.Color = ConsoleColor.DarkGreen;
        
        var aButton = _addButton.AddComponent<Button>();
        aButton.Right = SView.First;
        aButton.Graphic = addText;
        aButton.HighlightColor = ConsoleColor.Green;
        aButton.Action = () =>
        {
            _addButton.ActiveSelf = false;
            _addPrompt.ActiveSelf = true;
            aButton.OnDeselect();
            type.OnSelect();
            type.OnClick();
        };
        
        name.Confirmed += () =>
        {
            if (name.Content.Content != "") Players.Add(new Player(EPlayerType.User, name.Content.Content));
            ResetPrompt(name, type);
        };
        name.Canceled += () =>
        {
            ResetPrompt(name, type);
        };
    }

    private void ResetPrompt(InputField name, Dropdown type)
    {
        UiNavigation.Active = true;
        _addButton.ActiveSelf = true;
        _addPrompt.ActiveSelf = false;
        name.Current = "";
        name.EnabledSelf = false;
        name.Graphic!.EnabledSelf = false;
        type.Select = 0;
        type.OnDeselect();
        ShowPlayers(Players.Count);
    }

    private void ShowPlayers(int selected = 0)
    {
        GameEntity.ClearChildren();

        var offset = 15;

        UiRoutine? last = null;

        var idx = 0;
        foreach (var p in Players)
        {
            var e = GameEntity.InputField;
            e.Transform.LocalPosition = new Vector2Int(0, offset);
            var box = e.GetComponent<Box>();
            box.Size = new Vector2Int(43, 3);
            var field = e.GetComponent<InputField>();
            field.Graphic = box;
            field.Placeholder = null;
            field.HighlightColor = ConsoleColor.DarkCyan;
            field.ClickColor = ConsoleColor.Cyan;
            field.Current = p.Name;
            field.Limit = 40;
            field.Right = SView.First;
            GameEntity.AddChild(e);

            if (idx == selected) MainNav.Current = field;
            field.Up = last;
            last = field;
            
            offset -= 3;
            idx++;
        }

        if (Players.Count >= 10) return;
        
        InitAddButton();
        
        _addButton.Transform.LocalPosition = new Vector2Int(0, offset);
        GameEntity.AddChild(_addButton);
        
        _addPrompt.Transform.LocalPosition = new Vector2Int(0, offset);
        GameEntity.AddChild(_addPrompt);
        
        if (idx == selected) MainNav.Current = _addButton.GetComponent<Button>();
        _addButton.GetComponent<Button>().Up = last;

        _current = selected;
        
        SView.PlayersChanged(Transform[0].GetComponent<UiRoutine>());
    }
    
    protected override void OnKeyPress(ConsoleKeyInfo keyInfo)
    {
        if (_tab != 0) return;

        if (keyInfo.Key is ConsoleKey.Delete && _current < Players.Count)
        {
            Players.RemoveAt(_current);
            if (_current == Players.Count && _current != 0) _current--;
            ShowPlayers(_current);
        }
    }
}