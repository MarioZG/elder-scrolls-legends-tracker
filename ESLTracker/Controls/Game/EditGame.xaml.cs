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
using TESLTracker.DataModel;
using TESLTracker.DataModel.Enums;
using ESLTracker.Utils;
using ESLTracker.Utils.Messages;
using ESLTracker.Utils.SimpleInjector;
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


        public bool IsEditControl
        {
            get { return (bool)GetValue(IsEditControlProperty); }
            set { SetValue(IsEditControlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsEditControl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEditControlProperty =
            DependencyProperty.Register(
                "IsEditControl", 
                typeof(bool), 
                typeof(EditGame), 
                new PropertyMetadata(false));

        public bool TrackActiveDeck
        {
            get { return (bool)GetValue(TrackActiveDeckProperty); }
            set { SetValue(TrackActiveDeckProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TrackActiveDeck.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TrackActiveDeckProperty =
            DependencyProperty.Register("TrackActiveDeck", typeof(bool), typeof(EditGame), new PropertyMetadata(false, TrackActiveDeckChanged));

        private static void TrackActiveDeckChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool && (bool)e.NewValue)
            {
                EditGame control = (EditGame)d;
                control.DataContext.RegisterTrackingActiveDeck(control.deckItem.DataContext);
            }
        }

        public EditGame() 
        {
            InitializeComponent();
            
            //TODO: Find a way to move it to xaml!
            var nameOfPropertyInVm = "IsEditControl";
            var binding = new Binding(nameOfPropertyInVm) { Mode = BindingMode.TwoWay };
            this.SetBinding(IsEditControlProperty, binding);
        }
    }
}
