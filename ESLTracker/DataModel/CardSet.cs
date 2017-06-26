using ESLTracker.Services;
using ESLTracker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.DataModel
{
    public class CardSet
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public bool HasPacks { get; set; }

        public static explicit operator CardSet(string name)
        {
            var cardDatabase = TrackerFactory.DefaultTrackerFactory.GetService<ICardsDatabase>();
            return cardDatabase.CardSets.Where(cs => cs.Name.ToLower() == name.ToLower()).SingleOrDefault();
        }
    }
}
