using LuumieEngine.Classes;
using LuumieEngine.Components;
using LuumieEngine.Structs;
using UNO.Assets.Domain;

namespace UNO.Assets.Routines;

// TODO add reset?
// TODO add info
public class SettingsView : Routine
{
    public GameRules Rules => new(
        Points,
        Cards,
        _stacking.Active,
        _sevenZero.Active,
        _forcePlay.Active,
        _noBluffing.Active,
        _drawToMatch.Active);
    private int Points => 
        _limit.Current == "Custom" && int.TryParse(_customLimit.Current, out var custom) 
            ? custom : int.TryParse(_limit.Current, out var limit) ? limit : 0;
    private int Cards => int.TryParse(_cardAmount.Current, out var amount) ? amount : 7;

    public Button PlayButton = null!;
    public UiRoutine First => _limit;
    
    private Dropdown _limit = null!;
    private InputField _customLimit = null!;
    private InputField _cardAmount = null!;
    private Toggle _stacking = null!;
    private Toggle _sevenZero = null!;
    private Toggle _forcePlay = null!;
    private Toggle _noBluffing = null!;
    private Toggle _drawToMatch = null!;
    
    protected override void Awake()
    {
        AddEntities();
    }

    private void AddEntities()
    {
        var limit = GameEntity.Empty;
        limit.Transform.LocalPosition = new Vector2Int(8, 15);
        _limit = AddColors(limit, limit.AddComponent<Dropdown>(), "Score Limit");
        _limit.Options = new[] { "One Round", "300", "500", "900", "Custom" };
        
        
        var customLimit = GameEntity.Empty;
        customLimit.Transform.LocalPosition = new Vector2Int(8, 13);
        customLimit.ActiveSelf = false;
        _customLimit = AddColors(customLimit, customLimit.AddComponent<InputField>(), "Custom Limit");
        _customLimit.Placeholder = customLimit.AddComponent<Text>();
        _customLimit.Placeholder.Content = "0";
        _customLimit.Up = _limit;
        _customLimit.Right = PlayButton;


        var cardAmount = GameEntity.Empty;
        cardAmount.Transform.LocalPosition = new Vector2Int(8, 11);
        _cardAmount = AddColors(cardAmount, cardAmount.AddComponent<InputField>(), "Card Amount");
        _cardAmount.Placeholder = cardAmount.AddComponent<Text>();
        _cardAmount.Placeholder.Content = "7";
        _cardAmount.Up = _limit;
        _cardAmount.Right = PlayButton;
        

        _limit.Confirmed += () =>
        {
            if (_limit.Current == "Custom")
            {
                customLimit.ActiveSelf = true;
                _customLimit.Up = _limit;
                _cardAmount.Up = _customLimit;
                return;
            }
            customLimit.ActiveSelf = false;
            _cardAmount.Up = _limit;
        };
        

        var rules = GameEntity.Empty;
        rules.Transform.LocalPosition = new Vector2Int(8, 7);
        var rulesText = rules.AddComponent<Text>();
        rulesText.LocalPosition = new Vector2Int(-22, 0);
        rulesText.Content = "Rules:";
        
        GameEntity.AddChild(rules);

        var stacking = GameEntity.Empty;
        stacking.Transform.LocalPosition = new Vector2Int(8, 4);
        _stacking = AddColors(stacking, stacking.AddComponent<Toggle>(), "Stacking  ");
        OnToggle(_stacking, false);
        _stacking.Up = _cardAmount;
        _stacking.Right = PlayButton;
        // DISABLED FOR NOW
        _stacking.EnabledSelf = false;
        
        
        var sevenZero = GameEntity.Empty;
        sevenZero.Transform.LocalPosition = new Vector2Int(8, 1);
        _sevenZero = AddColors(sevenZero, sevenZero.AddComponent<Toggle>(), "Seven/Zero");
        OnToggle(_sevenZero, false);
        _sevenZero.Up = _stacking;
        _sevenZero.Right = PlayButton;
        // DISABLED FOR NOW
        _sevenZero.EnabledSelf = false;

        
        var forcePlay = GameEntity.Empty;
        forcePlay.Transform.LocalPosition = new Vector2Int(8, -2);
        _forcePlay = AddColors(forcePlay, forcePlay.AddComponent<Toggle>(), "Force Play");
        OnToggle(_forcePlay, false);
        _forcePlay.Up = _sevenZero;
        _forcePlay.Right = PlayButton;
        
        
        var noBluffing = GameEntity.Empty;
        noBluffing.Transform.LocalPosition = new Vector2Int(8, -5);
        _noBluffing = AddColors(noBluffing, noBluffing.AddComponent<Toggle>(), "No Bluffing");
        OnToggle(_noBluffing, false);
        _noBluffing.Up = _forcePlay;
        _noBluffing.Right = PlayButton;
        // DISABLED FOR NOW
        _noBluffing.EnabledSelf = false;
        
        var drawToMatch = GameEntity.Empty;
        drawToMatch.Transform.LocalPosition = new Vector2Int(8, -8);
        _drawToMatch = AddColors(drawToMatch, drawToMatch.AddComponent<Toggle>(), "Draw To Match");
        OnToggle(_drawToMatch, false);
        _drawToMatch.Up = _noBluffing;
        _drawToMatch.Right = PlayButton;
        
        _limit.Right = PlayButton;
    }

    private T AddColors<T>(GameEntity e, T r, string c) where T : UiRoutine
    {
        r.Content = e.AddComponent<Text>();
        r.ClickColor = ConsoleColor.Cyan;
        
        var text = e.AddComponent<Text>();
        text.LocalPosition = new Vector2Int(c is "Draw To Match" or "Custom Limit" ? -19 : -20, 0);
        text.Content = c;
        
        r.Graphic = text;
        r.HighlightColor = ConsoleColor.DarkCyan;
        
        GameEntity.AddChild(e);
        
        return r;
    }

    private static void OnToggle(Toggle t, bool a)
    {
        t.Active = !a;
        t.UpdateActiveStatus();
        t.OnClicked += () =>
        {
            t.GetComponent<Text>().Color = !t.Active ? ConsoleColor.Green : ConsoleColor.Red;
        };
        t.OnClick();
    }
    
    public void PlayersChanged(UiRoutine r)
    {
        _customLimit.Left = r;
        _cardAmount.Left = r;
        _stacking.Left = r;
        _sevenZero.Left = r;
        _forcePlay.Left = r;
        _noBluffing.Left = r;
        _drawToMatch.Left = r;
        
        _limit.Left = r;
    }
}