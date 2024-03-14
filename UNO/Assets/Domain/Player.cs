using System.Text.Json.Serialization;

namespace UNO.Assets.Domain;

public class Player
{
    public EPlayerType Type { get; }
    public string Name { get; }
    public int Points { get; set; }
    public Hand Hand { get; set; } = new();
    public bool Uno { get; set; }
    public bool CardDrawn { get; set; }

    public Player(EPlayerType type, string name)
    {
        Type = type;
        Name = name;
    }
    
    [JsonConstructor]
    protected Player(EPlayerType type, string name, int points, Hand hand) : this(type, name)
    {
        Points = points;
        Hand = hand;
    }

    public int Add(Card card)
    {
        return Hand.Add(card);
    }

    public ECardColor ChooseColor()
    {
        return Enum.GetValues<ECardColor>().ToList().MaxBy(c => Hand.ColorCount(c));
    }

    public Card? ChooseCard(Card tod)
    {
        var card = Hand.Cards.FindAll(c => c.Type == tod.Type || c.Color == tod.Color).MaxBy(c => c.Value);
        
        card ??= Hand.Cards.Find(c => c.Color is ECardColor.Wild);

        // if (card != null) Hand.Cards.Remove(card);

        return card;
    }

    public bool ChooseUno()
    {
        return Uno = Random.Shared.Next(100) <= 70;
    }
    
    public static string GenerateBotName()
    {
        string[] fname = { "Neo", "Alexa", "Byte", "Cypher", "Sprocket", "Echo", "Spark", "Widget", "Roxy", "Digi" };
        string[] lname = { "Sparx", "Robo", "Circuit", "Techton", "Mech", "Automaton", "Bitstream", "Algorithm", "Nanobot", "Cybernetix" };
        return $"BOT - {fname[Random.Shared.Next(10)]} {lname[Random.Shared.Next(10)]}";
    }
}