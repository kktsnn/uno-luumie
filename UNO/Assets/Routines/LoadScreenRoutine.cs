using LuumieEngine.Classes;
using LuumieEngine.Components;
using LuumieEngine.SceneManagement;
using LuumieEngine.Structs;
using UNO.Assets.Repository;
using UNO.Assets.Routines.Game;
using UNO.Assets.Screens;

namespace UNO.Assets.Routines;

public class LoadScreenRoutine : Routine
{
    public IGameRepository? Repository;
    public Prompt Confirmation = null!;
    
    private UiNavigation _uiNavigation = null!;
    
    private int _current;
    private bool _savesFound = true;

    protected override void Awake()
    {
        _uiNavigation = GameEntity.GetComponent<UiNavigation>();
        _uiNavigation.Next += Move;
        
        Saves();
        if (Transform.ChildCount() == 0) NoSavesFound();
    }

    protected override void OnKeyPress(ConsoleKeyInfo keyInfo)
    {
        if (keyInfo.Key is ConsoleKey.Escape)
        {
            if (Confirmation.Enabled) Confirmation.Hide();
            else SceneManager.ChangeScene(typeof(MainMenuScreen));
            return;
        }
        
        if (Confirmation.GameEntity.Active || 
            !_savesFound ||
            keyInfo.Key is not ConsoleKey.Delete) return;
        
        Confirmation.Show();
    }

    private void NoSavesFound()
    {
        var text = SceneElements.BigText("NoSavesFound");
        text.Transform.LocalPosition = new Vector2Int(0, 24);
        GameEntity.AddChild(text);
        _savesFound = false;
    }

    private void Saves(int offset = 0)
    {
        GameEntity.ClearChildren();
        var i = -90 - offset;

        UiRoutine? last = null;
        
        foreach (var save in Repository!.LoadAll())
        {
            var e = GameEntity.Empty;
            e.Transform.LocalPosition = new Vector2Int(i, 0);
            e.Name = save.state.GameId.ToString();
            e.AddComponent<Movement>();
            e.AddComponent<LoadVisible>();
            
            var card = 
                save.state.DiscardPile.Cards.Count == 0 ? 
                SceneElements.Card(null, ConsoleColor.White) : 
                SceneElements.Card(save.state.DiscardPile.ToD());
            var text = card.AddComponent<Text>();
            text.Alignment = ETextAlignment.Left;
            text.Content = $"Game ID: {save.state.GameId.ToString()[24..]}\n{save.dt.ToShortDateString()} {save.dt.ToShortTimeString()}\n{save.state.PrettyString()}";
            text.LocalPosition = new Vector2Int(0, 30);
            
            var image = card.GetComponent<AsciiImage>();
            image.LocalPosition = new Vector2Int(0, 6);
            
            var box = card.AddComponent<Box>();
            box.Size = new Vector2Int(29, 50);
            box.LocalPosition = new Vector2Int(0, 24);
            
            e.AddChild(card);
            
            var button = e.AddComponent<Button>();
            button.Graphic = box;
            button.HighlightColor = ConsoleColor.DarkYellow;
            button.Left = last;
            button.Action = () =>
            {
                
                GameController.Load(Repository.Load(Guid.Parse(e.Name)));
                SceneManager.ChangeScene(typeof(GameScreen));
            };

            GameEntity.AddChild(e);

            if (last == null)
            {
                _uiNavigation.Current = button;
                if (!_uiNavigation.Enabled) button.OnSelect();
            }
            last = button;
            i += box.Size.X + 1;
        }
    }

    private void Move(EDirection direction)
    {
        var step = 30;

        if (Confirmation.Enabled) return;
        
        switch (direction)
        {
            case EDirection.Left:
                _current--;
                if (_current < 3 || _current > Transform.ChildCount() - 5) return;
                break;
            case EDirection.Right:
                _current++;
                if (_current < 4 || _current > Transform.ChildCount() - 4) return;
                step = -30;
                break;
            default:
                return;
        }
        
        var speed = Transform[_current].LocalPosition.X / 15;
        foreach (var t in Transform)
        {
            t.GetComponent<Movement>().Offset(new Vector2Int(step, 0), speed * speed);
        }
    }

    private void Select(int i)
    {
        var offset = Math.Max(0, Math.Min(_current, Transform.ChildCount() - 4) - 3);
        Saves(offset * 30);
        if (Transform.ChildCount() == 0) NoSavesFound();
        else _uiNavigation.Current = Transform[i].GetComponent<Button>();
    }

    public void DeleteCurrent()
    {
        Repository!.Delete(Guid.Parse(GameEntity.Transform[_current].GameEntity.Name!));
        if (_current != 0) _current--;
        Select(_current);
    }
}