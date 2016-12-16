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
using System.Windows.Shapes;
using ESLTracker.DataModel;
using ESLTracker.Utils;

namespace ESLTracker
{
    /// <summary>
    /// Interaction logic for GameStatistics.xaml
    /// </summary>
    public partial class GameStatistics : Window
    {
        public GameStatistics()
        {
            InitializeComponent();

            foreach(DeckAttributes da in ClassAttributesHelper.Classes.Values)
            {
                DataGridTextColumn col = new DataGridTextColumn();
                col.Header = da.ToString();
                col.Binding = new Binding(da.ToString());

                DataTemplate cardLayout = new DataTemplate();
                cardLayout.DataType = typeof(GameStatistics);

                //set up the stack panel
                FrameworkElementFactory spFactory = new FrameworkElementFactory(typeof(StackPanel));
                spFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
                spFactory.SetValue(StackPanel.ToolTipProperty, da.ToString());

                //set up the card holder textblock
                FrameworkElementFactory cardHolder = new FrameworkElementFactory(typeof(Image));
                cardHolder.SetValue(Image.SourceProperty, new BitmapImage(new Uri(da.ImageSources.First(), UriKind.Absolute)));
                cardHolder.SetValue(Image.WidthProperty, 16.0);
                spFactory.AppendChild(cardHolder);

                if (da.ImageSources.Count() > 1)
                {
                    //set up the card holder textblock
                    FrameworkElementFactory cardHolder2 = new FrameworkElementFactory(typeof(Image));
                    cardHolder2.SetValue(Image.SourceProperty, new BitmapImage(new Uri(da.ImageSources.Skip(1).FirstOrDefault(), UriKind.Absolute)));
                    cardHolder2.SetValue(Image.WidthProperty, 16.0);
                    spFactory.AppendChild(cardHolder2);
                }

                //set the visual tree of the data template
                cardLayout.VisualTree = spFactory;

                col.HeaderTemplate = cardLayout;

                this.dataGrid.Columns.Add(col);
            }

        }
    }
}
