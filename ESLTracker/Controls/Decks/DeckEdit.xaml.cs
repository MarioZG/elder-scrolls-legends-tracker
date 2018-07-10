﻿using System;
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
using ESLTracker.DataModel;
using ESLTracker.Utils.SimpleInjector;
using ESLTracker.ViewModels.Decks;

namespace ESLTracker.Controls.Decks
{
    /// <summary>
    /// Interaction logic for DeckEdit.xaml
    /// </summary>
    public partial class DeckEdit : UserControl
    {
        public Deck Deck
        {
            get { return (Deck)GetValue(DeckProperty); }
            set { SetValue(DeckProperty, value); }
        }

        // Using a DependencyProperty as the backing store for deck.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeckProperty =
            DependencyProperty.Register("Deck", typeof(Deck), typeof(DeckEdit), new PropertyMetadata(null, DeckChanged));

        private static void DeckChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DeckEditViewModel)((DeckEdit)d).DataContext).Deck = e.NewValue as Deck;
        }

        public DeckEdit()
        {
            InitializeComponent();
            base.DataContext = MasserContainer.Container.GetInstance<DeckEditViewModel>();
        }
    }
}
