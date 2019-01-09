using ESLTracker.Properties;
using ESLTracker.Utils;
using ESLTracker.ViewModels.Enums;
using System;
using TESLTracker.Utils;

namespace ESLTracker.ViewModels
{
    public abstract class FilterDateViewModel : ViewModelBase
    {
        protected DateTime? filterDateFrom;
        public DateTime FilterDateFrom
        {
            get { return filterDateFrom.HasValue ? filterDateFrom.Value : new DateTime(2016, 1, 1) ; }
            set {
                filterDateFrom = value;
                filterDateSelectedOption = PredefinedDateFilter.Custom;
                RaisePropertyChangedEvent(nameof(filterDateSelectedOption));
                RaiseDataPropertyChange();
            }
        }

        protected DateTime? filterDateTo;
        public DateTime FilterDateTo
        {
            get { return filterDateTo.HasValue ? filterDateTo.Value : DateTime.Today.Date; }
            set {
                filterDateTo = value;
                filterDateSelectedOption = PredefinedDateFilter.Custom;
                RaisePropertyChangedEvent(nameof(filterDateSelectedOption));
                RaiseDataPropertyChange();
            }
        }

        public dynamic DisplayDataSource
        {
            get { return GetDataSet(); }
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
                RaisePropertyChangedEvent(nameof(FilterDateFrom));
                RaisePropertyChangedEvent(nameof(FilterDateTo));
                RaiseDataPropertyChange();

                settings.GamesFilter_SelectedPredefinedDateFilter = value;
                settings.Save();
            }
        }


        protected ISettings settings;
        protected IDateTimeProvider dateTimeProvider;

        public FilterDateViewModel(ISettings settings, IDateTimeProvider dateTimeProvider)
        {
            this.settings = settings;
            this.dateTimeProvider = dateTimeProvider;
            this.FilterDateSelectedOption = settings.GamesFilter_SelectedPredefinedDateFilter;
        }

        public void SetDateFilters(PredefinedDateFilter value)
        {
            DateTime today = dateTimeProvider.DateTimeNow.Date;
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
            RaisePropertyChangedEvent(nameof(DisplayDataSource));
        }

    }
}
