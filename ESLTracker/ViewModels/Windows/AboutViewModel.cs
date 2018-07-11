using ESLTracker.BusinessLogic.Cards;
using ESLTracker.DataModel;
using ESLTracker.Services;
using ESLTracker.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                return string.Format("{0} ({1} from {2:d})",
                    cardsDB.Version,
                    cardsDB.VersionInfo,
                    cardsDB.VersionDate);
            }
        }

        IApplicationService applicationService;
        ICardsDatabase cardsDB;
        ITracker tracker;

        public AboutViewModel(ITracker tracker, IApplicationService applicationService, ICardsDatabase cardsDB)
        {
            this.applicationService = applicationService;
            this.cardsDB = cardsDB;
            this.tracker = tracker;
        }
    }
}
