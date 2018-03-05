using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DWORD = System.UInt32;
using BYTE = System.Byte;
using CommonLib.Packets;
using CommonLib;

namespace CommonLib.Packets
{
    public enum LoginResultEnum : DWORD
    {
        GAME_SMSG_LOGIN_SUCCESS = 0x00000000,//成功
        GAME_SMSG_LOGIN_ERR_UNKNOWN_ACC = 0xfffffffe,//"IDまたはパスワードが違います"
        GAME_SMSG_LOGIN_ERR_BADPASS = 0xfffffffd,//"IDまたはパスワードが違います"
        GAME_SMSG_LOGIN_ERR_BFALOCK = 0xfffffffc,//"このアカウントは認証機能がロックされています"
        GAME_SMSG_LOGIN_ERR_ALREADY = 0xfffffffb,//"既にログインしています$r現在のログイン状態をリセットいたします"
        GAME_SMSG_LOGIN_ERR_IPBLOCK = 0xfffffffa,//"現在メンテナンス中です"
        GAME_SMSG_GHLOGIN_ERR_101 = 0xfffffff5,//"ゲーム料金が未払いか、利用期間切れです。$r簡単登録の方は正式登録をお済ませください。"
        GAME_SMSG_GHLOGIN_ERR_102 = 0xfffffff4,//"認証されていない、または利用停止されたID です。"
        GAME_SMSG_GHLOGIN_ERR_103 = 0xfffffff3,//"認証されていない、または利用停止されたID です。"
        GAME_SMSG_GHLOGIN_ERR_104 = 0xfffffff2,//"認証されていない、または利用停止されたID です。"
        GAME_SMSG_GHLOGIN_ERR_105 = 0xfffffff1,//"ECOは正式サービスとなりました。$rガンホーのアトラクションセンターで、アトラクションIDの紐付け処理をお願いします。"
        GAME_SMSG_GHLOGIN_ERR_106 = 0xfffffff0,//"βサービスは終了しました。正式サービス開始までお待ちください。"
        GAME_SMSG_GHLOGIN_ERR_107 = 0xffffffef,//"お試し期間は終了しました。$r改めてアトラクションセンターでIDの作成をお願いします。"
        GAME_SMSG_GHLOGIN_ERR_108 = 0xffffffee,//"ご入力された「お試しID」はクローズドベータテストの定員数、$r先着20,000名様の登録終了後に発行されたIDです。$r恐れ入りますが、ご入力された「お試しID」はそのまま所持していただき、$r次回実施予定のベータテストをお待ちください。$r（次回実施予定のベータテストはECO公式サイトでご案内いたします。）"
        GAME_SMSG_GHLOGIN_ERR_109 = 0xffffffed,//"認証予備エラー109"
        GAME_SMSG_GHLOGIN_ERR_110 = 0xffffffec,//"認証予備エラー110"
        //それ以外: GAME_SMSG_LOGIN_ERR_ERR,//"不明なエラー(%d)"
    }

    public class LoginAuthResult : ISendPacket
    {
        // 認証結果 (LoginResult Enum)
        public DWORD Result { get; set; }

        // UniqueAccountID: エラー時はffffffff
        public DWORD UserId { get; set; }
        // ゲストID残り時間　08/01/11より
        public DWORD Obsolete1 { get; set; } = 0;
        // ゲストID期限　(1970年1月1日0時0分0秒からの秒数）08/01/11より 
        public DWORD Obsolete2 { get; set; } = 0;
        //ログインサーバー認証時に付加、ワールドサーバーと区別するため？
        public BYTE Zero { get; set; } = 0;

        public LoginAuthResult(LoginResultEnum result, DWORD userId)
        {
            Result = (uint)result;
            UserId = userId;
        }

        public BasePacket ToPacket()
        {
            var buf = new byte[17];
            buf.Fill(Result.ToBytes(), 0);
            buf.Fill(UserId.ToBytes(), 4);
            buf.Fill(Obsolete1.ToBytes(), 8);
            buf.Fill(Obsolete2.ToBytes(), 12);
            buf[16] = Zero;
            return new BasePacket(0x0020, buf);
        }

        //for map server
        public BasePacket ToMSPacket()
        {
            var buf = new byte[10];
            buf.Fill(Result.ToBytes(), 0);
            buf.Fill(string.Empty.ToTBytes(), 4);
            buf.Fill(DateTime.Now.ToUnixTimestamp().ToBytes(), 6);
            return new BasePacket(0x0011, buf);
        }
    }
}
