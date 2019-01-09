using ESLTracker.BusinessLogic.Cards;
using ESLTracker.BusinessLogic.General;
using TESLTracker.DataModel;
using ESLTracker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TESLTracker.Utils;

namespace ESLTracker.ViewModels.Windows
{
    public class AboutViewModel : ViewModelBase
    {
        public string AppVersion
        {
            get
            {
                return applicationService.GetAssemblyInformationalVersion();
            }
        }

        public SerializableVersion FileVersion
        {
            get
            {
                return tracker.Version;
            }
        }
        public string Address
        {
            get
            {
                return new String(
                    (((char)(109)).ToString() +
                    ((char)(111)).ToString() +
                    ((char)(99)).ToString() +
                    ((char)(46)).ToString() +
                    ((char)(108)).ToString() +
                    ((char)(105)).ToString() +
                    ((char)(97)).ToString() +
                    ((char)(109)).ToString() +
                    ((char)(103)).ToString() +
                    ((char)(64)).ToString() +
                    ((char)(114)).ToString() +
                    ((char)(101)).ToString() +
                    ((char)(107)).ToString() +
                    ((char)(99)).ToString() +
                    ((char)(97)).ToString() +
                    ((char)(114)).ToString() +
                    ((char)(116)).ToString() +
                    ((char)(108)).ToString() +
                    ((char)(115)).ToString() +
                    ((char)(101)).ToString() +
                    ((char)(116)).ToString() +
                    ((char)(58)).ToString() +
                    ((char)(111)).ToString() +
                    ((char)(116)).ToString() +
                    ((char)(108)).ToString() +
                    ((char)(105)).ToString() +
                    ((char)(97)).ToString() +
                    ((char)(109)).ToString())
                    .Reverse().ToArray());
            }
        }

        public string SendMail
        {
            get
            {
                return Address + "?body=Application version: " + this.AppVersion;
            }
        }

        public string CardsDatabaseVersion
        {
            get
            {
                var cardsDb = cardsDatabasefactory.GetCardsDatabase();
                return string.Format("{0} ({1} from {2:d})",
                    cardsDb.Version,
                    cardsDb.VersionInfo,
                    cardsDb.VersionDate);
            }
        }

        IApplicationInfo applicationService;
        ICardsDatabaseFactory cardsDatabasefactory;
        ITracker tracker;

        public AboutViewModel(ITracker tracker, IApplicationInfo applicationService, ICardsDatabaseFactory cardsDatabasefactory)
        {
            this.applicationService = applicationService;
            this.cardsDatabasefactory = cardsDatabasefactory;
            this.tracker = tracker;
        }
    }
}
