using LuumieEngine.Classes;
using LuumieEngine.Components;

namespace UNO.Assets.Routines;

public class EntityCount : Routine
{
    private int Count => Transform.Scene!.Entities.Aggregate(0, (a, e) => e.Transform.AllChildCount() + a);
    private Text _text = null!;
    
    protected override void Awake()
    {
        _text = GetComponent<Text>();
    }

    protected override void Update()
    {
        _text.Content = Count.ToString();
    }
}