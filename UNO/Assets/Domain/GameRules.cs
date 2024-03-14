namespace UNO.Assets.Domain;

public class GameRules
{
    public int Points { get; set; }
    public int CardAmount { get; set; } = 7;
    public bool Stacking { get; set; }
    public bool SevenZero { get; set; }
    public bool ForcePlay { get; set; }
    public bool NoBluffing { get; set; } = true;
    public bool DrawToMatch { get; set; }
    
    public GameRules() {}
    
    public GameRules(
        int points,
        int cardAmount,
        bool stacking,
        bool sevenZero, 
        bool forcePlay, 
        bool noBluffing, 
        bool drawToMatch)
    {
        Points = points;
        CardAmount = cardAmount;
        Stacking = stacking;
        SevenZero = sevenZero;
        ForcePlay = forcePlay;
        NoBluffing = noBluffing;
        DrawToMatch = drawToMatch;
    }

    public string PrettyString()
    {
        return $"Points: {Points}\n" +
               $"Starting Cards: {CardAmount}\n" +
               $"Stacking: {OnOff(Stacking)}\n" +
               $"Seven/Zero: {OnOff(SevenZero)}\n" +
               $"Force Play: {OnOff(ForcePlay)}\n" +
               $"No Bluffing: {OnOff(NoBluffing)}\n" +
               $"Draw 2 Match: {OnOff(DrawToMatch)}";
    }

    private string OnOff(bool b)
    {
        return b ? "On" : "Off";
    }
}