﻿using System;
using System.ComponentModel;
using TESLTracker.DataModel.Enums;

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
        ViewModels.Enums.PredefinedDateFilter GamesFilter_SelectedPredefinedDateFilter { get; set; }
        TimeSpan GamesFilter_DayCutoffTime { get; set; }

        //make screenshot when pack is confirmed
        bool Packs_ScreenshotAfterAdded { get; set; }

        //screenshot name templates
        string Packs_ScreenshotNameTemplate { get; set; }
        string General_ScreenshotNameTemplate { get; set; }

        double OverlayDeck_Scale { get; set; }

        bool OverlayDeck_ShowOnStart { get; set; }
        bool OverlayWindow_ShowOnStart { get; set; }

        ViewModels.Decks.DeckDeleteMode DeckDeleteMode { get; set; }
        ViewModels.Decks.DeckViewSortOrder DeckViewSortOrder { get; set; }

        bool DeckViewLastGamesIndicatorShow { get; set; }
        int DeckViewLastGamesIndicatorCount { get; set; }

        string VersionCheck_VersionsUrl { get; set; }
        string VersionCheck_LatestBuildUrl { get; set; }
        string VersionCheck_CardsDBUrl { get; set; }
        string VersionCheck_LatestBuildUserUrl { get; set; }

        Guid? Packs_LastOpenedPackSetId { get; set; }

        bool General_StartGameWithTracker { get; set; }

        double SessionOverlay_Scale { get; set; }
        bool SessionOverlay_ShowOnStart { get; set; }
        DateTime SessionOverlay_SessionStartDateTime { get; set; }
        bool SessionOverlay_ResetOnApplicationStart { get; set; }

        int General_RankedSeasonResetTime { get; set; }

        event PropertyChangedEventHandler PropertyChanged;

        void Save();
        void Reload();
    }
}