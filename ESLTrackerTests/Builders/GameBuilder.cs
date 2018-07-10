using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTrackerTests.Builders
{
    public class GameBuilder
    {
        Game game;

        public GameBuilder()
        {
            game = new Game();
        }

        public GameBuilder WithDeck(Deck deck)
        {
            game.Deck = deck;
            return this;
        }

        public GameBuilder WithType(GameType? type)
        {
            game.Type = type;
            return this;
        }

        public GameBuilder WithOutcome(GameOutcome outcome)
        {
            game.Outcome = outcome;
            return this;
        }

        public Game Build()
        {
            return game;
        }
    }
}
