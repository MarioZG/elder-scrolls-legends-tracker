using System.Xml.Serialization;

namespace ESLTracker.DataModel.Enums
{
    public enum ArenaRank
    {
        [XmlEnum("1")]
        Level1 = 1,
        [XmlEnum("2")]
        Level2,
        [XmlEnum("3")]
        Level3,
        [XmlEnum("4")]
        Gladiator,
        [XmlEnum("5")]
        Level5,
        [XmlEnum("6")]
        Level6,
        [XmlEnum("7")]
        Level7,
        [XmlEnum("8")]
        Brawler,
        [XmlEnum("9")]
        Level9
    }
}