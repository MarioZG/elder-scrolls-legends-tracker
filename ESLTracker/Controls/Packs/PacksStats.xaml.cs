﻿using TESLTracker.DataModel;
using ESLTracker.Utils.SimpleInjector;
using ESLTracker.ViewModels.Packs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ESLTracker.Controls.Packs
{
    /// <summary>
    /// Interaction logic for PacksStats.xaml
    /// </summary>
    public partial class PacksStats : UserControl
    {

        new public PacksStatsViewModel DataContext
        {
            get
            {
                return (PacksStatsViewModel)base.DataContext;
            }
            set
            {
                base.DataContext = value;
            }
        }

        public IEnumerable<Pack> PacksData
        {
            get { return (IEnumerable<Pack>)GetValue(PacksDataProperty); }
            set { SetValue(PacksDataProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PacksData.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PacksDataProperty =
            DependencyProperty.Register("PacksData", typeof(IEnumerable<Pack>), typeof(PacksStats), new PropertyMetadata(new List<Pack>(), PacksDataChanged));

        private static void PacksDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PacksStats)d).DataContext.OrderedPacks = (IEnumerable<Pack>)e.NewValue;
        }

        public PacksStats()
        {
            DataContext = MasserContainer.Container.GetInstance<PacksStatsViewModel>();
            InitializeComponent();
        }
    }
}
