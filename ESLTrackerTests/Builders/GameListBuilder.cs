using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTrackerTests.Builders
{
    public class GameListBuilder
    {
        IEnumerable<Game> games;
        Deck deck;
        GameType? gameType;

        public GameListBuilder()
        {
            games = new List<Game>();
        }

        public GameListBuilder UsingDeck(Deck deck)
        {
            this.deck = deck;
            return this;
        }

        public GameListBuilder UsingType(GameType? gameType)
        {
            this.gameType = gameType;
            return this;
        }

        public GameListBuilder WithOutcome(int count, GameOutcome outcome)
        {
            var newGames = Enumerable.Range(0, count)
                .Select(x => new GameBuilder()
                            .WithOutcome(outcome)
                            .WithDeck(deck)
                            .WithType(gameType)
                            .Build()
                        );
            games = games.Union(newGames).ToList();
            return this;
        }

        public ObservableCollection<Game> Build()
        {
            return new ObservableCollection<Game>(games.ToList());
        }
    }
}
