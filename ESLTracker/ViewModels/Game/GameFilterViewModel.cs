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
    public abstract class GameFilterViewModel : ViewModelBase
    {
        protected DateTime? filterDateFrom;
        public DateTime? FilterDateFrom
        {
            get { return filterDateFrom; }
            set { filterDateFrom = value; RaiseDataPropertyChange(); }
        }

        protected DateTime? filterDateTo;
        public DateTime? FilterDateTo
        {
            get { return filterDateTo; }
            set { filterDateTo = value; RaiseDataPropertyChange(); }
        }

        protected GameType gameType;
        public GameType GameType
        {
            get { return gameType; }
            set { gameType = value; RaiseDataPropertyChange(); }
        }

        public dynamic DisplayDataSource
        {
            get { return GetDataSet(); }
        }

        public virtual IEnumerable<GameType> GameTypeSeletorValues
        {
            get
            {
                return Enum.GetValues(typeof(GameType)).Cast<GameType>();
            }
        }

        public Array FilterDateOptions
        {
            get
            {
                return Enum.GetValues(typeof(PredefinedDateFilter));
            }
        }

        protected PredefinedDateFilter filterDateSelectedOption;

        public PredefinedDateFilter FilterDateSelectedOption
        {
            get { return filterDateSelectedOption; }
            set
            {
                filterDateSelectedOption = value;
                SetDateFilters(value);
                RaisePropertyChangedEvent("FilterDateFrom");
                RaisePropertyChangedEvent("FilterDateTo");
                RaiseDataPropertyChange();

                settings.GamesFilter_SelectedPredefinedDateFilter = value;
                settings.Save();
            }
        }


        protected ITrackerFactory trackerFactory;
        protected ISettings settings;


        public GameFilterViewModel() : this(TrackerFactory.DefaultTrackerFactory)
        {

        }

        public GameFilterViewModel(ITrackerFactory trackerFactory)
        {
            this.trackerFactory = trackerFactory;
            this.settings = trackerFactory.GetService<ISettings>();
            this.FilterDateSelectedOption = settings.GamesFilter_SelectedPredefinedDateFilter;
        }


        public void SetDateFilters(PredefinedDateFilter value)
        {
            DateTime today = trackerFactory.GetDateTimeNow().Date;
            switch (value)
            {
                case PredefinedDateFilter.All:
                    filterDateFrom = null;
                    filterDateTo = null;
                    break;
                case PredefinedDateFilter.Today:
                    filterDateFrom = today.Date;
                    filterDateTo = today.Date;
                    break;
                case PredefinedDateFilter.Last7Days:
                    filterDateFrom = today.AddDays(-6);
                    filterDateTo = today.Date;
                    break;
                case PredefinedDateFilter.ThisMonth:
                    filterDateFrom = new DateTime(today.Year, today.Month, 1);
                    filterDateTo = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
                    break;
                case PredefinedDateFilter.PreviousMonth:
                    today = today.AddMonths(-1); //as we can change year in process!
                    filterDateFrom = new DateTime(today.Year, today.Month, 1);
                    filterDateTo = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
                    break;
                default:
                    break;
            }

        }

        public abstract dynamic GetDataSet();

        protected virtual void RaiseDataPropertyChange()
        {
            RaisePropertyChangedEvent("DisplayDataSource");
        }
    }       
}
