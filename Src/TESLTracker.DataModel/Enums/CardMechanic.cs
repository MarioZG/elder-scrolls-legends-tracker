using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TESLTracker.DataModel.Enums
{
    public enum CardMechanic
    {
        Prophecy = 1,
        LastGasp = 2,
        Summon = 4,
        Pilfer = 8,

        //dark brotherhood
        Slay = 16,

        //heros of skyrim
        BeastForm = 32,

        //return to clockwork city
        TreasureHunt = 64,
        Assemble = 128,

        //houses of morrowind
        Betray = 256,
        Plot = 512,
        Exalt = 1024,

        //Aliance wars
        Empower = 2048,
        Expertise = 4096,
        Veteran = 80192,
        // 16384

        //32768
    }
}
