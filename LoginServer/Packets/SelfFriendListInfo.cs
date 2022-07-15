using CommonLib.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginServer.Packets
{
    public class SelfFriendListInfo : ISendPacket
    {
        public byte State { get; set; }    // 現在の状態(オンライン・退席中等)
        public string Comment { get; set; }  // コメント
        public BasePacket ToPacket()
        {
            return BasePacket.MakeUniversalPacket(this, 0x00dd);
        }
    }
}
