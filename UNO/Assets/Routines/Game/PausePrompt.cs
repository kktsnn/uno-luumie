using LuumieEngine.Components;
using UNO.Assets.Domain;

namespace UNO.Assets.Routines.Game;

// TODO add reset
public class PausePrompt : Prompt
{
    public Text Text = null!;
    public AsciiImage Image = null!;
    public void Show(GameState save)
    {
        Show();
        
        Text.Content = $"Game ID: {save.GameId.ToString()[24..]}\n{save.PrettyString()}";
        Image.Path = SceneElements.FilesDir + $"/Cards/Deck/{save.DrawPile.ToD().Type}.txt";
        Image.Color = save.DrawPile.ToD().ConsoleColor;
    }
}