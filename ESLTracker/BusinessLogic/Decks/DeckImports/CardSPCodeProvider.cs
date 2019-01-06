using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.BusinessLogic.Decks.DeckImports
{
    public class CardSPCodeProvider : ICardSPCodeProvider
    {
        private Dictionary<string, string> codesDB;

        public string GetCardByCode(string code)
        {
            if(codesDB == null)
            {
                LoadCodesDB();
            }
            var kvp = codesDB.SingleOrDefault(kv => kv.Value == code);
            if (kvp.Key == default(KeyValuePair<string, string>).Key)
            {
                throw new MissingSPCodeException($"Missing SP code. Code={code}");
            }
            return kvp.Key;
        }

        public string GetCodeByCardName(string cardName)
        {
            if (codesDB == null)
            {
                LoadCodesDB();
            }
            if (! codesDB.ContainsKey(cardName))
            {
                throw new MissingSPCodeException($"Missing SP code. Card Name={cardName}");
            }
            return codesDB[cardName];

        }

        private void LoadCodesDB()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetName().Name + ".Resources." + "cardToCode.json";
            string result;
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }

            codesDB = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
        }
    }
}
