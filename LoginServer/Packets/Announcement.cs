using CommonLib;
using CommonLib.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginServer.Packets
{
    public class Announcement : ISendPacket
    {
        // 0 :システムメッセージ(黄)1 : 全体アナウンス(ピンク)
        public UInt32 ID { get; set; }

        // メッセージ本体 
        public string Message { get; set; }

        /// <summary>
        /// ID: 0 :システムメッセージ(黄)1 : 全体アナウンス(ピンク)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>

        public Announcement(UInt32 id, string message)
        {
            ID = id;
            Message = message;
        }

        public BasePacket ToPacket()
        {
            return new BasePacket(0x00bf, ID.ToBytes().Concat(Message.ToTBytes()).ToArray());
        }
    }
}
