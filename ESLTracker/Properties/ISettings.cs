using System;
using ESLTracker.DataModel.Enums;

namespace ESLTracker.Properties
{
    public interface ISettings
    {
        Guid? LastActiveDeckId { get; set; }
        double MainWindowPositionX { get; set; }
        double MainWindowPositionY { get; set; }
        bool MinimiseOnClose { get; set; }
        double OverlayWindowPositionX { get; set; }
        double OverlayWindowPositionY { get; set; }
        PlayerRank PlayerRank { get; set; }
        bool ShowDeckStats { get; set; }
        string DataPath { get; set; }

        string NewDeck_SoloArenaName { get; set; }
        string NewDeck_VersusArenaName { get; set; }

        /// <summary>
        /// flag set default to true - used to preserve setting in new versions of application
        /// </summary>
        bool UpgradeRequired { get; set; }

        //games filter settings
        ViewModels.Game.PredefinedDateFilter GamesFilter_SelectedPredefinedDateFilter { get; set; }
        TimeSpan GamesFilter_DayCutoffTime { get; set; }

        void Save();
    }
}