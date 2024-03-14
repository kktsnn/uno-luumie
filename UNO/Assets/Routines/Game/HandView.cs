using LuumieEngine.Classes;
using LuumieEngine.Components;
using LuumieEngine.Structs;
using UNO.Assets.Domain;

namespace UNO.Assets.Routines.Game;

public class HandView : Routine
{
    public GameController Controller = null!;
    public UiNavigation Nav = null!;

    public void LoadHand(Player p, bool show, Card? selected = null)
    {
        GameEntity.ClearChildren();
        
        Button? last = null;

        var skip = p.Hand.Cards.Count == 1 ? 0 : Math.Min(12, 65 / (p.Hand.Cards.Count - 1));
        var offset = skip * (p.Hand.Cards.Count - 1) / -2;
        foreach (var c in p.Hand.Cards)
        {
            var e = show ? SceneElements.Card(c) : SceneElements.Card("Unknown", ConsoleColor.White);
            e.Transform.Depth = offset - 100;
            e.Transform.LocalPosition = new Vector2Int(offset, 0);
            var b = AddMovement(e.AddComponent<Button>(), e.AddComponent<Movement>());
            b.Action = () =>
            {
                if (!Controller.CheckCard(c)) return;
                Nav.Current = null;
            };
            b.Left = last;
            last = b;
            e.AddComponent<Container<Card>>().Content = c;
            
            GameEntity.AddChild(e);
            
            if (c == selected) 
                if (show) Nav.Current = b;
                else b.OnSelect();
            offset += skip;
        }
        
        if (show && selected == null) Nav.Current = last;

        Nav.EnabledSelf = false;
        Nav.UpdateStatus(); // block input for one frame
        Nav.EnabledSelf = true;
    }

    public Vector2Int FindCardPosition(Card c)
    {
        foreach (var t in Transform)
        {
            if (t.GetComponent<Container<Card>>().Content != c) continue;
            GameEntity.RemoveChild(t.GameEntity);
            return t.Position;
        }

        return Transform.Position;
    }

    public void Skipped()
    {
        GameEntity.ClearChildren();
        GameEntity.AddChild(SceneElements.BigText("Skipped"));
    }

    private static Button AddMovement(Button b, Movement m)
    {
        b.Selected += () => { m.Offset(new Vector2Int(0, 3)); };
        b.Deselected += () => { m.Offset(new Vector2Int(0, -3)); };
        return b;
    }
}