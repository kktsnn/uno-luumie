using LuumieEngine.Classes;

namespace UNO.Assets.Routines;

public class LoadVisible : Routine
{
    private const int Limit = 120;
    protected override void Update()
    {
        Transform[0].GameEntity.ActiveSelf = Math.Abs(GameEntity.Transform.Position.X) < Limit;
    }
}