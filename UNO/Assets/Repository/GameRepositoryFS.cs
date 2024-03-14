using System.Text.Json;
using UNO.Assets.Domain;

namespace UNO.Assets.Repository;

public class GameRepositoryFS : IGameRepository
{
    private const string SaveLocation = "./saves/";
    
    public void Save(GameState state)
    {
        if (!Directory.Exists(SaveLocation))
            Directory.CreateDirectory(SaveLocation);
        
        var saveState = JsonSerializer.Serialize(state, new JsonSerializerOptions());

        using var w = new StreamWriter(SaveLocation + $"{state.GameId}.json");
        w.WriteLine(saveState);
    }
    
    public bool TryLoad(Guid? guid, out GameState state)
    {
        state = null!;

        if (guid == null || Path.Exists($"{SaveLocation}{guid}.json")) return false;
        
        using var r = new StreamReader($"{SaveLocation}{guid}.json");
        
        state = JsonSerializer.Deserialize<GameState>(r.ReadLine()!)!;

        return true;
    }

    public GameState Load(Guid guid)
    {
        using var r = new StreamReader($"{SaveLocation}{guid}.json");
        var state = JsonSerializer.Deserialize<GameState>(r.ReadLine()!);

        return state!;
    }

    public ICollection<(DateTime dt, GameState state)> LoadAll()
    {
        if (!Directory.Exists(SaveLocation)) 
            return new List<(DateTime dt, GameState state)>();
        
        return Directory.EnumerateFiles(SaveLocation).Select(
            path =>
            (
                File.GetLastWriteTime(path), 
                Load(Guid.Parse(Path.GetFileNameWithoutExtension(path)))
            )
        ).OrderByDescending(p => p.Item1).ToList();
    }

    public void Delete(Guid guid)
    {
        File.Delete($"{SaveLocation}{guid}.json");
    }
}