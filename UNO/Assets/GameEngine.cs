using UNO.Assets.Domain;

namespace UNO.Assets;

// TODO add game log
public class GameEngine
{
    public GameState State { get; set; } = new ();

    public int PlayerCount => State.Players.Count;

    public int ActivePlayerIndex
        => CorrectIndex(State.ActivePlayerCounter % PlayerCount);

    public int NextPlayerIndex
        => CorrectIndex((ActivePlayerIndex + (State.Clockwise ? 1 : -1)) % PlayerCount);
    
    public int PreviousPlayerIndex
        => CorrectIndex((ActivePlayerIndex - (State.Clockwise ? 1 : -1)) % PlayerCount);
    
    public Player PreviousPlayer
        => Players[PreviousPlayerIndex];
    public Player ActivePlayer 
        => Players[ActivePlayerIndex];
    public Player NextPlayer
        => Players[NextPlayerIndex];
    
    public ECardColor ActiveColor
    {
        get => State.DiscardPile.ToD().Color is ECardColor.Wild ? State.ActiveColor : State.DiscardPile.ToD().Color;
        set => State.ActiveColor = value;
    }
    public Card DiscardToD => State.DiscardPile.ToD();
    public Card VisibleDiscardTod => new (ActiveColor, DiscardToD.Type);

    public List<Player> Players
    {
        get => State.Players;
        set => State.Players = value;
    }

    private int CorrectIndex(int i)
    {
        return i + (i < 0 ? PlayerCount : 0);
    }

    public GameEngine()
    {
        State.Seed = Random.Shared.Next();
    }

    public void StartRound()
    {
        State.TurnCounter = 1;
        State.RoundCounter++;
        ClearPlayerHands();
        DealCards();
        // TODO let player choose
        if (DiscardToD.Color is ECardColor.Wild)
            ActiveColor = Enum.GetValues<ECardColor>()[new Random(State.Seed).Next(5)];
    }

    public void DealCards()
    {
        State.ActivePlayerCounter = State.Seed % PlayerCount;
        
        State.DrawPile.Initialize();
        State.DrawPile.Shuffle(State.Seed);
        
        State.DiscardPile.Clear();
        
        for (var i = 0; i < State.Rules.CardAmount; i++)
        {
            foreach (var p in State.Players)
            {
                p.Add(State.DrawPile.Draw());
            }
        }

        foreach (var p in State.Players)
        {
            p.Hand.Sort();
        }


        var tod = State.DrawPile.Draw();

        while (tod.Type is ECardType.Draw4)
        {
            State.DrawPile.Add(tod);
            State.DrawPile.Shuffle(State.Seed);
            tod = State.DrawPile.Draw();
        }
        
        State.DiscardPile.Add(tod);
    }

    public void Reshuffle()
    {
        var tod = State.DiscardPile.Draw();
        State.DrawPile.AddRange(State.DiscardPile.Cards);
        State.DiscardPile.Clear();
        State.DiscardPile.Add(tod);
    }

    public void PlayCard(Card c, ECardColor? color = null)
    {
        ActivePlayer.Hand.Cards.Remove(c);
        State.DiscardPile.Add(c);
        if (c.Type is ECardType.Reverse) Reverse();
        if (color != null) ActiveColor = color.Value;
        if (c.Type is ECardType.Draw2 or ECardType.Draw4 or ECardType.Skip or ECardType.Reverse)
            State.PenaltyDrawn = false;
    }

    public bool ValidateCard(Card? c)
    {
        return c != null && c.CanStack(VisibleDiscardTod);
    }

    public Card Draw()
    {
        if (State.DrawPile.IsEmpty()) Reshuffle();

        var c = State.DrawPile.Draw();
        return c;
    }

    public void AddCardToPlayer(Card c)
    {
        ActivePlayer.Add(c);
    }

    public bool Reverse()
    {
        return State.Clockwise = !State.Clockwise;
    }

    public void NextTurn()
    {
        IncrementPlayerCounter();

        ActivePlayer.CardDrawn = false;
            
        if (DiscardToD.Type is not ECardType.Skip && State.PenaltyDrawn) ActivePlayer.Uno = false;
    }

    public void MakeBotMove()
    {
        // TODO Check if this truly works
        while (true)
        {
            var c = ActivePlayer.ChooseCard(VisibleDiscardTod) ?? Draw();
            
            if (ValidateCard(c))
            {
                ECardColor? color = null;
                if (c.Type is ECardType.Wild) color = ActivePlayer.ChooseColor();
                PlayCard(c, color);
                break;
            }
            
            AddCardToPlayer(c);
            if (!State.Rules.DrawToMatch) break;
        }
    }

    public void IncrementPlayerCounter()
    {
        State.TurnCounter++;
        if (State.Clockwise)
            State.ActivePlayerCounter++;
        else
            State.ActivePlayerCounter--;
    }

    public bool RoundOver()
    {
        return Players.Any(p => p.Hand.Cards.Count == 0);
    }

    public void AddRoundPointsToActivePlayer()
    {
        ActivePlayer.Points += RoundPoints();
    }

    public int RoundPoints()
    {
        return Players.Sum(n => n.Hand.HandValue());
    }

    public bool MatchOver()
    {
        return Players.Any(p => p.Points >= State.Rules.Points);
    }

    public void ClearPlayerHands()
    {
        Players.ForEach(p => p.Hand.Cards.Clear());
    }

    public bool CheckPenalty()
    {
        if (State.PenaltyDrawn) return false;
        
        switch (DiscardToD.Type)
        {
            case ECardType.Draw2:
                AddCardToPlayer(Draw());
                AddCardToPlayer(Draw());
                break;
            case ECardType.Draw4:
                AddCardToPlayer(Draw());
                AddCardToPlayer(Draw());
                AddCardToPlayer(Draw());
                AddCardToPlayer(Draw());
                break;
            case ECardType.Skip:
                break;
            case ECardType.Reverse:
                if (PlayerCount == 2) break;
                return false;
            default:
                return false;
        }

        return State.PenaltyDrawn = true;
    }
}