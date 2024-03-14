using LuumieEngine.Classes;
using LuumieEngine.Components;
using LuumieEngine.Structs;
using UNO.Assets.Domain;

namespace UNO.Assets.Routines.Game;

public class DiscardPile : Routine
{
    public int Limit { get; set; } = 10;
    private Vector2Int _lastOffset;
    
    private Queue<GameEntity> _cards = new();

    public void Discard(Card card, Vector2Int location)
    {
        foreach (var c in _cards)
        {
            c.Transform.LocalDepth -= 1;
        }

        var e = SceneElements.Card(card);
        e.Transform.LocalPosition = location;
        e.Transform.Depth = _cards.Count;
        
        GameEntity.AddChild(e);
        
        Vector2Int offset;
        do
        {
            offset = new Vector2Int(
                Random.Shared.Next(-1, 2),
                Random.Shared.Next(-1, 2));
        } while (offset == _lastOffset);
        _lastOffset = offset;
        
        e.AddComponent<Movement>().MoveTo(Transform.Position + offset, 5f);
        
        _cards.Enqueue(e);
        if (_cards.Count > Limit) GameEntity.RemoveChild(_cards.Dequeue());
    }
}