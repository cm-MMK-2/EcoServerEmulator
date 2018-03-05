using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapServer.Packets
{
    public class CharaEquipment
    {
        // 頭
        public UInt32 Head { get; set; }
        // 頭アクセサリ
        public UInt32 Headacce { get; set; }
        // 顔
        public UInt32 Face { get; set; }
        // 顔アクセサリ
        public UInt32 Faceacce { get; set; }
        // 胸アクセサリ
        public UInt32 Chestacce { get; set; }
        // 上半身
        public UInt32 Top { get; set; }
        // 下半身
        public UInt32 Bottom { get; set; }
        // 背中
        public UInt32 Backpack { get; set; }
        // 右手装備
        public UInt32 Right { get; set; }
        // 左手装備
        public UInt32 Left { get; set; }
        // 靴
        public UInt32 Shoes { get; set; }
        // 靴下
        public UInt32 Socks { get; set; }
        // ペット
        public UInt32 Pet { get; set; }
        // エフェクト
        public UInt32 Effect { get; set; }

        public uint[] ToArray()
        {
            uint[] data = new uint[14];
            data[0] = Head;
            data[1] = Headacce;
            data[2] = Face;
            data[3] = Faceacce;
            data[4] = Chestacce;
            data[5] = Top;
            data[6] = Bottom;
            data[7] = Backpack;
            data[8] = Right;
            data[9] = Left;
            data[10] = Shoes;
            data[11] = Socks;
            data[12] = Pet;
            data[13] = Effect;
            return data;
        }
        
    }
}
