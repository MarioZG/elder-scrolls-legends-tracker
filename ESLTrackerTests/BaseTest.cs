using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.DataModel;
using ESLTracker.DataModel.Enums;
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
                { typeof(Deck), new Deck()},
                { typeof(GameType?), GameType.SoloArena},
                { typeof(bool?), true},
                { typeof(OrderOfPlay?), OrderOfPlay.Second},
                { typeof(GameOutcome), GameOutcome.Disconnect},
                { typeof(PlayerRank?), PlayerRank.TheMage},
                { typeof(int?), 234},
                { typeof(object), new object()}
            };

        protected Dictionary<Type, object> EditProp = new Dictionary<Type, object>()
            {
                { typeof(Guid), Guid.NewGuid() },
                { typeof(string), "modified value" },
                { typeof(DeckType), DeckType.VersusArena },
                { typeof(DeckAttributes), new DeckAttributes() { DeckAttribute.Endurance, DeckAttribute.Strength } },
                { typeof(DeckClass?), DeckClass.Monk },
                { typeof(DateTime), DateTime.Now.AddDays(-4) }
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

    }
}
