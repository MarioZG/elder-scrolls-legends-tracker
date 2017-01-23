using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ESLTracker.DataModel;

namespace ESLTracker.Utils
{
    public interface ICardsDatabase
    {
        IEnumerable<Card> Cards { get; }
        IEnumerable<string> CardsNames { get; }
        Version Version { get; set; }
        Card FindCardById(Guid value);
        Card FindCardByName(string name);
        void RealoadDB();
    }
}