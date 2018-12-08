using ESLTracker.DataModel;
using ESLTracker.Utils;
using ESLTracker.Utils.Extensions;
using ESLTracker.Utils.IOWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.BusinessLogic.Packs
{
    public class PacksDataCSVExporter
    {
        private readonly IWinDialogs winDialogs;
        private readonly IFileWrapper fileWrapper;

        public PacksDataCSVExporter(IWinDialogs winDialogs, IFileWrapper fileWrapper)
        {
            this.winDialogs = winDialogs;
            this.fileWrapper = fileWrapper;
        }

        public async Task<string> ExportToCSVFile(IEnumerable<Pack> dataToExport)
        {
            string targetCsvFile = winDialogs.SaveFileDialog(
               "PacksOpeningDetails" + DateTime.Now.ToString("yyyyMMdd") + ".csv",
               "Csv files(*.csv)|*.csv|All files(*.*)|*.*",
               true);

            bool targetFileSelected = !String.IsNullOrWhiteSpace(targetCsvFile);
            if (targetFileSelected)
            {
                string exportedData =  dataToExport
                        .SelectMany(p => p.Cards, (pack, ci) => new
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
                        .ToCsv();

                await Task.Factory.StartNew(() => fileWrapper.WriteAllText(targetCsvFile, exportedData));

            }
            return targetCsvFile;
        }
    }
}
