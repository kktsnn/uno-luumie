using LuumieEngine.Classes;
using LuumieEngine.Components;

namespace UNO.Assets.Routines.Game;

// TODO Animate?
public class Arrow : Routine
{
    public bool Direction;

    protected override void Awake()
    {
        GameEntity.GetComponent<Text>().Content = Direction ? "<----<----<----" : "---->---->---->";
    }

    public void FlipDirection()
    {
        Direction = !Direction;
        GameEntity.GetComponent<Text>().Content = Direction ? "<----<----<----" : "---->---->---->";
    }
}