using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Data
{
    /// <summary>
    /// job const id
    /// </summary>
    public enum Job
    {
        Novice = 0,
        Joker = 120,
#region F系
        SwordMan = 1,
        BladeMaster = 3,
        Bountyhunter = 5,
        Gladiator = 7,
        Fencer = 11,
        Knight = 13,
        Darkstalker = 15,
        Guardian = 17,
        Scout = 21,
        Assassin = 23,
        Commando = 25,
        Eraser = 27,
        Archer = 31,
        Striker = 33,
        Gunner = 35,
        Hawkeye = 37,
#endregion

#region SU系
        Wizard = 41,
        Sorcerer = 43,
        Sage = 45,
        ForceMaster = 47,
        Shaman = 51,
        Elementaler = 53,
        Enchanter = 55,
        Astralist = 57,
        Wotes = 61,
        Druid = 63,
        Bard = 65,
        Cardinal = 67,
        Warlock = 71,
        Cabalist = 73,
        Necromancer = 75,
        SoulTaker = 77,
#endregion

#region BP系
        Tatarabe = 81,
        Blacksmith = 83,
        Machinery = 85,
        Maestro = 87,
        Farmer = 91,
        Alchemist = 93,
        Marionest = 95,
        Harvest = 97,
        Ranger = 101,
        Explorer = 103,
        Treasurehunter = 105,
        Stryder = 107,
        Merchant = 111,
        Trader = 113,
        Gambler = 115,
        RoyalDealer = 117
#endregion
    }

    public enum JointJob
    {
        Breeder = 41,
        Gardener = 51,
    }
}
