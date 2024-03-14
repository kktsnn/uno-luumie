using LuumieEngine.Classes;
using LuumieEngine.Components;
using Timer = System.Timers.Timer;

namespace UNO.Assets.Routines;

public class Framerate : Routine
{
    private int _count;
    private readonly Timer _timer;
    private Text _counter = null!;

    public Framerate()
    {
        _timer = new Timer(1000);
        _timer.Elapsed += (_, _) =>
        {
            _counter.Content = _count.ToString();
            _count = 0;
        };
    }

    protected override void Awake()
    {
        _counter = GetComponent<Text>();
        _timer.Start();
    }

    protected override void Update()
    {
        _count++;
    }
}