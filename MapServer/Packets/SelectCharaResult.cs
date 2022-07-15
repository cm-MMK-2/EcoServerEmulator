using CommonLib.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapServer.Packets
{
    public class ChangeMapFinish : ISendPacket
    {
        public uint CharaId { get; set; }
        public uint Reserved { get; set; }

        public ChangeMapFinish(uint charaId)
        {
            CharaId = charaId;
            Reserved = 0;
        }

        public BasePacket ToPacket()
        {
            return BasePacket.MakeUniversalPacket(this, 0x1b67);
        }
    }
}
