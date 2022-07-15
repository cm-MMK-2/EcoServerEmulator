using CommonLib;
using CommonLib.Data;
using System;

namespace LoginServer.Data
{
    //http://eco.acronia.net/wiki/?Tribe
    public class BaseStatus
    {
        public ushort Str { get; set; }
        public ushort Dex { get; set; }
        public ushort Int { get; set; }
        public ushort Vit { get; set; }
        public ushort Agi { get; set; }
        public ushort Mag { get; set; }

        public static BaseStatus GetInitialStatus(Race tribe)
        {
            var status = new BaseStatus();
            switch (tribe)
            {
                case Race.Emil:
                    {
                        status.Str = 8;
                        status.Dex = 3;
                        status.Int = 3;
                        status.Vit = 10;
                        status.Agi = 4;
                        status.Mag = 3;
                    }
                    break;
                case Race.Titania:
                    {
                        status.Str = 6;
                        status.Dex = 3;
                        status.Int = 6;
                        status.Vit = 4;
                        status.Agi = 2;
                        status.Mag = 10;
                    }
                    break;
                case Race.Dominion:
                    {
                        status.Str = 10;
                        status.Dex = 5;
                        status.Int = 1;
                        status.Vit = 1;
                        status.Agi = 5;
                        status.Mag = 8;
                    }
                    break;
                case Race.DEM:
                    {
                        status.Str = 5;
                        status.Dex = 5;
                        status.Int = 5;
                        status.Vit = 5;
                        status.Agi = 5;
                        status.Mag = 5;
                    }
                    break;
            }
            return status;
        }
    }

    public class StatusExt : BaseStatus
    {
        public uint Lv { get; set; } = 1;
        public ushort Job { get; set; } = (ushort)CommonLib.Data.Job.Novice;
        //最大HP = floor[ (VIT×3 + floor[VIT/5]^2 + LV×2 + floor[LV/5]^2 + 50)×HP係数 ] + スキルによる補正 + 装備による補正
        public uint MaxHp { get
            {
                return (uint)Math.Floor((Vit * 3 + (Vit / 5).Pow(2) + Lv * 2 + (Lv / 5).Pow(2) + 50) * 1.0f/*todo: HP係数, 補正*/);
            }
        }
        public uint MaxMp { get; } = 100;
        public uint MaxSp { get; } = 100;
        public uint MaxEp { get; } = 30; // todo: read 

    }

    public class RuntimeStatus
    {

    }

}
