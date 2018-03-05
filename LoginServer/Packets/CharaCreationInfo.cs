using CommonLib;

namespace LoginServer.Packets
{
    //for 00a0 
    public class CharaCreationInfo
    {
        public byte chara_slot { get; set; } // キャラクタの番号(00,01,02,03) 
        public string chara_name { get; set; } // キャラクタ名
        public byte race { get; set; } // 種族 00: エミル 01: タイタニア 02: ドミニオン 03: DEM
        public byte sex { get; set; } // 性別 00: 男性 01: 女性
        public ushort hair { get; set; }// 髪型
        public byte hair_color { get; set; } // 髪色（ウィンター等の色にしても作成可能
        public ushort face { get; set; } // 顔


        public CharaCreationInfo(byte[] data)
        {
            int offset = 0;
            byte temp_length;
            chara_slot = data[offset++];
            chara_name = data.ToTSTR(out temp_length, offset);
            offset += (temp_length + 1);
            race = data[offset++];
            sex = data[offset++];
            hair = data.ToUInt16(offset);
            offset += 2;
            hair_color = data[offset++];
            face = data.ToUInt16(offset);
        }
    }
}
