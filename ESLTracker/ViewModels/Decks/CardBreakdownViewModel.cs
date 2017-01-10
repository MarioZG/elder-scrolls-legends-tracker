using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;
using ESLTracker.Utils.Extensions;
using LiveCharts;
using LiveCharts.Definitions.Series;
using LiveCharts.Wpf;

namespace ESLTracker.ViewModels.Decks
{
    public class CardBreakdownViewModel : ViewModelBase
    {
        private ObservableCollection<CardInstance> cardCollection;

        public ObservableCollection<CardInstance> CardCollection
        {
            get { return cardCollection; }
            set
            {
                cardCollection = value;
                cardCollection.CollectionChanged += CardCollection_CollectionChanged;
                RaisePropertyChangedEvent(String.Empty);
            }
        }

        private void CardCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //needed to refresh deck card breakdown control
            RaisePropertyChangedEvent(String.Empty);
        }

        public int? CardCount
        {
            get
            {
                return cardCollection?.Sum(c => c.Quantity);
            }
        }

        public int CardsValue
        {
            get
            {
                return SoulGemsHelper.CalculateCardsPurchaseValue(cardCollection);
            }
        }

        public SeriesCollection DeckColorsSeriesCollection
        {
            get
            {
                SeriesCollection sc = new SeriesCollection();
                if (cardCollection != null)
                {
                    foreach (DeckAttribute da in Enum.GetValues(typeof(DeckAttribute)))
                    {
                        decimal count = cardCollection.Where(ci => ci.Card.Attributes.Contains(da)).Sum(ci => ci.Quantity);
                        decimal doubleAttributeFix = cardCollection.Where(ci => ci.Card.Attributes.Count == 2 && ci.Card.Attributes.Contains(da)).Sum(ci => ci.Quantity);
                        count -= doubleAttributeFix / 2;
                        if (count > 0)
                        {
                            sc.Add(
                                  new StackedRowSeries
                                  {
                                      Title = da.ToString(),
                                      Values = new ChartValues<decimal> { count },
                                      StackMode = StackMode.Percentage,
                                      DataLabels = true,
                                      LabelsPosition = BarLabelPosition.Merged,
                                      Fill = ClassAttributesHelper.DeckAttributeColors[da].ToMediaBrush()
                                  }
                                  );
                        }
                    }
                    return sc;
                }
                return null;
            }
        }

        public SeriesCollection ManaCurveSeriesCollection
        {
            get
            {
                SeriesCollection sc = new SeriesCollection();
                ManaCurveMaxValue = 0;
                if (cardCollection != null)
                {
                    int maxCost = 7;
                    sc.Add(
                          new ColumnSeries
                          {
                              Title = "".ToString(),
                              Values = new ChartValues<int> { },
                              DataLabels = true,
                              LabelsPosition = BarLabelPosition.Top,
                          }
                          );
                    for (int cost = 0; cost <= maxCost; cost++)
                    {
                        int count = cardCollection.Where(ci => ci.Card.Cost == cost).Sum(ci => ci.Quantity);
                        if (cost == maxCost)
                        {
                            //add more expensive than maxCost
                            count += cardCollection.Where(ci => ci.Card.Cost > cost).Sum(ci => ci.Quantity);
                        }
                        ManaCurveMaxValue = Math.Max(ManaCurveMaxValue, count);
                        sc[0].Values.Add(count);
                    }
                    return sc;
                }
                return null;
            }
        }

        public SeriesCollection CardTypeSeriesCollection
        {
            get
            {
                SeriesCollection sc = new SeriesCollection();
                List<string> labels = new List<string>();
                if (cardCollection != null)
                {
                    sc.Add(
                          new RowSeries
                          {
                              Title = "".ToString(),
                              Values = new ChartValues<int> { },
                              //StackMode = StackMode.Percentage,
                              DataLabels = true,
                              LabelsPosition = BarLabelPosition.Merged,
                              Fill = System.Windows.Media.Brushes.LightSkyBlue
                          }
                      );
                    foreach (CardType ct in Enum.GetValues(typeof(CardType)))
                    {
                        int count = cardCollection.Where(ci => ci.Card.Type == ct).Sum(ci => ci.Quantity);
                        if (count > 0)
                        {
                            sc[0].Values.Add(count);
                            labels.Add(ct.ToString());
                        }
                    }
                    CardTypeLabelsCollection = labels.ToArray();
                    return sc;
                }
                return null;
            }
        }

        public string[] CardTypeLabelsCollection { get; set; }

        public int ManaCurveMaxValue { get; set; }

        public string CardTypeBreakdownText
        {
            get
            {
                StringBuilder text = new StringBuilder();
                if (cardCollection != null)
                {
                    foreach (CardType ct in Enum.GetValues(typeof(CardType)))
                    {
                        int count = cardCollection.Where(ci => ci.Card.Type == ct).Sum(ci => ci.Quantity);
                        if (count > 0)
                        {
                            text.AppendFormat("{0}: {1}   ", ct, count);
                        }
                    }
                    return text.ToString();
                }
                return null;
            }
        }
    }
}
