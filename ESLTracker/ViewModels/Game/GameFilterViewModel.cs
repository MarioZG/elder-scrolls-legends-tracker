using TESLTracker.DataModel.Enums;
using ESLTracker.Properties;
using ESLTracker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public GameFilterViewModel(ISettings settings, IDateTimeProvider dateTimeProvider) : base(settings, dateTimeProvider)
        {
            this.FilterDateSelectedOption = settings.GamesFilter_SelectedPredefinedDateFilter;
        }
    }       
}
