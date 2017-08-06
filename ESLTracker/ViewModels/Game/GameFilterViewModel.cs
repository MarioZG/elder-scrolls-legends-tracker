using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel.Enums;
using ESLTracker.Properties;
using ESLTracker.Utils;

namespace ESLTracker.ViewModels.Game
{
    public abstract class GameFilterViewModel : FilterDateViewModel
    {
        protected GameType gameType;
        public GameType GameType
        {
            get { return gameType; }
            set { gameType = value; RaiseDataPropertyChange(); }
        }

        public virtual IEnumerable<GameType> GameTypeSeletorValues
        {
            get
            {
                return Enum.GetValues(typeof(GameType)).Cast<GameType>();
            }
        }


        public GameFilterViewModel() : this(TrackerFactory.DefaultTrackerFactory)
        {

        }

        public GameFilterViewModel(ITrackerFactory trackerFactory): base (trackerFactory)
        {
            this.FilterDateSelectedOption = settings.GamesFilter_SelectedPredefinedDateFilter;
        }
    }       
}
