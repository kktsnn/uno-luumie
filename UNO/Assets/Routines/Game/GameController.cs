using LuumieEngine.Classes;
using LuumieEngine.Components;
using LuumieEngine.SceneManagement;
using LuumieEngine.Structs;
using UNO.Assets.Domain;
using UNO.Assets.Repository;
using UNO.Assets.Screens;

namespace UNO.Assets.Routines.Game;

public class GameController : Routine
{
    public static IGameRepository Repository { get; set; } = null!;
    public static GameEngine Engine { get; } = new ();

    public Log Log = null!;
    public DiscardPile Discard = null!;
    public HandView View = null!;
    public Prompt ColorPrompt = null!;
    public PausePrompt PausePrompt = null!;

    public Arrow UpArrow = null!;
    public Arrow DownArrow = null!;

    private UiNavigation _nav = null!;
    
    private readonly Vector2Int[] _seats =
    {
        new(-99, -4),
        new(-89, 11),
        new(-64, 16),
        new(-38, 20),
        new(-13, 22),
        // ==============
        new(13, 22),
        new(38, 20),
        new(64, 16),
        new(89, 11),
        new(99, -4)
    };

    private PlayerIcon[] _activeSeats = null!;

    private bool _playerTurn;
    private bool _penalty;
    
    public static void Load(GameState state)
    {
        Engine.State = state;
    }

    public static void StartNewGame(List<Player> players, GameRules rules)
    {
        Engine.Players = players;
        Engine.State.Rules = rules;
        StartRound();
    }

    public static void StartRound()
        => Engine.StartRound();

    protected override void Awake()
    {
        SceneManager.SceneLoaded += Start;
    }

    private void Start()
    {
        SceneManager.SceneLoaded -= Start;
        if (Engine.RoundOver())
        {
            SceneManager.ChangeScene(typeof(RoundSummaryScreen));
            return;
        }

        _nav = GetComponent<UiNavigation>();
        SeatPlayers();
        Discard.Discard(Engine.VisibleDiscardTod, new Vector2Int());
        TurnStart();
    }

    private void SeatPlayers()
    {
        var playerSeats = GameEntity.Empty;
        GameEntity.AddChild(playerSeats);

        _activeSeats = new PlayerIcon[Engine.Players.Count];
        
        var skip = Engine.Players.Count == 1 ? 0 : 9 / (Engine.Players.Count - 1);
        var seatIdx = 0;
        var activeSeatIdx = 0;
        foreach (var p in Engine.Players)
        {
            var seat = GameEntity.Empty;
            seat.Transform.LocalPosition = _seats[seatIdx];
            seatIdx += skip;

            seat.AddComponent<Text>();
            var icon = seat.AddComponent<PlayerIcon>();
            icon.Player = p;
            playerSeats.AddChild(seat);
            
            _activeSeats[activeSeatIdx++] = icon;
        }
    }

    private void TurnStart()
    {
        Engine.NextTurn();
        
        _choosing = false;
        _card = null;
        if (Engine.DiscardToD.Type is not ECardType.Skip) 
            _activeSeats[Engine.ActivePlayerIndex].Uno(false);
        
        _activeSeats[Engine.ActivePlayerIndex].Highlight(true);
        
        View.LoadHand(Engine.ActivePlayer, false);
        
        _playerTurn = Engine.ActivePlayer.Type is EPlayerType.User;

        if (_playerTurn) UiNavigation.Active = true;
        else StartCoroutine(BotTurn());
    }

    private IEnumerator<int> BotTurn()
    {
        UiNavigation.Active = false;
        yield return 10;
        if (!CheckToD()) yield break;
        
        var c = Engine.ActivePlayer.ChooseCard(Engine.VisibleDiscardTod);
        if (c == null) StartCoroutine(Draw(1, false));
        else CheckCard(c);
    }
    
    private void TurnEnd()
    {
        if (Engine.ActivePlayer.Hand.Cards.Count == 1 && Engine.ActivePlayer.Uno == false)
        {
            Log.AddLine($"{Engine.ActivePlayer.Name} draws UNO penalty");
            Engine.ActivePlayer.CardDrawn = false;
            StartCoroutine(Draw(2, true));
            return;
        }
        
        _activeSeats[Engine.ActivePlayerIndex].Highlight(false);
        Repository.Save(Engine.State);
        
        if (Engine.RoundOver())
        {
            if (Engine.MatchOver()) Repository.Delete(Engine.State.GameId);
            SceneManager.ChangeScene(typeof(RoundSummaryScreen));
        }
        else TurnStart();
    }

    // ==================================
    
    private bool CheckToD()
    {
        if (!_penalty) return true;
        switch (Engine.DiscardToD.Type)
        {
            case ECardType.Skip:
                StartCoroutine(Skip());
                break;
            case ECardType.Draw2:
                StartCoroutine(Draw(2, true));
                break;
            case ECardType.Draw4:
                StartCoroutine(Draw(4, true));
                break;
        }
        return _penalty = false;
    }

    private Card? _card;
    
    public bool CheckCard(Card c)
    {
        if (!Engine.ValidateCard(c)) return false;

        _card = c;
        _nav.Current = null;
        
        if (!_playerTurn && Engine.ActivePlayer.Hand.Cards.Count == 1) 
            if (Engine.ActivePlayer.ChooseUno()) CallUno();
        
        if (_card.Type is ECardType.Draw2 or ECardType.Draw4 or ECardType.Skip) _penalty = true;
        if (_card.Color is ECardColor.Wild)
        {
            if (_playerTurn) ColorPrompt.Show();
            else StartCoroutine(PlayCard(_card, Engine.ActivePlayer.ChooseColor()));
        }
        else StartCoroutine(PlayCard(_card));
        return true;
    }

    private void CallUno()
    {
        Log.AddLine($"{Engine.ActivePlayer.Name} calls UNO");
        _activeSeats[Engine.ActivePlayerIndex].Uno(true);
    }

    private IEnumerator<int> PlayCard(Card c, ECardColor? color = null)
    {
        Discard.Discard(color == null ? c : new Card(color.Value, c.Type), View.FindCardPosition(c));
        Engine.PlayCard(c, color);
        Log.AddLine($"{Engine.ActivePlayer.Name} played {c}");
        if (color != null) Log.AddLine($"{Engine.ActivePlayer.Name} chose color {color}");
        _activeSeats[Engine.ActivePlayerIndex].UpdatePlayer();

        if (c.Type is ECardType.Reverse)
        {
            UpArrow.FlipDirection();
            DownArrow.FlipDirection();
        }
        
        yield return 10;
        
        TurnEnd();
    }

    private bool _choosing;
    
    protected override void OnKeyPress(ConsoleKeyInfo keyInfo)
    {
        
        if (keyInfo.Key is ConsoleKey.Escape)
        {
            if (!PausePrompt.Enabled) PausePrompt.Show(Engine.State);
            else PausePrompt.Hide();
            return;
        }

        if (PausePrompt.Enabled || !_playerTurn) return;
        
        // turn cards (TODO prompt)
        if (keyInfo.Key is ConsoleKey.Spacebar && !_choosing)
        {
            _choosing = true;

            if (CheckToD()) View.LoadHand(Engine.ActivePlayer, true);
            
            return;
        }

        if (!_choosing) return;

        // draw
        if (keyInfo.Key is ConsoleKey.Z && !_drawing)
        {
            if (Engine.ActivePlayer.CardDrawn) TurnEnd();
            else
            {
                StartCoroutine(Draw(1, false));
                if (!Engine.State.Rules.DrawToMatch) Engine.ActivePlayer.CardDrawn = true;
            }
            return;
        }

        // call uno
        if (keyInfo.Key is ConsoleKey.X && Engine.ActivePlayer.Hand.Cards.Count <= 2)
        {
            Engine.ActivePlayer.Uno = true;
            CallUno();
            return;
        }
        
        // color prompt
        if (!ColorPrompt.Enabled) return;

        ECardColor color;
        switch (keyInfo.Key)
        {
            case ConsoleKey.A or ConsoleKey.LeftArrow:
                color = ECardColor.Blue;
                break;
            case ConsoleKey.S or ConsoleKey.DownArrow:
                color = ECardColor.Red;
                break;
            case ConsoleKey.D or ConsoleKey.RightArrow:
                color = ECardColor.Yellow;
                break;
            case ConsoleKey.W or ConsoleKey.UpArrow:
                color = ECardColor.Green;
                break;
            default:
                return;
        }

        StartCoroutine(PlayCard(_card!, color));
        ColorPrompt.Hide();
    }

    private bool _drawing;

    private IEnumerator<int> Draw(int amount, bool endTurn)
    {
        if (Engine.ActivePlayer.CardDrawn)
        {
            TurnEnd();
            yield break;
        }
        
        yield return 1;
        
        _drawing = true;
        Log.AddLine($"{Engine.ActivePlayer.Name} draws {(amount == 1 ? "a" : amount)} card{(amount == 1 ? "" : "s")}");
        
        for (var _ = 0; _ < amount; _++)
        {
            _card = Engine.Draw();
            Engine.AddCardToPlayer(_card);
            View.LoadHand(Engine.ActivePlayer, _playerTurn, _card);
            _activeSeats[Engine.ActivePlayerIndex].UpdatePlayer();
            yield return 5;
        }
        
        _drawing = false;
        
        if (endTurn)
        {
            TurnEnd();
            yield break;
        }
        
        if (!Engine.State.Rules.DrawToMatch || Engine.ValidateCard(_card)) Engine.ActivePlayer.CardDrawn = true;
        
        // TODO Rewrite this mess
        // TODO Block input (spam breaks)
        if ((Engine.State.Rules.ForcePlay || !_playerTurn) && !CheckCard(_card!)) 
            if (!_playerTurn) StartCoroutine(Draw(1, false));
        else if (!_playerTurn) TurnEnd();
    }

    private IEnumerator<int> Skip()
    {
        // yield return 1;
        View.Skipped();
        yield return 10;
        TurnEnd();
    }
}