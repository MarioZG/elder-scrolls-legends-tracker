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
        Card FindCardById(Guid value);
    }
}