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
            return BasePacket.MakeUniversalPacket(this, 0x0212);
        }
    }

    public class CharaDetailedStatus2 : ISendPacket
    {
        public byte StatusCount { get; set; } = 0x13;
        public ushort Speed { get; set; } = 410; //速度 410（旧380

        public ushort MinAtkN { get; set; } //最小ATK1
        public ushort MinAtkS { get; set; } //最小ATK2
        public ushort MinAtkT { get; set; } //最小ATK3
        public ushort MaxAtkN { get; set; } //最大ATK1
        public ushort MaxAtkS { get; set; } //最大ATK2
        public ushort MaxAtkT { get; set; } //最大ATK3

        public ushort MinMatk { get; set; } //最小M.ATK
        public ushort MaxMatk { get; set; } //最大M.ATK

        public ushort BaseDef { get; set; } //基本DEF
        public ushort ExtDef { get; set; } //追加DEF
        public ushort BaseMdef { get; set; }//基本M.DEF
        public ushort ExtMdef { get; set; } //追加M.DEF

        public ushort SHit { get; set; }//S.HIT(近距離命中率)
        public ushort LHit { get; set; }//L.HIT(遠距離命中率)

        public ushort SAvoid { get; set; }//S.AVOID(近距離回避力)
        public ushort LAvoid { get; set; } //L.AVOID(遠距離回避力)

        public ushort ASpd { get; set; } //A.SPD(攻撃速度)
        public ushort CSpd { get; set; }//C.SPD(詠唱速度)

        public BasePacket ToPacket()
        {
            return BasePacket.MakeUniversalPacket(this, 0x0217);
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
            return new BasePacket(0x0230, data);
        }
    }

}
