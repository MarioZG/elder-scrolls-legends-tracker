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
            var game = new DataModel.Game()
            {
                Date = dateTimeProvider.DateTimeNow
            };

            return game;
        }
    }
}
