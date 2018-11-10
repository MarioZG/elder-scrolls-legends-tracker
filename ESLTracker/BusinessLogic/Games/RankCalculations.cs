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

        public void CalculateCurrentRankProgress(
            IEnumerable<Game> games, 
            DateTime sessionStart,
            out PlayerRank rank, 
            out int rankProgress, 
            out int starsInRank,
            out int? legendStart,
            out int? legendMin,
            out int? legedmax,
            out int? legendCurrent) 
        {
            legendStart = legendMin = legendCurrent = legedmax = null;
            games = games.Where(g => g.Type == GameType.PlayRanked).OrderByDescending(g => g.Date);
            var lastGame = games.FirstOrDefault();
            if (lastGame == null)
            {
                rank = PlayerRank.TheRitual; //12
                rankProgress = 0;
                starsInRank = GetStarsperRank(rank);
            }
            else if (lastGame.Date.Month != dateTimeProvider.DateTimeNow.Month
                && lastGame.Date.Year == dateTimeProvider.DateTimeNow.Year)
            {
                CalculateEndofMonthRankReset(out rank, out rankProgress, out starsInRank, lastGame);
            }
            else
            {
                rank = lastGame.PlayerRank.Value;
                starsInRank = GetStarsperRank(rank);
                rankProgress = 0;
                if (rank != PlayerRank.TheLegend)
                {
                    CalculateRankProgress(games, ref rank, ref rankProgress, ref starsInRank, lastGame);
                }
                else
                {
                    CalculateLegendRankProgress(games, sessionStart, out legendStart, out legendMin, out legedmax, out legendCurrent);
                }
            }
        }

        private void CalculateEndofMonthRankReset(out PlayerRank rank, out int rankProgress, out int starsInRank, Game lastGame)
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

        private void CalculateRankProgress(IEnumerable<Game> games, ref PlayerRank rank, ref int rankProgress, ref int starsInRank, Game lastGame)
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
                    if (rankProgress > starsInRank && game.Equals(games.First()))
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

        private void CalculateLegendRankProgress(IEnumerable<Game> games, DateTime sessionStart, out int? legendStart, out int? legendMin, out int? legedmax, out int? legendCurrent)
        {
            games = games.Where(g =>
                            g.Date > sessionStart
                            && g.PlayerRank == PlayerRank.TheLegend
                            && g.PlayerLegendRank.HasValue);
            legedmax = games.Max(g => g.PlayerLegendRank);
            legendCurrent = games?.FirstOrDefault()?.PlayerLegendRank;
            legendMin = games.Min(g => g.PlayerLegendRank);
            legendStart = games.LastOrDefault()?.PlayerLegendRank;
        }

        private int GetStarsperRank(PlayerRank rank)
        {
            //-1 in allreturns as one star is neutral - stars in ui represent filled line between two stars
            // filled star represent won game (or empty star inserpen represents lost game)
            //case cale (ex: 7-1): no of games to advance - 1 less to show
            switch (rank)
            {
                case PlayerRank.TheRitual:
                case PlayerRank.TheLover:
                case PlayerRank.TheLord:
                    return 4 -1;
                case PlayerRank.TheMage:
                    return 7 - 1;
                case PlayerRank.TheShadow:
                case PlayerRank.TheSteed:
                case PlayerRank.TheApprentice:
                    return 5 - 1;
                case PlayerRank.TheWarrior:
                    return 7 - 1;
                case PlayerRank.TheLady:
                case PlayerRank.TheTower:
                case PlayerRank.TheAtronach:
                    return 6 - 1;
                case PlayerRank.TheThief:
                    return 7 - 1;
                case PlayerRank.TheLegend:
                    return 0 - 1;
                default:
                    throw new NotImplementedException($"Unknown player rank={rank}");
            }
        }
    }
}
