using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;
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

        public GameBuilder WithPlayerRank(PlayerRank? playerRank)
        {
            game.PlayerRank = playerRank;
            return this;
        }

        internal GameBuilder WithDate(DateTime? date)
        {
            game.Date = date.GetValueOrDefault();
            return this;
        }

        internal GameBuilder WithOpponentClass(DeckClass opponentClass)
        {
            game.OpponentClass = opponentClass;
            return this;
        }

        internal GameBuilder WithESLVersion(SerializableVersion serializableVersion)
        {
            game.ESLVersion = serializableVersion;
            return this;
        }

        public Game Build()
        {
            return game;
        }


    }
}
