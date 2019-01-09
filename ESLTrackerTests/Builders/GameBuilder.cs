using TESLTracker.DataModel;
using TESLTracker.DataModel.Enums;
using ESLTracker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TESLTracker.Utils;

namespace ESLTrackerTests.Builders
{
    public class GameBuilder
    {
        Game game;

        public GameBuilder()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            game = new Game();
#pragma warning restore CS0618 // Type or member is obsolete
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

        public GameBuilder WithPlayerLegendRank(int? playerLegendRank)
        {
            game.PlayerLegendRank = playerLegendRank;
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

        internal GameBuilder WithBonusRound(bool? bonusRound)
        {
            game.BonusRound = bonusRound;
            return this;
        }

        internal GameBuilder WithOpponentRank(PlayerRank opponentRank)
        {
            game.OpponentRank = opponentRank;
            return this;
        }

        internal GameBuilder WithOrderOfPlay(OrderOfPlay prderOfPlay)
        {
            game.OrderOfPlay = prderOfPlay;
            return this;
        }

        internal GameBuilder WithOpponentName(string opponentName)
        {
            game.OpponentName = opponentName;
            return this;
        }

        internal GameBuilder WithOpponentLegendRank(int? oppLegendRank)
        {
            game.OpponentLegendRank = oppLegendRank;
            return this;
        }

        public Game Build()
        {
            return game;
        }


    }
}
