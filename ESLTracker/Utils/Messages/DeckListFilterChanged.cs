using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.ViewModels;
using ESLTracker.ViewModels.Decks;

namespace ESLTracker.Utils.Messages
{
    public class DeckListFilterChanged
    {
        public enum Source
        {
            TypeFilter,
            ClassFilter,
            Unknown
        }

        public Source ChangeSource { get; private set; }
        public IEnumerable<DeckType> FilteredTypes { get; private set; }
        public bool ShowFInishedArenaRuns { get; private set; }
        public bool ShowHiddenDecks { get; private set; }
        public DeckClass? SelectedClass { get; private set; }
        public IEnumerable<DeckClass> FilteredClasses { get; private set; }


        public DeckListFilterChanged(
            Source source,
            IEnumerable<DeckType> filteredTypes,
            bool? showFinishedArenaRuns,
            bool? showHiddenDecks,
            DeckClass? selectedClass,
            IEnumerable<DeckClass> filteredClasses)
        {
            this.ChangeSource = source;
            this.FilteredTypes = filteredTypes;
            this.ShowFInishedArenaRuns = showFinishedArenaRuns.HasValue ? showFinishedArenaRuns.Value : false;
            this.ShowHiddenDecks = showHiddenDecks.HasValue ? showHiddenDecks.Value : false;
            this.SelectedClass = selectedClass;
            this.FilteredClasses = filteredClasses;
        }

    }
}
