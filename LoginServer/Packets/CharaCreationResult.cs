using CommonLib;
using CommonLib.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginServer.Packets
{
    public enum CharaCreationResultEnum : uint
    {
        GAME_SMSG_CHRCREATE_SUCCESS = 0x00000000, //成功
        GAME_SMSG_CHRCREATE_E_NAME_BADCHAR = 0xffffffa0, //"キャラクター名に使用できない文字が使われています"
        GAME_SMSG_CHRCREATE_E_NAME_TOO_SHORT = 0xffffff9f, //"キャラクター名が短すぎます"
        GAME_SMSG_CHRCREATE_E_NAME_CONFLICT = 0xffffff9e, //"既に同じ名前のキャラクターが存在します"
        GAME_SMSG_CHRCREATE_E_ALREADY_SLOT = 0xffffff9d, //"既にキャラクターが存在します"
        GAME_SMSG_CHRCREATE_E_NAME_TOO_LONG = 0xffffff9c, //"キャラクター名が長すぎます"
        GAME_SMSG_CHRCREATE_E_ERROR = 0xffffff9b, // ソレ以外 "不明なキャラ作成エラー(%d)" insert table failed
    }

    public class CharaCreationResult : ISendPacket
    {
        public UInt32 Result { get; set; }
        public CharaCreationResult(CharaCreationResultEnum result)
        {
            Result = (uint)result;
        }

        public BasePacket ToPacket()
        {
            return new BasePacket(0x00A1, Result.ToBytes());
        }
    }
}
