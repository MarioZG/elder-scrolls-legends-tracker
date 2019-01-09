using ESLTracker.BusinessLogic.Cards;
using ESLTracker.BusinessLogic.Packs;
using TESLTracker.DataModel;
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

        private CardSet packSetFilter;
        public CardSet PackSetFilter
        {
            get { return packSetFilter; }
            set { SetProperty(ref packSetFilter, value) ; RaiseDataPropertyChange(); }
        }

        private readonly ILogger logger;
        private readonly ITracker tracker;
        private readonly PacksChartsDataCalculator packsChartsDataCalculator;
        private readonly PacksDataCSVExporter packsDataCSVExporter;

        public OpeningPackStatsWindowViewModel(
            ILogger logger,
            ISettings settings,
            IDateTimeProvider dateTimeProvider, 
            ITracker tracker,
            PacksDataCSVExporter packsDataCSVExporter,
            PacksChartsDataCalculator packsChartsDataCalculator) : base(settings, dateTimeProvider)
        {
            this.logger = logger;
            this.tracker = tracker;
            this.packsChartsDataCalculator = packsChartsDataCalculator;
            this.packsDataCSVExporter = packsDataCSVExporter;

            CommandExportToCsv = new RealyAsyncCommand<object>(CommandExportToCsvExecute);
            CommandOpenCsv = new RealyAsyncCommand<object>(CommandOpenCsvExcute);

            packSetFilter = CardSetsListProvider.AllSets;
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
                return packsChartsDataCalculator.GetPieChartByClassData(GetDataSet());
            }
        }

        public dynamic PieChartByRarity
        {
            get
            {
                logger.Trace($"PieChartByRarity");
                return packsChartsDataCalculator.GetPieChartByRarityData(GetDataSet());
            }
        }

        public dynamic PieChartPremiumByRarity
        {
            get
            {
                logger.Trace($"PieChartPremiumByRarity");
                return packsChartsDataCalculator.GetPieChartPremiumByRarityData(GetDataSet());
            }
        }

        private ObservableCollection<CardInstance> top10Cards = new ObservableCollection<CardInstance>();
        public dynamic Top10Cards
        {
            get
            {
                logger.Trace($"Top10Cards");
                IEnumerable<CardInstance> top10CardsData = packsChartsDataCalculator.GetTopXCardsData(GetDataSet(), 10);

                top10Cards.Clear();
                top10CardsData.All(ci => { top10Cards.Add(ci); return true; });
                return top10Cards;
            }
        }

        public override dynamic GetDataSet()
        {
            logger.Trace($"GetDataSet");
            logger.Trace($"Filtering packs from={this.filterDateFrom}; to={this.filterDateTo};");

            var dataSet = GetPacksInDateRange.SelectMany(p => p.Cards);

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

        private async Task<object> CommandExportToCsvExecute(object arg)
        {
            string targetCsvFile = await packsDataCSVExporter.ExportToCSVFile(GetPacksInDateRange);
            if (!String.IsNullOrWhiteSpace(targetCsvFile))
            {
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
