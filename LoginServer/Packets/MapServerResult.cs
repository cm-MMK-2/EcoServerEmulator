using CommonLib;
using CommonLib.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginServer.Packets
{
    public class MapServerResult : ISendPacket
    {
        public byte  Result { get; set; }
        public string ServerAddr { get; set; }
        public uint ServerPort { get; set; }

        public MapServerResult(byte _result, string _serveraddr, uint _port)
        {
            Result = _result;
            ServerAddr = _serveraddr;
            ServerPort = _port;
        }

        public BasePacket ToPacket()
        {
            var buf = new byte[256];
            int offset = 0;
            buf[0] = Result;
            ++offset;
            var xstr_bytes = ServerAddr?.ToTBytes();
            if (xstr_bytes != null && xstr_bytes.Length != 0)
            {
                buf.Fill(xstr_bytes, offset);
                offset += xstr_bytes.Length;
            }
            else
            {
                buf[offset++] = 0;
            }

            buf.Fill(ServerPort.ToBytes(), offset);
            offset += 4;

            return new BasePacket(0x0033, buf.Take(offset).ToArray());
        }
    }
}
