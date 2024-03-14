using LuumieEngine.Classes;
using LuumieEngine.Components;
using LuumieEngine.Structs;
using UNO.Assets.Domain;

namespace UNO.Assets.Routines.Game;

public class PlayerIcon : Routine
{
    public Player Player = null!;

    private Text _name = null!;

    private bool _uno;
    protected override void Awake()
    {
        _name = GameEntity.GetComponent<Text>();
        _name.LocalPosition = new Vector2Int(0, 7);
        UpdatePlayer();
    }

    public void UpdatePlayer()
    {
        _name.Content = $"{Player.Name} ({Player.Hand.Cards.Count})";
        GameEntity.ClearChildren();
        var offset = 0;
        for (var i = 0; i < Math.Min(4, Player.Hand.Cards.Count); i++)
        {
            var c = SceneElements.Card("Unknown", i == 0 && _uno ? ConsoleColor.DarkYellow : ConsoleColor.White);
            c.Transform.LocalPosition = new Vector2Int(offset, 0);
            c.Transform.Depth = -i;
            offset += 3 - i;
            GameEntity.AddChild(c);
        }
    }

    public void Highlight(bool active)
    {
        _name.Color = active ? ConsoleColor.DarkYellow : ConsoleColor.White;
    }

    public void Uno(bool active)
    {
        _uno = active;
        UpdatePlayer();
    }
}