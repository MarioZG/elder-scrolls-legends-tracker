using ESLTracker.BusinessLogic.Cards;
using ESLTracker.DataModel;
using ESLTracker.Properties;
using ESLTracker.Utils;
using ESLTracker.Utils.Extensions;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ESLTracker.ViewModels.PackStatistics
{
    public class OpeningPackStatsWindowViewModel : FilterDateViewModel
    {
        public IAsyncCommand<object> CommandExportToCsv { get; private set; }
        public IAsyncCommand<object> CommandOpenCsv { get; private set; }

        public string TargetCsvFile { get; set; }

        private readonly CardSet AllFilter = new CardSet() { Name = "All"};

        private CardSet packSetFilter;
        public CardSet PackSetFilter
        {
            get { return packSetFilter; }
            set { SetProperty<CardSet>(ref packSetFilter, value) ; RaiseDataPropertyChange(); }
        }

        public IEnumerable<CardSet> PackSetAutocomplete
        {
            get { return new CardSet[] { AllFilter }.Union(cardsDatabase.CardSets.Where(cs => cs.HasPacks)).ToList(); }
        }

        private readonly ICardImage cardImageService;
        private readonly IWinDialogs winDialogs;
        private readonly ICardsDatabase cardsDatabase;
        private readonly ICardInstanceFactory cardInstanceFactory;
        private readonly ILogger logger;
        private readonly ITracker tracker;

        public OpeningPackStatsWindowViewModel(
            ILogger logger,
            ISettings settings,
            IDateTimeProvider dateTimeProvider, 
            ITracker tracker, 
            ICardInstanceFactory cardInstanceFactory,
            ICardImage cardImageService,
            IWinDialogs winDialogs,
            ICardsDatabase cardsDatabase) : base(settings, dateTimeProvider)
        {
            this.logger = logger;
            this.tracker = tracker;
            this.cardInstanceFactory = cardInstanceFactory;
            this.cardImageService = cardImageService;
            this.winDialogs = winDialogs;
            this.cardsDatabase = cardsDatabase; ;

            CommandExportToCsv = new RealyAsyncCommand<object>(CommandExportToCsvExecute);
            CommandOpenCsv = new RealyAsyncCommand<object>(CommandOpenCsvExcute);

            packSetFilter = AllFilter;
        }

        protected override void RaiseDataPropertyChange()
        {
            base.RaiseDataPropertyChange();
            RaisePropertyChangedEvent(nameof(PieChartByClass));
            RaisePropertyChangedEvent(nameof(PieChartByRarity));
            RaisePropertyChangedEvent(nameof(PieChartPremiumByRarity));
            RaisePropertyChangedEvent(nameof(Top10Cards));
            RaisePropertyChangedEvent(nameof(GetPacksInDateRange));
        }

        public dynamic PieChartByClass
        {
            get
            {
                logger.Trace($"PieChartByClass");
                var rawData = ((IEnumerable<CardInstance>)GetDataSet())
                    .SelectMany(c => c.Card.Attributes, ( ci, a) => new { Attribute = a, Qty = (decimal) 1 / ci.Card.Attributes.Count});
                decimal totalCount = rawData.Sum( d=> d.Qty);
                var data = rawData
                    .GroupBy(c => c.Attribute)
                    .OrderBy(c => c.Key)
                    .Select(c => new PieSeries
                    {
                        Title = $"{ c.Key.ToString()} { Math.Round(c.Sum(d => d.Qty) / totalCount * 100, 2)}% ({c.Sum(d => d.Qty).ToString("0.#")})",
                        Fill = ClassAttributesHelper.DeckAttributeColors[c.Key].ToMediaBrush(),
                        Values = new ChartValues<decimal>() { c.Sum(d => d.Qty) },
                        DataLabels = false,
                        LabelPoint = chartPoint => string.Format("{0:P}", chartPoint.Participation)
                    });

                SeriesCollection sc = new SeriesCollection();

                sc.AddRange(data);
                return sc;
            }
        }

        public dynamic PieChartByRarity
        {
            get
            {
                logger.Trace($"PieChartByRarity");
                var rawData = ((IEnumerable<CardInstance>)GetDataSet())
                    .Select(c => c.Card.Rarity);
                int totalCount = rawData.Count();
                var data = rawData
                    .GroupBy(c => c)
                    .OrderBy(c=> c.Key)
                    .Select(c => new PieSeries
                    {
                        Title = $"{ c.Key.ToString()} { Math.Round((decimal)c.Count()/totalCount*100, 2)}% ({c.Count()})",
                        Fill = cardImageService.GetRarityBrush(c.Key),
                        Values = new ChartValues<int>() { c.Count() },
                        DataLabels = false,
                        LabelPoint = chartPoint => string.Format("{0:P}", chartPoint.Participation),
                        
                    });

                SeriesCollection sc = new SeriesCollection();

                sc.AddRange(data);
                return sc;
            }
        }

        public dynamic PieChartPremiumByRarity
        {
            get
            {
                logger.Trace($"PieChartPremiumByRarity");
                var rawData = ((IEnumerable<CardInstance>)GetDataSet())
                    .Where(ci=> ci.IsPremium)
                    .Select(c => c.Card.Rarity);
                int totalCount = rawData.Count();
                var data = rawData
                    .GroupBy(c => c)
                    .OrderBy(c => c.Key)
                    .Select(c => new PieSeries
                    {
                        Title = $"{ c.Key.ToString()} { Math.Round((decimal)c.Count() / totalCount * 100, 2)}% ({c.Count()})",
                        Fill = cardImageService.GetRarityBrush(c.Key),
                        Values = new ChartValues<int>() { c.Count() },
                        DataLabels = false,
                        LabelPoint = chartPoint => string.Format("{0:P}", chartPoint.Participation),

                    });

                SeriesCollection sc = new SeriesCollection();

                sc.AddRange(data);
                return sc;
            }
        }

        private ObservableCollection<CardInstance> top10Cards = new ObservableCollection<CardInstance>();
        public dynamic Top10Cards
        {
            get
            {
                logger.Trace($"Top10Cards");
                var rawData = ((IEnumerable<CardInstance>)GetDataSet())
                    .GroupBy(ci => ci.Card)
                    .Select(ci => cardInstanceFactory.CreateFromCard(ci.Key, ci.Count()))
                    .OrderByDescending(cis => cis.Quantity)
                    .Take(10);

                top10Cards.Clear();
                rawData.All(ci => { top10Cards.Add(ci); return true; });
                return top10Cards;
            }
        }

        public override dynamic GetDataSet()
        {
            logger.Trace($"GetDataSet");
            logger.Trace($"Filtering packs from={this.filterDateFrom}; to={this.filterDateTo};");

            var dataSet = GetPacksInDateRange
                .SelectMany(p => p.Cards);

            logger.Trace($"DataSet.Count={dataSet.Count()}");

            return dataSet;
        }

        public IEnumerable<Pack> GetPacksInDateRange
        {
            get
            {
                return tracker.Packs
                                .Where(p => (p.DateOpened > this.FilterDateFrom) 
                                    && (p.DateOpened.Date <= this.FilterDateTo.Date)
                                    && (PackSetFilter?.Id == Guid.Empty || p.CardSet.Id == PackSetFilter.Id));
            }
        }

        private Task<object> CommandExportToCsvExecute(object arg)
        {

            string targetCsvFile = winDialogs.SaveFileDialog(
                "PacksOpeningDetails" + DateTime.Now.ToString("yyyyMMdd")+".csv",
                "Csv files(*.csv)|*.csv|All files(*.*)|*.*",
                true);

            if (!String.IsNullOrWhiteSpace(targetCsvFile))
            {
                System.IO.File.WriteAllText(
                    targetCsvFile,
                    GetPacksInDateRange
                        .SelectMany(p=> p.Cards, (pack, ci) => new
                        {
                            pack.DateOpened,
                            ci.Card.Name,
                            ci.Card.Rarity,
                            ci.Card.IsUnique,
                            ci.IsPremium,
                            ci.Card.Race,
                            ci.Card.Set,
                            ci.Card.Type,
                            Agility = ci.Card.Attributes.Contains(DataModel.Enums.DeckAttribute.Agility) ? (ci.Card.Attributes.Count() == 2 ? 0.5 : 1) : 0,
                            Endurance = ci.Card.Attributes.Contains(DataModel.Enums.DeckAttribute.Endurance) ? (ci.Card.Attributes.Count() == 2 ? 0.5 : 1) : 0,
                            Intelligence = ci.Card.Attributes.Contains(DataModel.Enums.DeckAttribute.Intelligence) ? (ci.Card.Attributes.Count() == 2 ? 0.5 : 1) : 0,
                            Neutral = ci.Card.Attributes.Contains(DataModel.Enums.DeckAttribute.Neutral) ? (ci.Card.Attributes.Count() == 2 ? 0.5 : 1) : 0,
                            Strength = ci.Card.Attributes.Contains(DataModel.Enums.DeckAttribute.Strength) ? (ci.Card.Attributes.Count() == 2 ? 0.5 : 1) : 0,
                            Willpower = ci.Card.Attributes.Contains(DataModel.Enums.DeckAttribute.Willpower) ? (ci.Card.Attributes.Count() == 2 ? 0.5 : 1) : 0,
                        })
                    .ToCsv());

                TargetCsvFile = targetCsvFile;
                RaisePropertyChangedEvent(nameof(TargetCsvFile));
            }


            return null;
        }

        private Task<object> CommandOpenCsvExcute(object arg)
        {
             Process.Start(TargetCsvFile);

            return null;
        }
    }
}
