namespace UNO.Assets.Domain;

public class Hand
{
    public List<Card> Cards { get; set; } = new();

    public int Add(Card card)
    {
        var i = Search(card);
        Cards.Insert(i, card);
        return i;
    }

    public void Add(IEnumerable<Card> c)
        => Cards.AddRange(c);

    public Card Get(int i)
    {
        var c = Cards[i];
        Cards.RemoveAt(i);
        return c;
    }
    
    public void Sort()
        => Cards = Cards.OrderBy(c => (int)c.Color).ThenBy(c => (int)c.Type).ToList();

    public bool HasColor(ECardColor color)
        => Cards.Any(c => c.Color == color);

    public bool HasType(ECardType type)
        => Cards.Any(c => c.Type == type);
    
    public bool HasCard(Card card)
        => Cards.Any(c => c.Equals(card));

    public bool HasValidCard(Card card)
        => Cards.Any(c => c.CanStack(card));

    public int HandValue()
        => Cards.Sum(c => c.Value);

    public int ColorCount(ECardColor color)
        => Cards.Count(c => c.Color == color && c.Color is not ECardColor.Wild);

    public bool Empty()
        => Cards.Count == 0;

    public int Search(Card c)
        => ~Cards.BinarySearch(c);

    public override string ToString()
    {
        return string.Join(' ', Cards);
    }
}