using ESLTracker.BusinessLogic.Cards;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.Properties;
using ESLTracker.Utils;
using ESLTracker.Utils.Extensions;
using ESLTracker.Utils.SimpleInjector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLTracker.BusinessLogic.DataFile
{
    public class FileLoader
    {
        private readonly PathManager pathManager;
        private readonly FileUpdater fileUpdater;
        private readonly ISettings settings;
        private readonly ILogger logger;
        private readonly ICardsDatabaseFactory cardsDatabaseFactory;

        public FileLoader(
            ISettings settings, 
            PathManager pathManager, 
            FileUpdater fileUpdater, 
            ILogger logger,
            ICardsDatabaseFactory cardsDatabaseFactory)
        {
            this.pathManager = pathManager;
            this.fileUpdater = fileUpdater;
            this.settings = settings;
            this.logger = logger;
            this.cardsDatabaseFactory = cardsDatabaseFactory;
        }

        public Tracker LoadDatabase(bool throwDataFileException = false)
        {
            logger?.Trace("LoadDatabase: Start loading tracker database");
            Tracker tracker = null;
            try
            {
                if (File.Exists(pathManager.FullDataFilePath))
                {
                    tracker = SerializationHelper.DeserializeXmlPath<Tracker>(pathManager.FullDataFilePath);

                    FixUpDataFileAfterDeserialisation(tracker);

                    //check for data update
                    if (tracker.Version < Tracker.CurrentFileVersion)
                    {
                        logger?.Trace("LoadDatabase: newer version detected - update on file needs to be performed");


                        if (fileUpdater.UpdateFile(tracker.Version, tracker))
                        {
                            //reload after update
                            tracker = LoadDatabase();
                        }
                        else
                        {
                            throw new DataFileException(string.Format("You are using old file format version and application cannot upgrade your file." + Environment.NewLine + Environment.NewLine + "File version={0}. Application works with {1}", tracker.Version, Tracker.CurrentFileVersion));
                        }
                    }
                    else if (tracker.Version > Tracker.CurrentFileVersion)
                    {
                        logger?.Trace("LoadDatabase: using newer file can suppers");

                        //using old application
                        throw new DataFileException(
                            string.Format("You are using old version of application. If you continue you might loose data!" + Environment.NewLine + Environment.NewLine + "Press Yes to start anyway (and potencailly loose data), No to cancel." + Environment.NewLine + Environment.NewLine + " File version={0}. Application works with {1}", tracker.Version, Tracker.CurrentFileVersion),
                            true);
                    }
                }
                else
                {
                    logger?.Trace("LoadDatabase: tracker db does not exist.Creating empty one");

                    tracker = new Tracker();
                    tracker.Version = Tracker.CurrentFileVersion;
                }
            }
            catch (DataFileException ex)
            {
                logger?.Trace(ex, "LoadDatabase: DataFileException");
                
                //Datafile isses should have been resolved on app init
                if (throwDataFileException) //should be true only in app init code
                {
                    throw;
                }
            }
            catch
            {
                if (tracker != null)
                {
                    if (tracker.Version != Tracker.CurrentFileVersion)
                    {
                        if (fileUpdater.UpdateFile(tracker.Version, tracker))
                        {
                            //reload after update
                            tracker = LoadDatabase();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
                else
                {
                    if (fileUpdater.UpdateFile())
                    {
                        //reload after update
                        tracker = LoadDatabase();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            logger?.Trace($"LoadDatabase: finished. Tracker version={tracker.Version}");

            return tracker;
        }

        // populates Card property on CardInsatnce object (due to removing dependies in datamdel namesapce)
        private void FixUpDataFileAfterDeserialisation(Tracker tracker)
        {
            var cardsDB = cardsDatabaseFactory.GetCardsDatabase();
            foreach (CardInstance ci in tracker.Decks.SelectMany(d => d.History.SelectMany(h => h.Cards)))
            {
                ci.Card = cardsDB.FindCardById(ci.CardId);
            }

            foreach (CardInstance ci in tracker.Packs.SelectMany(p => p.Cards))
            {
                ci.Card = cardsDB.FindCardById(ci.CardId);
            }

            foreach (CardInstance ci in tracker.Rewards.Where(r => r.Type == RewardType.Card).Select(r => r.CardInstance))
            {
                ci.Card = cardsDB.FindCardById(ci.CardId);
            }
        }
    }
}
