using ESLTracker.BusinessLogic.DataFile;
using ESLTracker.Utils;
using ESLTracker.Utils.IOWrappers;
using ESLTracker.Utils.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.BusinessLogic.Cards
{
    public class CardsDatabaseFactory : ICardsDatabaseFactory
    {
        private readonly IFileWrapper fileWrapper;
        private readonly PathManager pathManager;
        private readonly IMessenger messenger;

        static CardsDatabase _instance;

        public CardsDatabaseFactory(IFileWrapper fileWrapper, PathManager pathManager, IMessenger messenger)
        {
            this.fileWrapper = fileWrapper;
            this.pathManager = pathManager;
            this.messenger = messenger;
        }

        public CardsDatabase LoadCardsDatabase()
        {
            var databasePath = pathManager.CardsDatabasePath;
            if (fileWrapper.Exists(databasePath))
            {
                CardsDatabase database = SerializationHelper.DeserializeJson<CardsDatabase>(File.ReadAllText(databasePath));
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
            messenger.Send<CardsDbReloaded>(new CardsDbReloaded(_instance));
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

        public ICardsDatabase UpdateCardsDB(string newContent, Version currentVersion)
        {
            string fileName = ".\\Resources\\cards.json";

            string backupFileName = string.Format("{0}_{1}{2}",
                Path.GetFileNameWithoutExtension(fileName),
                currentVersion,
                Path.GetExtension(fileName)); //includes . 
            backupFileName = Path.Combine(Path.GetDirectoryName(fileName), backupFileName);

            if (fileWrapper.Exists(backupFileName))
            {
                fileWrapper.Delete(backupFileName);
            }
            fileWrapper.Move(fileName, backupFileName);

            fileWrapper.WriteAllText(fileName, newContent);
            return RealoadDB();
        }
    }
}
