using System.Xml.Serialization;

namespace ESLTracker.DataModel.Enums
{
    public enum ArenaRank
    {
        [XmlEnum("1")]
        GrandChampion = 1,
        [XmlEnum("2")]
        Champion,
        [XmlEnum("3")]
        Hero,
        [XmlEnum("4")]
        Gladiator,
        [XmlEnum("5")]
        Warrior,
        [XmlEnum("6")]
        Myrmidon,
        [XmlEnum("7")]
        Bloodletter,
        [XmlEnum("8")]
        Brawler,
        [XmlEnum("9")]
        PitDog
    }
}