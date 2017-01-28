using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
using ESLTracker.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ESLTrackerTests
{
    public class BaseTest
    {
        private TestContext testContextInstance;
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        protected Dictionary<Type, object> StartProp = new Dictionary<Type, object>()
            {
                { typeof(Guid), Guid.NewGuid() },
                { typeof(string), "start value" },
                { typeof(DeckType), DeckType.SoloArena },
                { typeof(DeckAttributes), new DeckAttributes() { DeckAttribute.Intelligence } },
                { typeof(DeckClass?), DeckClass.Crusader },
                { typeof(DateTime), DateTime.Now },
#pragma warning disable CS0618 // Type or member is obsolete
                { typeof(Deck), new Deck()},
#pragma warning restore CS0618 // Type or member is obsolete
                { typeof(GameType?), GameType.SoloArena},
                { typeof(bool?), true},
                { typeof(bool), true},
                { typeof(OrderOfPlay?), OrderOfPlay.Second},
                { typeof(GameOutcome), GameOutcome.Disconnect},
                { typeof(PlayerRank?), PlayerRank.TheMage},
                { typeof(int?), 234},
                { typeof(int), 2234},
                { typeof(ArenaRank?), ArenaRank.Level5},
                { typeof(SerializableVersion), new SerializableVersion(1,2,3,4)},
                { typeof(ObservableCollection<DeckVersion>), new ObservableCollection<DeckVersion>() {new DeckVersion() { Version = new SerializableVersion(1,2) } } },
                { typeof(ObservableCollection<CardInstance>), new ObservableCollection<CardInstance>() {new CardInstance(new Card() {Name = "Card start" }) } },
                { typeof(PropertiesObservableCollection<CardInstance>), new PropertiesObservableCollection<CardInstance>() {new CardInstance(new Card() {Name = "Card edit" }) } },
                { typeof(Card), Card.Unknown},
                { typeof(System.Windows.Media.Brush), System.Windows.Media.Brushes.Aquamarine},
                { typeof(object), new object()}
            };

        protected Dictionary<Type, object> EditProp = new Dictionary<Type, object>()
            {
                { typeof(Guid), Guid.NewGuid() },
                { typeof(string), "modified value" },
                { typeof(DeckType), DeckType.VersusArena },
                { typeof(DeckAttributes), new DeckAttributes() { DeckAttribute.Endurance, DeckAttribute.Strength } },
                { typeof(DeckClass?), DeckClass.Monk },
                { typeof(DateTime), DateTime.Now.AddDays(-4) },
#pragma warning disable CS0618 // Type or member is obsolete
                { typeof(Deck), new Deck()},
#pragma warning restore CS0618 // Type or member is obsolete
                { typeof(GameType?), GameType.PlayCasual},
                { typeof(bool?), false},
                { typeof(bool), false},
                { typeof(OrderOfPlay?), OrderOfPlay.First},
                { typeof(GameOutcome), GameOutcome.Draw},
                { typeof(PlayerRank?), PlayerRank.TheLover},
                { typeof(int?), 567},
                { typeof(int), 1567},
                { typeof(ArenaRank?), ArenaRank.Gladiator },
                { typeof(SerializableVersion), new SerializableVersion(1,2,3,4)},
                { typeof(ObservableCollection<DeckVersion>), new ObservableCollection<DeckVersion>() {new DeckVersion() { Version = new SerializableVersion(2,3) } } },
                { typeof(ObservableCollection<CardInstance>), new ObservableCollection<CardInstance>() {new CardInstance(new Card() {Name = "Card edit" }) } },
                { typeof(PropertiesObservableCollection<CardInstance>), new PropertiesObservableCollection<CardInstance>() {new CardInstance(new Card() {Name = "Card edit" }) } },
                { typeof(Card), new Card() {Name = "Other unknown card" } },
                { typeof(System.Windows.Media.Brush), System.Windows.Media.Brushes.Gainsboro},
             //   { typeof(object), new object()}
            };

        protected void PopulateObject(object obj, Dictionary<Type, object> values)
        {
            foreach (PropertyInfo p in obj.GetType().GetProperties())
            {
                if (p.CanWrite)
                {
                    TestContext.WriteLine("Setting prop {0} of type {1}", p.Name, p.PropertyType);
                    p.SetValue(obj, values[p.PropertyType]);
                }
            }
        }

        protected ObservableCollection<Game> GenerateGamesList(
            Deck deck, 
            int victories = 0, 
            int defeats = 0, 
            int draws = 0, 
            int disconnects = 0,
            GameType? gameType = null)
        {
            return new System.Collections.ObjectModel.ObservableCollection<Game>(
                    Enumerable.Range(0, disconnects).Select(x => new Game() { Deck = deck, Outcome = GameOutcome.Disconnect, Type = gameType }).Union(
                    Enumerable.Range(0, defeats).Select(x => new Game() { Deck = deck, Outcome = GameOutcome.Defeat, Type = gameType }).Union(
                    Enumerable.Range(0, draws).Select(x => new Game() { Deck = deck, Outcome = GameOutcome.Draw, Type = gameType }).Union(
                    Enumerable.Range(0, victories).Select(x => new Game() { Deck = deck, Outcome = GameOutcome.Victory, Type = gameType })
                    )))
                );
        }

        protected IEnumerable<FieldInfo> GetAllFields(Type t)
        {
            if (t == null)
                return Enumerable.Empty<FieldInfo>();

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                                 BindingFlags.Static | BindingFlags.Instance |
                                 BindingFlags.DeclaredOnly;
            return t.GetFields(flags).Concat(GetAllFields(t.BaseType));
        }

    }
}
