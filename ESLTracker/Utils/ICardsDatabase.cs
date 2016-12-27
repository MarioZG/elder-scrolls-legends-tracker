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
        void PopulateCollection(ObservableCollection<string> cardNames, ObservableCollection<CardInstance> targetCollection);
        Card FindCardById(Guid value);
    }
}