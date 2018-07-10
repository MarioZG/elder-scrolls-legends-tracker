using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.Services;
using ESLTracker.Utils;
using ESLTracker.Utils.IOWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.BusinessLogic.Cards
{
    public class CardsDatabaseFactory : ICardsDatabaseFactory
    {
        private IFileWrapper fileWrapper;
        private PathManager pathManager;

        static CardsDatabase _instance;

        public CardsDatabaseFactory(IFileWrapper fileWrapper, PathManager pathManager)
        {
            this.fileWrapper = fileWrapper;
            this.pathManager = pathManager;
        }

        public CardsDatabase LoadCardsDatabase()
        {
            var databasePath = pathManager.CardsDatabasePath;
            if (fileWrapper.Exists(databasePath))
            {
                CardsDatabase database = SerializationHelper.DeserializeJson<CardsDatabase>(System.IO.File.ReadAllText(databasePath));
                return database;
            }
            else
            {
                return null;
            }
        }

        public ICardsDatabase RealoadDB()
        {
            _instance = LoadCardsDatabase();
            return _instance;
        }

        public ICardsDatabase GetCardsDatabase()
        {
            if (_instance == null)
            {
                _instance = LoadCardsDatabase();
            }
            return _instance;
        }
    }
}
