namespace UNO.Assets.Domain;

public class Deck
{
    public List<Card> Cards { get; set; } = new();

    public void Initialize()
    {
        foreach (var color in Enum.GetValues<ECardColor>())
        {
            if (color == ECardColor.Wild) break;
            for (var i = 0; i < 2; i++)
            {
                foreach (var value in Enum.GetValues<ECardType>())
                {
                    if (value == ECardType.Wild) break;
                    if (value == ECardType.Zero && i == 1) continue;
                    Cards.Add(new Card(color, value));
                }
            }
        }
        for (var _ = 0; _ < 4; _++)
        {
            Cards.Add(new Card(ECardColor.Wild, ECardType.Wild));
            Cards.Add(new Card(ECardColor.Wild, ECardType.Draw4));
        }
    }

    public void Shuffle(int seed)
    {
        var rng = new Random(seed);
        Cards = new List<Card>(Cards.OrderBy(v => rng.Next()));
    }

    public Card Draw()
    {
        var c = Cards[^1];
        Cards.RemoveAt(Cards.Count - 1);
        return c;
    }
    
    public List<Card> DrawRange(int amount)
    {
        List<Card> cards = new();
        for (int i = 0; i < amount; i++)
        {
            cards.Add(Draw());
        }

        return cards;
    }
    
    public bool IsEmpty() 
        => Cards.Count == 0;

    public void Add(Card card)
        => Cards.Add(card);

    public void AddRange(IEnumerable<Card> cards)
    {
        foreach (var card in cards)
        {
            Cards.Add(card);
        }
    }

    public void Clear()
    {
        Cards.Clear();
    }

    public Card ToD()
        => Cards.Last();
}