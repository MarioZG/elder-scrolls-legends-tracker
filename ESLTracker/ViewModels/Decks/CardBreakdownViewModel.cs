using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.BusinessLogic.Cards;
using ESLTracker.BusinessLogic.Decks;
using TESLTracker.DataModel;
using TESLTracker.DataModel.Enums;
using ESLTracker.Utils;
using ESLTracker.Utils.Extensions;
using LiveCharts;
using LiveCharts.Definitions.Series;
using LiveCharts.Wpf;
using TESLTracker.Utils;

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

        private CardBreakdown cardBreakdownService = new CardBreakdown();
        private SoulGemCalculator soulGemCalculator = new SoulGemCalculator();

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
                return soulGemCalculator.CalculateCardsPurchaseValue(cardCollection);
            }
        }

        public SeriesCollection DeckColorsSeriesCollection
        {
            get
            {
                SeriesCollection sc = new SeriesCollection();
                if (cardCollection != null)
                {
                    var breakdown = cardBreakdownService.GetCardsColorBreakdown(cardCollection);
                    foreach (var item in breakdown)
                    {
                        sc.Add(
                           new StackedRowSeries
                           {
                               Title = item.Key.ToString(),
                               Values = new ChartValues<decimal> { item.Value },
                               StackMode = StackMode.Percentage,
                               DataLabels = true,
                               LabelsPosition = BarLabelPosition.Parallel,
                               Fill = ClassAttributesHelper.DeckAttributeColors[item.Key].ToMediaBrush()
                           }
                           );
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
                sc.Add(
                   new ColumnSeries
                   {
                       Title = "".ToString(),
                       Values = new ChartValues<int> { },
                       DataLabels = true,
                       LabelsPosition = BarLabelPosition.Top,
                   }
                );
                if (cardCollection != null)
                {
                    var breakdown = cardBreakdownService.GetManaBreakdown(cardCollection);
                    foreach(var item in breakdown)
                    {                       
                        sc[0].Values.Add(item.Value);
                    }

                    ManaCurveMaxValue = breakdown.Max(i => i.Value);

                    return sc;
                }
                return null;
            }
        }

        public int ManaCurveMaxValue { get; set; } 

        public string CardTypeBreakdownText
        {
            get
            {
                StringBuilder text = new StringBuilder();
                if (cardCollection != null)
                {
                    var breakdown = cardBreakdownService.GetCardTypeBreakdown(cardCollection);

                    foreach (var item in breakdown)
                    {
                        text.AppendFormat("{0}: {1}   ", item.Key, item.Value);
                    }
                    return text.ToString();
                }
                return null;
            }
        }

        public string KeywordsBreakdownText
        {
            get
            {
                StringBuilder text = new StringBuilder();
                if (cardCollection != null)
                {
                    decimal count = cardCollection.Sum(ci => ci.Quantity);


                    var keywordsBreakdown = cardBreakdownService.GetCardKeywordsBreakdown<CardKeyword>(cardCollection, c => c.Keywords);
                    var mechanicsBreakdown = cardBreakdownService.GetCardKeywordsBreakdown<CardMechanic>(cardCollection, c => c.Mechanics);

                    foreach (var ct in keywordsBreakdown)
                    {
                        text.AppendFormat("{0}: {1} ({2}%)"+Environment.NewLine, ct.Key, ct.Value, Math.Round(ct.Value * 100 / count, 0));
                    }

                    foreach (var ct in mechanicsBreakdown)
                    {

                        text.AppendFormat("{0}: {1} ({2}%)" + Environment.NewLine, ct.Key, ct.Value, Math.Round(ct.Value * 100 / count, 0));
                    }
                    return text.ToString();
                }
                return null;
            }
        }
    }
}
