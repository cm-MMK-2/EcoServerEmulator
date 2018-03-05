using CommonLib;
using CommonLib.Data;
using CommonLib.Packets;
using CommonLib.Socket;
using MapServer.Packets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace MapServer
{
    public class MapServerProtocol
    {
        RNGCryptoServiceProvider rng;
        private readonly byte[] firstAck;

        public MapServerProtocol()
        {
            firstAck = new BasePacket(0xffff, new byte[] { 0xe8, 0x6a, 0x6a, 0xca, 0xdc, 0xe8, 0x06, 0x05, 0x2b, 0x29, 0xf8, 0x96, 0x2f, 0x86, 0x7c, 0xab, 0x2a, 0x57, 0xad, 0x30 }).ToBytes();
            rng = new RNGCryptoServiceProvider();
        }

        ~MapServerProtocol()
        {
            rng.Dispose();
        }

        /// <summary>
        /// Protocol 000A
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        /// <returns></returns>
        public void ConnectionInit(EcoSession session, BasePacket packet)
        {
            session.Send(firstAck);
            session.Send(new BasePacket(0x000b, packet.Data));
            #region send password hash
            byte[] front_word = new byte[4];
            byte[] back_word = new byte[4];
            rng.GetBytes(front_word);
            rng.GetBytes(back_word);
            front_word[0] = 0;
            back_word[0] = 0;
            session.FrontWord = front_word.ToUInt32();
            session.BackWord = back_word.ToUInt32();
            var pass_hash = new BasePacket(0x000f, front_word.Concat(back_word).ToArray()).ToBytes();
            session.Send(pass_hash);
            #endregion
        }


        /// <summary>
        /// Protocol 0032
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        /// <returns></returns>
        public void ConnectionCheck(EcoSession session, BasePacket packet)
        {
            session.Send(new BasePacket(0x0033));
        }


        /// <summary>
        /// Protocol 0010
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        /// <returns></returns>
        public async void UserLogin(EcoSession session, BasePacket packet)
        {
            var login_data = new LoginData(packet.Data);
            Logger.Debug($"Received Login Request, Username: {login_data.Username}, Password:{login_data.Password}, MacAddress:{login_data.MacAddress.ToHexString()}, SingleSignOn:{login_data.SingleSignOn.ToString()}");
            LoginAuthResult loginAuthResult;
            //select user info from db
            Account userAccount = await DatabaseManager.SelectAsync<Account>("username", $@"'{login_data.Username}'");

            if (userAccount.is_online)
            {
                loginAuthResult = new LoginAuthResult(LoginResultEnum.GAME_SMSG_LOGIN_ERR_ALREADY, 1);
                session.Send(loginAuthResult.ToPacket());
                Logger.Debug($"USER: {login_data.Username} has already logged in.");
                return;
            }

            if (userAccount.is_banned)
            {
                loginAuthResult = new LoginAuthResult(LoginResultEnum.GAME_SMSG_LOGIN_ERR_BFALOCK, 1);
                session.Send(loginAuthResult.ToPacket());
                Logger.Debug($"USER: {login_data.Username} is banned.");
                return;
            }

            string pass_hash = Utilities.PasswordHash(userAccount.password, session.FrontWord.ToString(), session.BackWord.ToString()).ToHexString();


            if (login_data.Password == pass_hash)
            {
                loginAuthResult = new LoginAuthResult(LoginResultEnum.GAME_SMSG_LOGIN_SUCCESS, 1);
                session.UserLoginSuccess = true;
                session.AccountID = userAccount.id;
                Logger.Debug($"USER: {login_data.Username} login success.");
            }
            else
            {
                loginAuthResult = new LoginAuthResult(LoginResultEnum.GAME_SMSG_LOGIN_ERR_BADPASS, 0);
                Logger.Debug($"USER: {login_data.Username} login disallowed (wrong password).");
            }


            session.Send(loginAuthResult.ToMSPacket());

        }

        /// <summary>
        /// Protocol 01FD
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        /// <returns></returns>
        public async void SelectMap(EcoSession session, BasePacket packet)
        {
            Logger.Debug($"Received SelectChara Request :{packet.Data.ToHexString()}");
            SelectMapRequestInfo scri = new SelectMapRequestInfo(packet.Data);
            if (scri.MapMove == 0) //login
            {

                //session.CharaID = await DatabaseManager.SelectAsync($@"SELECT id FROM Chara WHERE account_id = {session.AccountID} AND Slot = {scri.SelectedSlot}");
                CharaInfo info = await DatabaseManager.SelectAsync<CharaInfo>(
                    $@"SELECT Chara.Id AS CharaID, Name, Race, Form, Race, Form, Sex, HairStyle, HairColor, Wig, IsEmptySlot, Face, RebirthLv, Ex, Wing, WingColor, Map, X, Y, Dir, Hp, Mp, Sp,Ep, MaxHp, MaxMp, MaxSp, MaxEp, Gold FROM Chara LEFT JOIN CharaData ON Chara.id = CharaData.id WHERE Chara.account_id = {session.AccountID} AND Slot = {scri.SelectedSlot}"
                );

                CharaEquipment equip = await DatabaseManager.SelectAsync<CharaEquipment>(info.CharaID, "Equip");

                info.Equipments = equip.ToArray();

                session.CharaID = info.CharaID;
                ShowTinyIcon(session, info.ShowTinyIcon == 1);
                EverydayDungeonNotification(session, info.DailyDungeonCleared == 1);
                MoveSpeedChange(session, session.CharaID, 0x0A); //lock chara move
                CharaModeChange(session, session.CharaID, 2, 0);
                RightClickSettings(session, RightClickSettingsEnum.ALLOW_ALL); //todo: save user settings

                // todo:
                SendCharaInventory(session);
                session.Send(info.ToPacket());
                UpdateCharaMaxHpMpSpEp(session, info);
                UpdateCharaHpMpSpEp(session, info);
                UpdateCharaBuffAndDebuff(session, session.CharaID);
                UpdateDetailedStatus(session, info);
                UpdateElements(session);
                UpdateCharaCapaAndPayl(session);
                //=====
            }
            else //move between map servers
            {

            }
            //session.Send(new BasePacket(0x0033));
        }

        /// <summary>
        /// Protocol 13c7 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        public void FinishMapMove(EcoSession session, BasePacket packet)
        {
            //0226 send skill data
            //022d HEARTスキル
            //122a MonsterID通知
            //0235 自キャラのEXP,JobEXPバー,WRP
            //0244 ステータスウィンドウの職業
            //023a Lv JobLv ボーナスポイント スキルポイント
            //22d4 不明 another?
            //09ed ecoin通知
            //196e クエスト回数・時間
            //1bbc スタンプ帳詳細
            //025d ステータス限界突破
            //0695 ステータスボーナス
            //2260 ワールドゲージボーナス
            //2288 旅人のメモ
            //23a0 不明 another?
            //2549 称号リスト通知
            //240e ビンゴ関連？
            //19dc パーティ参加or作成通知？
            //19d9 パーティー名
            //19e1 パーティメンバーの変更通知
            //19e6 リングメンバーのオンライン情報
        }

        /// <summary>
        /// Protocol 11f8
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        public void CharaMove(EcoSession session, BasePacket packet)
        {

        }

        /// <summary>
        /// Protocol 0FA5 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        public void AttackModeChange(EcoSession session, BasePacket packet)
        {

        }


        /// <summary>
        /// Protocol 11FE 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        public async void RequestMove(EcoSession session, BasePacket packet)
        {
            ushort mov = await DatabaseManager.SelectAsyncUshort(
                $@"SELECT Mov FROM CharaData WHERE id = {session.CharaID}" 
            );
            MoveSpeedChange(session, session.CharaID, mov);
        }


        /// <summary>
        /// Protocol 121B
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        public void MotionChange(EcoSession session, BasePacket packet)
        {

        }

        //=====================================================

        /// <summary>
        /// 取得済みEXモーション1
        /// </summary>
        /// <param name="session"></param>
        private void GetExMotion1(EcoSession session)
        {
            //todo: get motion from db
            uint[] motion1 = new uint[16];

            byte[] data = new byte[65];

            data[0] = (byte)motion1.Length;

            for (byte i = 0; i < data[0]; i++)
            {
                data.Fill(motion1[i].ToBytes(), 1 + 4* i);
            }

            session.Send(new BasePacket(0x1ce8, data));
        }

        /// <summary>
        /// 取得済みEXモーション2
        /// </summary>
        /// <param name="session"></param>
        private void GetExMotion2(EcoSession session)
        {
            //todo: get motion from db
            uint[] motion2 = new uint[3];

            byte[] data = new byte[13];

            data[0] = (byte)motion2.Length;

            for (byte i = 0; i < data[0]; i++)
            {
                data.Fill(motion2[i].ToBytes(), 1 + 4 * i);
            }

            session.Send(new BasePacket(0x1d06, data));
        }

        private void UpdateDetailedStatus(EcoSession session, CharaInfo info)
        {
            session.Send(new CharaDetailedStatus1().ToPacket());
            session.Send(new CharaDetailedStatus2().ToPacket());
        }


        private void UpdateElements(EcoSession session)
        {
            CharaElements elements = new CharaElements();
            session.Send(elements.ToPacket());
        }

        private void UpdateCharaCapaAndPayl(EcoSession session)
        {
            CharaCapaAndPayl capaAndPayl = new CharaCapaAndPayl();
            session.Send(capaAndPayl.ToPacket());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="info"></param>
        private void UpdateCharaMaxHpMpSpEp(EcoSession session, CharaInfo info)
        {
            byte[] data = new byte[33];
            data.Fill(info.CharaID.ToBytes(), 0);
            data[4] = 3;
            data.Fill(((ulong)info.MaxHp).ToBytes(), 5);
            data.Fill(((ulong)info.MaxMp).ToBytes(), 13);
            data.Fill(((ulong)info.MaxSp).ToBytes(), 21);
            data.Fill(info.MaxEp.ToBytes(), 29);
            session.Send(new BasePacket(0x0221, data));
        }

        private void UpdateCharaHpMpSpEp(EcoSession session, CharaInfo info)
        {
            byte[] data = new byte[33];
            data.Fill(info.CharaID.ToBytes(), 0);
            data[4] = 3;
            data.Fill(((ulong)info.Hp).ToBytes(), 5);
            data.Fill(((ulong)info.Mp).ToBytes(), 13);
            data.Fill(((ulong)info.Sp).ToBytes(), 21);
            data.Fill(info.MaxEp.ToBytes(), 29);
            session.Send(new BasePacket(0x021c, data));
        }

        private void UpdateCharaBuffAndDebuff(EcoSession session, uint chara_id)
        {
            byte[] data = new byte[52];
            data.Fill(chara_id.ToBytes(), 0);
            //todo: record buffs and debuffs
            session.Send(new BasePacket(0x157c, data));
        }

        /// <summary>
        /// send inventory data
        /// </summary>
        /// <param name="session"></param>
        /// <param name="chara_id"></param>
        private void SendCharaInventory(EcoSession session)
        {
            //todo
            session.Send(new ItemInfoExt(2).ToPacket());
        }

        ///// <summary>
        ///// send chara data
        ///// </summary>
        ///// <param name="session"></param>
        ///// <param name="chara_id"></param>
        //private void SendCharaInfo(EcoSession session, CharaInfo info)
        //{
        //    session.Send(info.ToPacket());
        //}

        /// <summary>
        /// タイニーアイコン
        /// </summary>
        /// <param name="session"></param>
        /// <param name="show"></param>
        private void ShowTinyIcon(EcoSession session, bool show)
        {
            session.Send(new BasePacket(0x1f72, new byte[] { (byte)(show ? 0x01 :0x00) }));
        }

        /// <summary>
        /// DWORD unknown;
        /// DWORD flag?; //毎日ダンジョンクリア時に0x00000001を受信(！を非表示にする)
        /// </summary>
        /// <param name="session"></param>
        /// <param name="cleared"></param>
        private void EverydayDungeonNotification(EcoSession session, bool cleared)
        {
            UInt64 data = (UInt64)(cleared ? 0x01 : 0x00);
            session.Send(new BasePacket(0x026a, data.ToBytes()));
        }

        /// <summary>
        /// change chara's move speed
        /// </summary>
        /// <param name="session"></param>
        /// <param name="chara_id"></param>
        /// <param name="speed"></param>
        private void MoveSpeedChange(EcoSession session,uint chara_id, ushort speed)
        {
            session.Send(new BasePacket(0x1239, chara_id.ToBytes().Concat(speed.ToBytes()).ToArray()));
        }

        /// <summary>
        /// 通常
        /// ** ** ** **
        /// 00 00 00 02
        /// 00 00 00 00
        /// 闘技場モード
        /// ********
        /// 00 00 00 42
        /// 00 00 00 01
        /// ドミニオン界モード
        /// ********
        /// 00 00 01 02
        /// 00 00 00 04
        /// </summary>
        /// <param name="session"></param>
        /// <param name="chara_id"></param>
        /// <param name="mode1"></param>
        /// <param name="mode2"></param>
        private void CharaModeChange(EcoSession session, uint chara_id, uint mode1, uint mode2)
        {
            byte[] data = new byte[12];
            data.Fill(chara_id.ToBytes(), 0);
            data.Fill(mode1.ToBytes(), 0);
            data.Fill(mode2.ToBytes(), 0);
            session.Send(new BasePacket(0x0fa7, data));
        }


        /// <summary>
        /// Set Right Click Menu functions
        /// </summary>
        /// <param name="session"></param>
        /// <param name="options"></param>
        private void RightClickSettings(EcoSession session, RightClickSettingsEnum options)
        {            
            session.Send(new BasePacket(0x1a5f, ((uint)options).ToBytes()));
        }
    }
}
