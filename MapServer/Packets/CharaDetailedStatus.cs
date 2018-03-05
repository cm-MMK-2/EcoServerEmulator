using CommonLib;
using CommonLib.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapServer.Packets
{
    public class CharaDetailedStatus1 : ISendPacket
    {
        public ushort[] BaseStatus { get; set; } = new ushort[8];
        public ushort[] ReviseStatus { get; set; } = new ushort[8];
        public ushort[] BounusStatus { get; set; } = new ushort[8];

        public BasePacket ToPacket()
        {
            byte[] data = new byte[53];
            return new BasePacket(0x0212, data);
        }
    }

    public class CharaDetailedStatus2 : ISendPacket
    {
        byte StatusCount { get; set; } = 0x13;
        ushort Speed { get; set; } = 410; //速度 410（旧380

        ushort MinAtkN; //最小ATK1
        ushort MinAtkS; //最小ATK2
        ushort MinAtkT; //最小ATK3
        ushort MaxAtkN; //最大ATK1
        ushort MaxAtkS; //最大ATK2
        ushort MaxAtkT; //最大ATK3

        ushort MinMatk; //最小M.ATK
        ushort MaxMatk; //最大M.ATK

        ushort BaseDef; //基本DEF
        ushort ExtDef;  //追加DEF
        ushort BaseMdef;//基本M.DEF
        ushort ExtMdef; //追加M.DEF

        ushort SHit; //S.HIT(近距離命中率)
        ushort LHit; //L.HIT(遠距離命中率)

        ushort SAvoid; //S.AVOID(近距離回避力)
        ushort LAvoid; //L.AVOID(遠距離回避力)

        ushort ASpd; //A.SPD(攻撃速度)
        ushort CSpd; //C.SPD(詠唱速度)

        public BasePacket ToPacket()
        {
            byte[] data = new byte[39];
            return new BasePacket(0x0217, data);
        }
    }

    public class CharaCapaAndPayl : ISendPacket
    {
        public uint Capa { get; set; } = 0;
        public uint Payl { get; set; } = 0;
        public uint MaxCapa { get; set; } = 2670;

        public uint MaxPayl { get; set; } = 7110;

        public BasePacket ToPacket()
        {
            byte[] data = new byte[16];
            data.Fill(Capa.ToBytes(), 0);
            data.Fill(Payl.ToBytes(), 4);
            data.Fill(MaxCapa.ToBytes(), 8);
            data.Fill(MaxPayl.ToBytes(), 12);
            return new BasePacket(0230, data);
        }
    }

}
