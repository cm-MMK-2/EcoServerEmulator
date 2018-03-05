using CommonLib;
using CommonLib.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LoginServer.Packets
{
    public class SingleCharaSelectionInfo
    {
        // キャラクター名 (\0は含めない)
        public string Name { get; set; }
        // 種族
        public byte Race { get; set; }
        // 09/12/20確認 DEM FORM状態
        public byte Form { get; set; }
        // 性別
        public byte Sex { get; set; }
        // 髪型
        public UInt16 HairStyle { get; set; }
        // 髪色
        public byte HairColor { get; set; }
        // つけ髪(デフォルトでff)
        public UInt16 Wig { get; set; }
        // 不明(キャラがあればff)
        public byte IsEmptySlot { get; set; }
        // 顔 2011/10/27ABYTE->;AWORD
        public UInt16 Face { get; set; }
        // 転生前のレベル。転生する前は0
        public byte RebirthLv { get; set; }
        // 転生特典 しっぽとかわっかとか
        public byte Ex { get; set; }
        // 転生特典 翼
        public byte Wing { get; set; }
        // 転生特典 翼色
        public byte WingColor { get; set; }
        // 職業
        public byte Job { get; set; }
        // 現在位置(マップ)
        public UInt32 Map { get; set; }
        // レベル
        public byte LvBase { get; set; }
        // 1次職業レベル
        public byte LvJob1 { get; set; }
        // 残りクエスト数
        public UInt16 Quest { get; set; }
        // 2次エキスパート職業レベル
        public byte LvJob2x { get; set; }
        // 2次テクニカル職業レベル
        public byte LvJob2t { get; set; }
        // 3次職業レベル
        public byte LvJob3 { get; set; }
    }

    public class SingleCharaSelectionInfoExt: SingleCharaSelectionInfo
    {
        public uint id { get; set; }
        public byte Slot { get; set; }
    }

    public class CharaSelectionInfo : ISendPacket
    {
        public const byte Size = 4;
        public SingleCharaSelectionInfo[] SciArray { get; set; } = new SingleCharaSelectionInfo[Size];
        
        public CharaSelectionInfo()
        {
            for(byte i = 0; i < Size; i++)
            {
                SciArray[i] = new SingleCharaSelectionInfo();
            }
        }

        public void AddChara(SingleCharaSelectionInfo ssci, byte slot)
        {
            SciArray[slot] = ssci;
        }

        public BasePacket ToPacket()
        {
            int offset = 0;
            var bytes = new byte[2000];

            foreach (PropertyInfo propertyInfo in typeof(SingleCharaSelectionInfo).GetProperties())
            {
                bytes[offset] = Size;
                offset += 1;
                if (propertyInfo.PropertyType == typeof(byte))
                {
                    for (byte i = 0; i < Size; i++)
                    {
                        bytes[offset++] = (byte)propertyInfo.GetValue(SciArray[i]);
                    }
                }
                else if(propertyInfo.PropertyType == typeof(UInt16))
                {
                    for (byte i = 0; i < Size; i++)
                    {
                        bytes.Fill(((UInt16)propertyInfo.GetValue(SciArray[i])).ToBytes(), offset);
                        offset += 2;
                    }
                }
                else if(propertyInfo.PropertyType == typeof(UInt32))
                {
                    for (byte i = 0; i < Size; i++)
                    {
                        bytes.Fill(((UInt32)propertyInfo.GetValue(SciArray[i])).ToBytes(), offset);
                        offset += 4;
                    }
                }
                else if (propertyInfo.PropertyType == typeof(string))
                {
                    for (byte i = 0; i < Size; i++)
                    {
                        var xstr_bytes = (propertyInfo.GetValue(SciArray[i]) as string)?.ToXBytes();
                        if (xstr_bytes != null)
                        {
                            bytes.Fill(xstr_bytes, offset);
                            offset += xstr_bytes.Length;
                        }
                        else
                        {
                            bytes[offset++] = 0;
                        }
                    }
                }
                else
                {
                    throw new Exception("Get wrong type in SelectionSingleCharaInfo class, type can only be <byte, UInt16, UInt32, string>");
                }
            }

            if(offset > 2000)
            {
                throw new Exception("Data bytes size exceeds max size 2000");
            }

            return new BasePacket(0x0028, bytes.Take(offset).ToArray());
        }
    }
}
