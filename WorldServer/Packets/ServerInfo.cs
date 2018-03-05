using CommonLib;
using CommonLib.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Packets
{
    public class ServerInfo : ISendPacket
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public uint Port { get; set; }
        public bool Status { get; set; }

        public BasePacket ToPacket()
        {
            return new BasePacket(0x0033, Name.ToTBytes().Concat($"{Address}:{Port.ToString()}".ToTBytes()).ToArray());
        }
    }
}
