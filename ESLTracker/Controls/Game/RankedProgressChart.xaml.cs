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
using ESLTracker.ViewModels.Game;

namespace ESLTracker.Controls.Game
{
    /// <summary>
    /// Interaction logic for RankedProgressChart.xaml
    /// </summary>
    public partial class RankedProgressChart : UserControl
    {


        new public RankedProgressChartViewModel DataContext
        {
            get
            {
                return (RankedProgressChartViewModel)base.DataContext;
            }
            set
            {
                base.DataContext = value;
            }
        }

        public RankedProgressChart()
        {
            InitializeComponent();

            foreach (DataModel.Enums.PlayerRank pr in Enum.GetValues(typeof(DataModel.Enums.PlayerRank))) {
                this.chart.AxisY[0].Sections.Add(new LiveCharts.Wpf.AxisSection() { Value = RankedProgressChartViewModel.GetPlayerRankStartValue(pr)-2, SectionWidth = 2, Label = "Serpent", Fill = new SolidColorBrush(Colors.LightBlue) });
                this.chart.AxisY[0].Sections.Add(new LiveCharts.Wpf.AxisSection() { Value = RankedProgressChartViewModel.GetPlayerRankStartValue(pr), SectionWidth = 8, Label = pr.ToString(), Fill = null});
            }
        }
    }
}
