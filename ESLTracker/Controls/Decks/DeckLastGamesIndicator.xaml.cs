﻿using ESLTracker.BusinessLogic.Decks;
using ESLTracker.ViewModels.Decks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ESLTracker.Controls.Decks
{
    /// <summary>
    /// Interaction logic for DeckLastGamesIndicator.xaml
    /// </summary>
    public partial class DeckLastGamesIndicator : UserControl
    {

        private const int LAST_GAME_INDICATOR_HEIGHT = 4;
        private const int LAST_GAME_INDICATOR_MARGIN = 1;

        public int GamesCount
        {
            get { return (int)GetValue(GamesCountProperty); }
            set { SetValue(GamesCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for GamesCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GamesCountProperty =
            DependencyProperty.Register("GamesCount", typeof(int), typeof(DeckLastGamesIndicator), new PropertyMetadata(-1, NumberOfColumnsChanged));



        public IEnumerable<bool> LastGamesOutcome
        {
            get { return (IEnumerable<bool>)GetValue(LastGamesOutcomeProperty); }
            set { SetValue(LastGamesOutcomeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LastGamesOutcome.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LastGamesOutcomeProperty =
            DependencyProperty.Register("LastGamesOutcome", typeof(IEnumerable<bool>), typeof(DeckLastGamesIndicator), new PropertyMetadata(null, LastGamesOutcomeChanged));


        private static void LastGamesOutcomeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DeckLastGamesIndicator control = d as DeckLastGamesIndicator;
            RefreshLastGamesIndicator(d.GetValue(LastGamesOutcomeProperty) as IEnumerable<bool>, control);
        }

        private static void NumberOfColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DeckLastGamesIndicator control = d as DeckLastGamesIndicator;
            control.grid.ColumnDefinitions.Clear();
            for (int i = 0; i < (int)e.NewValue; i++)
            {
                control.grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star)});
            }
            DataModel.Deck deck = d.GetValue(DeckProperty) as DataModel.Deck;
            if (deck != null)
            {
                RefreshLastGamesIndicator(d.GetValue(LastGamesOutcomeProperty) as IEnumerable<bool>, control);
            }
        }

        public DataModel.Deck Deck
        {
            get { return (DataModel.Deck)GetValue(DeckProperty); }
            set { SetValue(DeckProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValuesList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeckProperty =
            DependencyProperty.Register("Deck", typeof(DataModel.Deck), typeof(DeckLastGamesIndicator), new PropertyMetadata(null, DeckChanged));

        private static void DeckChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                DataModel.Deck deck = e.NewValue as DataModel.Deck;
                DeckLastGamesIndicator control = d as DeckLastGamesIndicator;
                deck.PropertyChanged += delegate {
                    RefreshLastGamesIndicator(d.GetValue(LastGamesOutcomeProperty) as IEnumerable<bool>, control);
                };
                RefreshLastGamesIndicator(d.GetValue(LastGamesOutcomeProperty) as IEnumerable<bool>, control);
            }
        }

        private static void RefreshLastGamesIndicator(IEnumerable<bool> lastGamesOutcome, DeckLastGamesIndicator control)
        {
            int col = 0;
            control.grid.Children.Clear();
            foreach (bool win in lastGamesOutcome)
            {
                Rectangle r = new Rectangle();
                r.Fill = win ? Brushes.SeaGreen : Brushes.Brown;
                r.SetValue(Grid.ColumnProperty, col++);
                r.Height = LAST_GAME_INDICATOR_HEIGHT;
                r.Margin = new Thickness(LAST_GAME_INDICATOR_MARGIN, 0, LAST_GAME_INDICATOR_MARGIN, 0);
                control.grid.Children.Add(r);
            }
        }

        public DeckLastGamesIndicator()
        {
            InitializeComponent();
        }
    }
}
