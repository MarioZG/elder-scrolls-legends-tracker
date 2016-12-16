using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel.Enums;
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
                return Enum.GetValues(typeof(DateFilter));
            }
        }

        protected DateFilter filterDateSelectedOption;

        public DateFilter FilterDateSelectedOption
        {
            get { return filterDateSelectedOption; }
            set
            {
                filterDateSelectedOption = value;
                SetDateFilters(value);
                RaisePropertyChangedEvent("FilterDateFrom");
                RaisePropertyChangedEvent("FilterDateTo");
                RaiseDataPropertyChange();
            }
        }


        protected ITrackerFactory trackerFactory;
        
        public GameFilterViewModel() : this(TrackerFactory.DefaultTrackerFactory)
        {

        }

        public GameFilterViewModel(ITrackerFactory trackerFactory)
        {
            this.trackerFactory = trackerFactory;
        }


        public void SetDateFilters(DateFilter value)
        {
            DateTime today = trackerFactory.GetDateTimeNow().Date;
            switch (value)
            {
                case DateFilter.All:
                    filterDateFrom = null;
                    filterDateTo = null;
                    break;
                case DateFilter.Last7Days:
                    filterDateFrom = today.AddDays(-6);
                    filterDateTo = today.Date;
                    break;
                case DateFilter.ThisMonth:
                    filterDateFrom = new DateTime(today.Year, today.Month, 1);
                    filterDateTo = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));
                    break;
                case DateFilter.PreviousMonth:
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

    

    public enum DateFilter
    {
        All,
        Last7Days,
        ThisMonth,
        PreviousMonth
    }
}
