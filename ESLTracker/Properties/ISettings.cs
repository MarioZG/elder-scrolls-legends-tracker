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

        void Save();
    }
}