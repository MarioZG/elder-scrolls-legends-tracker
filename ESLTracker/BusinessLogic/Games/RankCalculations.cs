using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.BusinessLogic.Games
{
    public class RankCalculations
    {
        private readonly IDateTimeProvider dateTimeProvider;

        public RankCalculations(IDateTimeProvider dateTimeProvider)
        {
            this.dateTimeProvider = dateTimeProvider;
        }

        public void CalculateCurrentRankProgress(IEnumerable<Game> games, out PlayerRank rank, out int rankProgress, out int starsInRank) 
        {
            games = games.Where(g => g.Type == GameType.PlayRanked);
            var lastGame = games.OrderByDescending(g => g.Date).FirstOrDefault();
            if (lastGame == null)
            {
                rank = PlayerRank.TheRitual; //12
                rankProgress = 0;
                starsInRank = GetStarsperRank(rank);
            }
            else if (lastGame.Date.Month != dateTimeProvider.DateTimeNow.Month
                && lastGame.Date.Year == dateTimeProvider.DateTimeNow.Year)
            {
                //no games played this month
                rankProgress = 0;
                switch (lastGame.PlayerRank.Value)
                {
                    case PlayerRank.TheRitual:
                    case PlayerRank.TheLover:
                    case PlayerRank.TheLord:
                    case PlayerRank.TheMage:
                        rank = PlayerRank.TheRitual;
                        break;
                    case PlayerRank.TheShadow:
                    case PlayerRank.TheSteed:
                    case PlayerRank.TheApprentice:
                    case PlayerRank.TheWarrior:
                        rank = PlayerRank.TheMage;
                        break;
                    case PlayerRank.TheLady:
                    case PlayerRank.TheTower:
                    case PlayerRank.TheAtronach:
                    case PlayerRank.TheThief:
                    case PlayerRank.TheLegend:
                        rank = PlayerRank.TheWarrior;
                        break;
                    default:
                        throw new NotImplementedException($"Unknown player rank={lastGame.PlayerRank.Value}");
                }
                starsInRank = GetStarsperRank(rank);
            }
            else
            {
                rank = lastGame.PlayerRank.Value;
                starsInRank = GetStarsperRank(rank);
                rankProgress = 0;
                if (rank != PlayerRank.TheLegend)
                {
                    var currRankGames = games
                        .Where(g =>
                            g.Date.Year == dateTimeProvider.DateTimeNow.Year
                            && g.Date.Month == dateTimeProvider.DateTimeNow.Month
                            && g.PlayerRank == lastGame.PlayerRank)
                        .OrderByDescending(g => g.Date)
                        .Reverse()  //to step throght them  in chronological order
                        .ToList();

                    foreach (Game game in currRankGames)
                    {
                        if (game.Outcome == GameOutcome.Victory)
                        {
                            rankProgress += game.BonusRound.GetValueOrDefault(false) ? 2 : 1;
                            //last game was to advance rank,no games after that
                            if (rankProgress > starsInRank && game.Equals(games.Last()))
                            {
                                rank = (PlayerRank)((int)rank - 1);
                                rankProgress = 0;
                                starsInRank = GetStarsperRank(rank);
                            }
                        }
                        else
                        {
                            rankProgress--;
                            if (rankProgress < -2)
                            { //min serpent - cannot drop more
                                rankProgress = -2;
                            }
                        }
                    }
                }
            }
        }

        private int GetStarsperRank(PlayerRank rank)
        {
            switch (rank)
            {
                case PlayerRank.TheRitual:
                case PlayerRank.TheLover:
                case PlayerRank.TheLord:
                    return 4;
                case PlayerRank.TheMage:
                    return 7;
                case PlayerRank.TheShadow:
                case PlayerRank.TheSteed:
                case PlayerRank.TheApprentice:
                    return 5;
                case PlayerRank.TheWarrior:
                    return 7;
                case PlayerRank.TheLady:
                case PlayerRank.TheTower:
                case PlayerRank.TheAtronach:
                    return 6;
                case PlayerRank.TheThief:
                    return 7;
                case PlayerRank.TheLegend:
                    return 0;
                default:
                    throw new NotImplementedException($"Unknown player rank={rank}");
            }
        }
    }
}
