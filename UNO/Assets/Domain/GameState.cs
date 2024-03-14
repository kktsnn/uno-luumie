namespace UNO.Assets.Domain;

public class GameState
{
    public Guid GameId { get; set; } = Guid.NewGuid();
    public int Seed { get; set; } = Random.Shared.Next();
    
    public List<Player> Players { get; set; } = new();
    
    public GameRules Rules { get; set; } = new();
    
    public Deck DrawPile { get; set; } = new();
    public Deck DiscardPile { get; set; } = new();
    public int ActivePlayerCounter  { get; set; }
    public int TurnCounter { get; set; } = 1;
    public int RoundCounter { get; set; }
    public bool Clockwise { get; set; } = true;
    public ECardColor ActiveColor { get; set; }
    public bool PenaltyDrawn { get; set; }
    public bool BotPlaying { get; set; }

    public string PrettyString()
    {
        // Triple quotes have notes at the end of lines for some reason.
        return
            $"{new string('=', 21)}\n" +
            $"Round: {RoundCounter}\n" +
            $"Turn: {TurnCounter}\n" +
            $"{new string('=', 21)}\n" +
            $"Players:\n" +
            $"{string.Join('\n', Players.Select(n => 
                $"{n.Name} - \nCards: {n.Hand.Cards.Count}, Points: {n.Points}")
            )}\n" +
            $"{new string('\n', 20 - Players.Count * 2)}" +
            $"{new string('=', 21)}\n" +
            $"Rules:\n" +
            $"{Rules.PrettyString()}";
    }
}