using UNO.Assets.Domain;

namespace UNO.Assets.Repository;

public interface IGameRepository
{
    void Save(GameState state);
    bool TryLoad(Guid? guid, out GameState load);
    GameState Load(Guid guid);
    ICollection<(DateTime dt, GameState state)> LoadAll();
    void Delete(Guid guid);
}