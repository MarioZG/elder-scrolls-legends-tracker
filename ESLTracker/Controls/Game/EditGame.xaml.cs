﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using ESLTracker.DataModel.Enums;
using ESLTracker.ViewModels.Game;

namespace ESLTracker.Controls.Game
{
    /// <summary>
    /// Interaction logic for EditGame.xaml
    /// </summary>
    public partial class EditGame : UserControl
    {
        new public EditGameViewModel DataContext
        {
            get
            {
                return (EditGameViewModel)base.DataContext;
            }
            set
            {
                base.DataContext = value;
            }
        }

        public EditGame()
        {
            InitializeComponent();

            DataModel.Tracker.Instance.PropertyChanged += Instance_PropertyChanged;
            opponentClass.DataContext.PropertyChanged += DataContext_PropertyChanged;

        }

        private void DataContext_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "SelectedClass")
            {
                this.DataContext.ShowWinsVsClass(opponentClass.DataContext.SelectedClass);
            }
        }

        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (this.selectedDeck.DataContext != null)
            {
                if (DataModel.Tracker.Instance.ActiveDeck.Type == DeckType.VersusArena)
                {
                    this.cbGameType.SelectedItem = DataModel.Enums.GameType.VersusArena;
                }
            }
        }

      
    }
}
