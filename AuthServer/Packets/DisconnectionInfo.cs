using CommonLib;
using CommonLib.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer.Packets
{
    public class DisconnectionInfo : ISendPacket
    {
        public int Type { get; set; }
        public string Info1 { get; set; }
        public string Info2 { get; set; }
        public string Info3 { get; set; } //DialogMsg

        public DisconnectionInfo(int _type)
        {
            Type = _type;
        }

        private void FillString(string str, ref byte[] buf, ref int offset)
        {
            var xstr_bytes = str?.ToTBytes();
            if (xstr_bytes != null && xstr_bytes.Length != 0)
            {
                buf.Fill(xstr_bytes, offset);
                offset += xstr_bytes.Length;
            }
            else
            {
                buf[offset++] = 0;
            }
        }

        public BasePacket ToPacket()
        {
            int offset = 0;
            var buf = new byte[2000];
            buf.Fill(Type.ToBytes(), 0);
            offset += 4;
            FillString(Info1, ref buf, ref offset);
            FillString(Info2, ref buf, ref offset);
            FillString(Info3, ref buf, ref offset);
            return new BasePacket(0x0036, buf.Take(offset).ToArray());
        }

    }
}
