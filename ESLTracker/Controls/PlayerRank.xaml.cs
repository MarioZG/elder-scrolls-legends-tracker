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

namespace ESLTracker.Controls
{
    /// <summary>
    /// Interaction logic for PlayerRank.xaml
    /// </summary>
    public partial class PlayerRank : UserControl
    {

        public DataModel.Enums.PlayerRank? SelectedItem
        {
            get
            {
                return (DataModel.Enums.PlayerRank?)this.cbPlayerRank.SelectedItem;
            }
            set
            {
                this.cbPlayerRank.SelectedItem = value;
            }
        }

        public int? LegendRank
        {
            get
            {
                if ((! SelectedItem.HasValue) 
                    || (SelectedItem != DataModel.Enums.PlayerRank.TheLegend))
                {
                    return null;
                }
                int retValue;
                return int.TryParse(this.txtPlayerLegendRank.Text, out retValue) ? (int?)retValue : null;
            }
            set
            {
                this.txtPlayerLegendRank.Text = value.HasValue ? value.ToString() : null;
            }
        }

        public PlayerRank()
        {
            InitializeComponent();
        }
    }
}
