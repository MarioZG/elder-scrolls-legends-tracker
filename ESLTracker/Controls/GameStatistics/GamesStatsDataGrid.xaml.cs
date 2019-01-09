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
using System.Windows.Threading;
using TESLTracker.DataModel;
using ESLTracker.Utils;
using ESLTracker.Utils.Converters;
using ESLTracker.Utils.SimpleInjector;
using ESLTracker.ViewModels;

namespace ESLTracker.Controls.GameStatistics
{
    /// <summary>
    /// Interaction logic for GamesStatsDataGrid.xaml
    /// </summary>
    public partial class GamesStatsDataGrid : UserControl
    {

        private List<DataGridTextColumn> dynamicColumns = new List<DataGridTextColumn>();

        public GamesStatsDataGrid()
        {
            InitializeComponent();

            CreateClassColumns();

            IMessenger messenger = MasserContainer.Container.GetInstance<IMessenger>();
            messenger.Register<Utils.Messages.GameStatsOpponentGroupByChanged>(this, UpdateGridHeaders);
        }

        private void UpdateGridHeaders(Utils.Messages.GameStatsOpponentGroupByChanged obj)
        {
            this.dataGrid.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new Action(delegate ()
                    {
                        UpdateGridHeadersExecute(obj);
                    })
                );
        }

        private void UpdateGridHeadersExecute(Utils.Messages.GameStatsOpponentGroupByChanged obj)
        { 
            foreach (var col in dynamicColumns)
            {
                this.dataGrid.Columns.Remove(col);
            }
            dynamicColumns.Clear();

            if (obj.OpponentGroupBy == "class")
            {
                CreateClassColumns();
            }
            else if (obj.OpponentGroupBy == "opponentDeckTag")
            {
                CreateTagColumns(obj.Tags);
            }
            else
            {
                throw new NotImplementedException("opponent headers missing for " + obj.OpponentGroupBy);
            }
        }

        private void CreateTagColumns(IEnumerable<string> tags)
        {
            int totalColumnIndex = this.dataGrid.Columns.IndexOf(this.classColumnsPlaceholder);

            GameStatsGetOpponentTagValue converter = new GameStatsGetOpponentTagValue();
            foreach (string tag in tags)
            {
                DataGridTextColumn col = new DataGridTextColumn();
                col.Header = tag;
                col.Binding = new Binding("Tags")
                            { Mode = BindingMode.OneTime,
                            Converter = converter,
                            ConverterParameter = tag};
               // col.Binding = new Binding("Tags[0]")
                col.CanUserSort = false;

                //DataTemplate cardLayout = new DataTemplate();
                //cardLayout.DataType = typeof(GamesStatsDataGrid);

                ////set up the stack panel
                //FrameworkElementFactory spFactory = new FrameworkElementFactory(typeof(StackPanel));
                //spFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
                //spFactory.SetValue(StackPanel.ToolTipProperty, tag);

                ////set up the card holder textblock
                //FrameworkElementFactory cardHolder = new FrameworkElementFactory(typeof(Image));
                //cardHolder.SetValue(Image.SourceProperty, new BitmapImage(new Uri(da.ImageSources.First(), UriKind.Absolute)));
                //cardHolder.SetValue(Image.WidthProperty, 16.0);
                //spFactory.AppendChild(cardHolder);

                ////set the visual tree of the data template
                //cardLayout.VisualTree = spFactory;

                //col.HeaderTemplate = cardLayout;

                dynamicColumns.Add(col);
                this.dataGrid.Columns.Insert(++totalColumnIndex, col);
            }
        }

        private void CreateClassColumns()
        {
            int totalColumnIndex = this.dataGrid.Columns.IndexOf(this.classColumnsPlaceholder);

            foreach (DeckAttributes da in ClassAttributesHelper.Classes.Values)
            {
                DataGridTextColumn col = new DataGridTextColumn();
                col.Header = da.ToString();
                col.Binding = new Binding(da.ToString());
                col.CanUserSort = false;

                DataTemplate cardLayout = new DataTemplate();
                cardLayout.DataType = typeof(GamesStatsDataGrid);

                //set up the stack panel
                FrameworkElementFactory spFactory = new FrameworkElementFactory(typeof(StackPanel));
                spFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
                spFactory.SetValue(StackPanel.ToolTipProperty, da.ToString());


                foreach (var attributeImage in da.ImageSources)
                {
                    //set up the card holder textblock
                    FrameworkElementFactory cardHolder = new FrameworkElementFactory(typeof(Image));
                    cardHolder.SetValue(Image.SourceProperty, new BitmapImage(new Uri(attributeImage, UriKind.Absolute)));
                    cardHolder.SetValue(Image.WidthProperty, 16.0);
                    spFactory.AppendChild(cardHolder);
                }

                //set the visual tree of the data template
                cardLayout.VisualTree = spFactory;

                col.HeaderTemplate = cardLayout;

                dynamicColumns.Add(col);
                this.dataGrid.Columns.Insert(++totalColumnIndex, col);
            }
        }
    }
}
