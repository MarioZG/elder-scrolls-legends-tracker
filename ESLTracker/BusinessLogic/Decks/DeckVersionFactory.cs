//using ESLTracker.DataModel;
//using ESLTracker.Utils;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ESLTracker.BusinessLogic.Decks
//{
//    public class DeckVersionFactory : IDeckVersionFactory
//    {
//        IGuidProvider guidProvider;

//        public DeckVersionFactory(IGuidProvider guidProvider)
//        {
//            this.guidProvider = guidProvider;
//        }

//        public DeckVersion CreateDeckVersion()
//        {
//            return new DeckVersion()
//            {
//                VersionId = guidProvider.GetNewGuid()
//            };
//        }

//        /// <summary>
//        /// Creates new deck version in history, adds to colletion and returns reference
//        /// </summary>
//        /// <param name="major"></param>
//        /// <param name="minor"></param>
//        /// <param name="createdDate"></param>
//        /// <returns></returns>
//        public DeckVersion CreateDeckVersion(Deck deck, int major, int minor, DateTime createdDate)
//        {
//            SerializableVersion version = new SerializableVersion(major, minor);
//            if (deck.DoNotUse.Any(v => v.Version == version))
//            {
//                throw new ArgumentException(string.Format("Version {0} alread has been added to deck '{1}'", version, deck.Name));
//            }
//            DeckVersion dv = new DeckVersion();
//            dv.VersionId = guidProvider.GetNewGuid();
//            dv.CreatedDate = createdDate;
//            dv.Version = version;
//            deck.DoNotUse.Add(dv); //add to history
//            deck.SelectedVersionId = dv.VersionId;
//            return dv;
//        }
//    }
//}
