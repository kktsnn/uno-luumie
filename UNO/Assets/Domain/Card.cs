using System.Text.Json.Serialization;

namespace UNO.Assets.Domain;

[Serializable]
public class Card : IComparable<Card>
{
    [JsonPropertyName("color")]
    public ECardColor Color { get; set; }

    [JsonIgnore]
    public ConsoleColor ConsoleColor => Color switch
    {
        ECardColor.Red => ConsoleColor.Red,
        ECardColor.Green => ConsoleColor.Green,
        ECardColor.Blue => ConsoleColor.Blue,
        ECardColor.Yellow => ConsoleColor.Yellow,
        ECardColor.Wild => ConsoleColor.White,
        _ => throw new ArgumentOutOfRangeException()
    };
    [JsonPropertyName("type")]
    public ECardType Type { get; set; }
    [JsonIgnore]
    public string TypeString => Type switch
    {
        ECardType.Draw4 => "+4",
        ECardType.Draw2 => "+2",
        ECardType.Reverse => "R",
        ECardType.Skip => "S",
        ECardType.Wild => "W",
        ECardType.Zero => "0",
        ECardType.One => "1",
        ECardType.Two => "2",
        ECardType.Three => "3",
        ECardType.Four => "4",
        ECardType.Five => "5",
        ECardType.Six => "6",
        ECardType.Seven => "7",
        ECardType.Eight => "8",
        ECardType.Nine => "9",
        _ => throw new ArgumentOutOfRangeException()
    };

    [JsonIgnore]
    public int Value
    {
        get
        {
            if (Color is ECardColor.Wild)
                return 50;
            if (Type is ECardType.Draw2 or ECardType.Skip or ECardType.Reverse)
                return 20;
            return (int)Type;
        }
    }

    public List<string> Log = new ();

    [JsonConstructor]
    public Card(ECardColor color, ECardType type)
    {
        Color = color;
        Type = type;
    }

    public int CompareTo(Card? other)
    {
        if (other == null)
        {
            return -1;
        }

        if (Color != other.Color) return Color < other.Color ? -1 : 1;
        return Type <= other.Type ? -1 : 1;
    }

    public override string ToString()
    {
        return $"{Color} {Type}";
    }

    public bool CanStack(Card tod)
    {
        return Color is ECardColor.Wild || Color == tod.Color || Type == tod.Type;
    }
}