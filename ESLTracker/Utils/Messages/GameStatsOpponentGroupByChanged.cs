using System.Collections.Generic;

namespace ESLTracker.Utils.Messages
{
    public class GameStatsOpponentGroupByChanged
    {
        public string OpponentGroupBy { get; set; }
        public IEnumerable<string> Tags { get; internal set; }
    }
}