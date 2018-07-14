using ESLTracker.DataModel;

namespace ESLTracker.BusinessLogic.Games
{
    public interface IGameFactory
    {
        Game CreateGame();
        Game CreateGame(Game previousGame);
    }
}