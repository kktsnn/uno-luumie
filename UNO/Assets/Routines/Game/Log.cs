using LuumieEngine.Classes;
using LuumieEngine.Components;

namespace UNO.Assets.Routines.Game;

public class Log : Routine
{
    public int Limit { get; set; } = 15;
    private readonly Queue<string> _log = new();

    protected override void Awake()
    {
        GameEntity.GetComponent<Text>().Alignment = ETextAlignment.Left;
        for (var i = 0; i < Limit; i++) AddLine("");
    }

    public void AddLine(string line)
    {
        _log.Enqueue(line);
        if (_log.Count > Limit) _log.Dequeue();
        if (Enabled) UpdateText();
    }

    public void Clear()
    {
        _log.Clear();
        UpdateText();
    }

    private void UpdateText()
    {
        GameEntity.GetComponent<Text>().Content = string.Join("\n", _log);
    }
}