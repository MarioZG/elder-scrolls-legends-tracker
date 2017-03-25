using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using System.Data.SQLite;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace ESLTracker.Utils
{
    public class CardsDatabase : ICardsDatabase
    {

        static CardsDatabase _instance;
        [Obsolete("Use IFactory to obtain instance")]
        public static CardsDatabase Default
        {
            get
            {
                if (_instance == null)
                {
                    _instance = LoadCardsDatabase("./Resources/cards.json");
                }
                return _instance;
            }
        }

        public IEnumerable<string> CardsNames {
            get
            {
                return Cards.Select(c => c.Name);
            }
        }

        IEnumerable<Card> cards;
        public IEnumerable<Card> Cards
        {
            get
            {
                return cards;
            }
            set
            {
                cards = value;
            }
        }

        public Version Version { get; set; }
        public string VersionInfo { get; set; }
        public DateTime VersionDate { get; set; }

        private CardsDatabase()
        {

        }

        public static CardsDatabase LoadCardsDatabase(string datbasePath)
        {
            CardsDatabase database = SerializationHelper.DeserializeJson<CardsDatabase>(System.IO.File.ReadAllText(datbasePath));
            database.GenerateImages();
            database.LoadLocalization();
            return database;
        }

        private void LoadLocalization()
        {
            string path = Environment.ExpandEnvironmentVariables(
                @"%USERPROFILE%\AppData\LocalLow\Dire Wolf Digital\The Elder Scrolls_ Legends\LocalizationDB.db");

            SQLiteFactory factory = (SQLiteFactory)DbProviderFactories.GetFactory("System.Data.SQLite");
            using (SQLiteConnection connection = (SQLiteConnection)factory.CreateConnection())
            {
                connection.ConnectionString = "Data Source = " + path;
                connection.Open();
                SQLiteCommand command = new SQLiteCommand("SELECT * FROM 'Lookup' ORDER BY value ASC; ", connection);
                SQLiteDataReader reader = command.ExecuteReader();
                foreach (DbDataRecord record in reader)
                {
                    string id = record["key"].ToString();
                    if (id.Contains("-title"))
                    {
                        id = id.Replace("-title", "");
                        try{
                            cards.Where(c => c.Id == Guid.Parse(id)).First().Name = record["value"].ToString();
                        }
                        catch { }
                    }
                    else if (id.Contains("-game_text"))
                    {
                        id = id.Replace("-game_text", "");
                        try
                        {
                            cards.Where(c => c.Id == Guid.Parse(id)).First().Text = HtmlToPlainText(record["value"].ToString());
                        }
                        catch { }
                    }
                }
            }
        }

        private void GenerateImages()
        {
            Regex rgx = new Regex("[^a-zA-Z0-9]");
            foreach (var card in cards)
            {
                var name = rgx.Replace(card.Name, "");
                card.ImageName = "pack://application:,,,/Resources/Cards/" + name + ".png";
                 
            }
        }
        private static string HtmlToPlainText(string html)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
            const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

            var text = html;
            //Decode html specific characters
            text = System.Net.WebUtility.HtmlDecode(text);
            //Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");
            //Replace <br /> with line breaks
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            //Strip formatting
            text = stripFormattingRegex.Replace(text, string.Empty);

            return text;
        }

        public Card FindCardByName(string name)
        {
            return Cards.Where(c => c.Name.ToLower().Trim() == name.ToLower().Trim()).DefaultIfEmpty(Card.Unknown).FirstOrDefault();
        }

        public Card FindCardById(Guid value)
        {
            return Cards.Where(c => c.Id == value).DefaultIfEmpty(Card.Unknown).FirstOrDefault();
        }

        public void RealoadDB()
        {
            _instance = LoadCardsDatabase("./Resources/cards.json");
        }
    }
}
