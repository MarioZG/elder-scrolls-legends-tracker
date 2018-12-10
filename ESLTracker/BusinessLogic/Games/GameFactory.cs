using ESLTracker.DataModel;
using ESLTracker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.BusinessLogic.Games
{
    public class GameFactory : IGameFactory
    {
        private readonly IDateTimeProvider dateTimeProvider;

        public GameFactory(IDateTimeProvider dateTimeProvider)
        {
            this.dateTimeProvider = dateTimeProvider;
        }

        public Game CreateGame()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var game = new Game()
            {
                Date = dateTimeProvider.DateTimeNow
            };
#pragma warning restore CS0618 // Type or member is obsolete

            return game;
        }

        public Game CreateGame(Game previousGame)
        {
            var game = CreateGame();


            game.Deck = previousGame.Deck;

            //restore values that are likely the same,  like game type, player rank etc
            game.Type = previousGame.Type;
            if (game.Type == DataModel.Enums.GameType.PlayRanked)
            {
                game.PlayerRank = previousGame.PlayerRank;
                game.PlayerLegendRank = previousGame.PlayerLegendRank;
                game.BonusRound = false;
            }
            return game;
        }
    }
}
