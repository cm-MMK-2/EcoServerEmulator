using CommonLib;
using CommonLib.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapServer.Packets
{
    public class CharaElements : ISendPacket
    {
        public ushort[] AtkElement { get; set; } = new ushort[7];

        public ushort[] DefElement { get; set; } = new ushort[7];

        public BasePacket ToPacket()
        {
            byte[] data = new byte[30];
            data[0] = 7;
            data[15] = 7;
            for (int i = 0; i < 7; i++)
            {
                data.Fill(AtkElement[i].ToBytes(), 1 + i * 2);
                data.Fill(DefElement[i].ToBytes(), 16 + i * 2);
            }
            return new BasePacket(0x0223, data);
        }
    }
}
