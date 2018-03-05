using CommonLib;
using CommonLib.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Packets
{
    public class LoginData
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public byte[] MacAddress { get; set; }
        public UInt32 SingleSignOn { get; set; }

        public LoginData(byte[] data)
        {
            int offset = 0;
            byte temp_length;
            Username = data.ToTSTR(out temp_length, offset);
            offset += (temp_length + 1);
            Password = data.ToTSTR(out temp_length, offset);
            offset += (temp_length + 1);
            temp_length = data[offset];
            MacAddress = data.Skip(offset + 1).Take(temp_length).ToArray();
            offset += (temp_length + 1);
            SingleSignOn = data.ToUInt32(offset);
        }

        //public LoginPacket(BasePacket packet)
        //{
        //    var data = packet.Data;
        //    int offset = 4;
        //    byte temp_length;
        //    Username = data.ToTSTR(out temp_length, offset);
        //    offset += (temp_length + 1);
        //    Password = data.ToTSTR(out temp_length, offset);
        //    offset += (temp_length + 1);
        //    MacAddress = data.ToTSTR(out temp_length, offset);
        //    offset += (temp_length + 1);
        //    SingleSignOn = data.ToUInt32(offset);
        //}
    }
}
