using CommonLib;
using CommonLib.Packets;
using System;
using System.Reflection;

namespace LoginServer.Packets
{
    public class SingleCharaEquipmentSelectionInfo
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
    }

    public class SingleCharaEquipmentSelectionInfoExt: SingleCharaEquipmentSelectionInfo
    {
        public uint id { get; set; }

        public SingleCharaEquipmentSelectionInfoExt(uint _id)
        {
            id = _id;
        }

        public SingleCharaEquipmentSelectionInfoExt()
        {
        }
    }

    public class EquipmentSelectionInfo : ISendPacket
    {     
        const byte Size = 4;
        SingleCharaEquipmentSelectionInfo[] infoArray = new SingleCharaEquipmentSelectionInfo[Size];

        public EquipmentSelectionInfo()
        {
            for (byte i = 0; i < Size; i++)
            {
                infoArray[i] = new SingleCharaEquipmentSelectionInfo();
            }
        }

        public void AddEquipmentInfo(SingleCharaEquipmentSelectionInfo equipmentInfo, byte slot)
        {
            infoArray[slot] = equipmentInfo;
        }

        public BasePacket ToPacket()
        {
            var properties = typeof(SingleCharaEquipmentSelectionInfo).GetProperties();
            var classSize = properties.Length * 4 + 1;
            byte[] bytes = new byte[classSize * Size];
            for (byte i = 0; i < Size; i++)
            {
                bytes[i * classSize] = 14;
                int offset = i * classSize + 1;
                foreach (PropertyInfo propertyInfo in properties)
                {
                    bytes.Fill(((UInt32)propertyInfo.GetValue(infoArray[i])).ToBytes(), offset);
                    offset += 4;
                }
            }
            return new BasePacket(0x29, bytes);
        }
    }
}
