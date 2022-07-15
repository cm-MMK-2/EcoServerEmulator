using CommonLib;
using CommonLib.Data;
using CommonLib.Packets;
using CommonLib.Socket;
using LoginServer.Data;
using LoginServer.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace LoginServer
{
    public class LoginServerProtocol
    {
        private readonly byte[] firstAck;
        RNGCryptoServiceProvider rng;
        public LoginServerProtocol()
        {
            firstAck = new BasePacket(0xffff, new byte[] { 0xe8, 0x6a, 0x6a, 0xca, 0xdc, 0xe8, 0x06, 0x05, 0x2b, 0x29, 0xf8, 0x96, 0x2f, 0x86, 0x7c, 0xab, 0x2a, 0x57, 0xad, 0x30 }).ToBytes();
            rng = new RNGCryptoServiceProvider();
        }

        ~LoginServerProtocol()
        {
            rng.Dispose();
        }

        /// <summary>
        /// Protocol 0001
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        /// <returns></returns>
        public void VersionCheck(EcoSession session, BasePacket packet)
        {
            try
            {
                //Logger.Debug($"Send Packet:{loginServerAck.ToHexString()}");
                session.Send(firstAck);
                byte[] front_word = new byte[4];
                byte[] back_word = new byte[4];
                rng.GetBytes(front_word);
                rng.GetBytes(back_word);
                front_word[0] = 0;
                back_word[0] = 0;
                session.FrontWord = front_word.ToUInt32();
                session.BackWord = back_word.ToUInt32();
                var pass_hash = new BasePacket(0x001e, front_word.Concat(back_word).ToArray()).ToBytes();
                session.Send(pass_hash);
                session.Send(new BasePacket(0x0002, packet.Data));
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        /// <summary>
        /// Protocol 000A
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        /// <returns></returns>
        public void ConnectionCheck(EcoSession session, BasePacket packet)
        {
            session.Send(new BasePacket(0x000b, packet.Data));
        }


        /// <summary>
        /// Protocol 001F
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        /// <returns></returns>
        public async void UserLogin(EcoSession session, BasePacket packet)
        {
            try
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
                    return;
                }

                if (userAccount.is_banned)
                {
                    loginAuthResult = new LoginAuthResult(LoginResultEnum.GAME_SMSG_LOGIN_ERR_BFALOCK, 1);
                    session.Send(loginAuthResult.ToPacket());
                    return;
                }

                string pass_hash = Utilities.PasswordHash(userAccount.password, session.FrontWord.ToString(), session.BackWord.ToString()).ToHexString();

                if (login_data.Password == pass_hash)
                {
                    loginAuthResult = new LoginAuthResult(LoginResultEnum.GAME_SMSG_LOGIN_SUCCESS, 1);
                    session.UserLoginSuccess = true;
                    session.AccountID = userAccount.id;
                }
                else
                {
                    loginAuthResult = new LoginAuthResult(LoginResultEnum.GAME_SMSG_LOGIN_ERR_BADPASS, 0);
                }

                session.Send(loginAuthResult.ToPacket());

                if (session.UserLoginSuccess)
                {

                    CharaSelectionInfo sci = new CharaSelectionInfo();
                    EquipmentSelectionInfo sei = new EquipmentSelectionInfo();
                    //select chara data from db
                    List<SingleCharaSelectionInfoExt> lscsiws = await DatabaseManager.SelectMultiAsync<SingleCharaSelectionInfoExt>("account_id", session.AccountID.ToString(), "Chara");


                    foreach (var scsiws in lscsiws)
                    {
                        sci.AddChara(scsiws, scsiws.Slot);
                        SingleCharaEquipmentSelectionInfoExt scesi = await DatabaseManager.SelectAsync<SingleCharaEquipmentSelectionInfoExt>(scsiws.id, "Equip");
                        sei.AddEquipmentInfo(scesi, scsiws.Slot);
                    }

                    //test use
                    //sci.AddChara(new SingleCharaSelectionInfo {
                    //    Name = "TEST",
                    //    RebirthLv = 10, Sex = 1, HairStyle = 7, HairColor = 50, Wig = 0xffff, IsEmptySlot = 0xff, Face = 0, Map = 0x0098f058, Job = 91, LvBase = 15, LvJob1 = 15, LvJob2t = 50, LvJob2x = 1, LvJob3 = 1, Quest = 5 });

                    session.Send(sci.ToPacket());

                    //for test use
                    //sei.AddEquipmentInfo(new SingleCharaEquipmentSelectionInfo { Chestacce = 0x02fc6a3c, Tops = 0x0395d924, Backpack = 0x02fc3328, Shoes = 0x02fe8678 });

                    session.Send(sei.ToPacket());
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }


        /// <summary>
        /// Protocol 0034
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        /// <returns></returns>
        public void RequestCharaID(EcoSession session, BasePacket packet)
        {
            Logger.Debug($"Received CharaID Request :{packet.Data.ToHexString()}");
        }


        /// <summary>
        /// Protocol 00A0
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        /// <returns></returns>
        public async void CreateChara(EcoSession session, BasePacket packet)
        {
            CharaCreationInfo cci = new CharaCreationInfo(packet.Data);
            Logger.Debug($"Received Chara Creation Request at slot:{cci.chara_slot}, chara name:{cci.chara_name}, race:{cci.race},sex:{cci.sex}, hair:{cci.hair}, hair color:{cci.hair_color}, face:{cci.face}");

            #region validation check

            if (session.AccountID == 0)
            {
                //error
                Logger.Error($"Create chara failed, AccountID is 0");
                return;
            }

            if (cci.chara_name.Length < 3)
            {
                session.Send(new CharaCreationResult(CharaCreationResultEnum.GAME_SMSG_CHRCREATE_E_NAME_TOO_SHORT).ToPacket());
                return;
            }

            if (cci.chara_name.Length > 8)
            {
                session.Send(new CharaCreationResult(CharaCreationResultEnum.GAME_SMSG_CHRCREATE_E_NAME_TOO_LONG).ToPacket());
                return;
            }

            Regex r = new Regex(@"[.,\/#!$%\^&\*;:{}=\-_~()\s]");
            if (r.IsMatch(cci.chara_name))
            {
                session.Send(new CharaCreationResult(CharaCreationResultEnum.GAME_SMSG_CHRCREATE_E_NAME_BADCHAR).ToPacket());
                return;
            }

            uint repeat_slot_count = await DatabaseManager.SelectCountAsync($@"SELECT COUNT(id) AS COUNT FROM Chara WHERE 'account_id' = {session.AccountID} AND 'Slot' = {cci.chara_slot}");
            if (repeat_slot_count > 0)
            {
                session.Send(new CharaCreationResult(CharaCreationResultEnum.GAME_SMSG_CHRCREATE_E_ALREADY_SLOT).ToPacket());
                return;
            }

            uint repeat_name_count = await DatabaseManager.SelectCountAsync("Name", $@"'{cci.chara_name}'", "Chara", "id");
            if (repeat_name_count > 0)
            {
                session.Send(new CharaCreationResult(CharaCreationResultEnum.GAME_SMSG_CHRCREATE_E_NAME_CONFLICT).ToPacket());
                return;
            }
            #endregion

            #region insert chara info
            Dictionary<string, object> charaInsertDict = new Dictionary<string, object>();
            charaInsertDict.Add("account_id", session.AccountID);
            charaInsertDict.Add("Slot", cci.chara_slot);
            charaInsertDict.Add("Name", cci.chara_name);
            charaInsertDict.Add("Race", cci.race);
            charaInsertDict.Add("Sex", cci.sex);
            charaInsertDict.Add("HairStyle", cci.hair);
            charaInsertDict.Add("HairColor", cci.hair_color);
            charaInsertDict.Add("Face", cci.face);
            charaInsertDict.Add("Map", 20013003); //start map

            if (await DatabaseManager.InsertAsync("Chara", charaInsertDict) <= 0)
            {
                session.Send(new CharaCreationResult(CharaCreationResultEnum.GAME_SMSG_CHRCREATE_E_ERROR).ToPacket());
                return;
            }

            #region  insert default equipments for the chara

            uint chara_id = await DatabaseManager.SelectAsync($@"SELECT id FROM Chara WHERE account_id = {session.AccountID} AND Slot = {cci.chara_slot}");

            if (chara_id == 0)
            {
                Logger.Error("Get chara id error.");
            }

            SingleCharaEquipmentSelectionInfoExt scesi = new SingleCharaEquipmentSelectionInfoExt(chara_id);
            Dictionary<string, object> equipInsertDict = new Dictionary<string, object>();

            equipInsertDict.Add("id", chara_id);
            // Head
            equipInsertDict.Add("Head", scesi.Head);
            // Headacce
            equipInsertDict.Add("Headacce", scesi.Headacce);
            // Face
            equipInsertDict.Add("Face", scesi.Face);
            //Faceacce
            equipInsertDict.Add("Faceacce", scesi.Faceacce);
            //Chestacce
            equipInsertDict.Add("Chestacce", scesi.Chestacce);
            //Tops
            equipInsertDict.Add("Top", scesi.Top);
            //Bottoms
            equipInsertDict.Add("Bottom", scesi.Bottom);
            //Backpack
            equipInsertDict.Add("Backpack", scesi.Backpack);
            //Right
            equipInsertDict.Add("Right", scesi.Right);
            //Left
            equipInsertDict.Add("Left", scesi.Left);
            //Shoes
            equipInsertDict.Add("Shoes", scesi.Shoes);
            //Socks
            equipInsertDict.Add("Socks", scesi.Socks);
            //Pet
            equipInsertDict.Add("Pet", scesi.Pet);
            //Effect
            equipInsertDict.Add("Effect", scesi.Effect);

            if (await DatabaseManager.InsertAsync("Equip", equipInsertDict) <= 0)
            {
                Logger.Error("Insert Equip failed.");
            }

            #endregion

            #region Insert Default CharaData
            Dictionary<string, object> charaDataInsertDict = new Dictionary<string, object>();
            StatusExt initialStatus = BaseStatus.GetInitialStatus((Race)cci.race) as StatusExt;
            charaDataInsertDict.Add("id", chara_id);
            charaDataInsertDict.Add("HP", initialStatus.MaxHp);
            charaDataInsertDict.Add("Mp", initialStatus.MaxMp);
            charaDataInsertDict.Add("Sp", initialStatus.MaxSp);
            charaDataInsertDict.Add("Ep", initialStatus.MaxEp);
            charaDataInsertDict.Add("Str", initialStatus.Str);
            charaDataInsertDict.Add("Dex", initialStatus.Dex);
            charaDataInsertDict.Add("Int", initialStatus.Int);
            charaDataInsertDict.Add("Vit", initialStatus.Vit);
            charaDataInsertDict.Add("Agi", initialStatus.Agi);
            charaDataInsertDict.Add("Mag", initialStatus.Mag);
            charaDataInsertDict.Add("Luk", 0);
            charaDataInsertDict.Add("Cha", 0);
            charaDataInsertDict.Add("Mov", 10);
            charaDataInsertDict.Add("Gold", 0);
            charaDataInsertDict.Add("HyouiTarget", 0);
            charaDataInsertDict.Add("HyouiPart", 0);
            //initial status points
            if (await DatabaseManager.InsertAsync("CharaData", charaDataInsertDict) <= 0)
            {
                //error
                Logger.Error("Insert CharaData failed.");
            }
            #endregion
            #endregion

            //send success message
            session.Send(new CharaCreationResult(CharaCreationResultEnum.GAME_SMSG_CHRCREATE_SUCCESS).ToPacket());
        }


        /// <summary>
        /// Protocol 00A0
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        /// <returns></returns>
        public async void RequestMapID(EcoSession session, BasePacket packet)
        {
            Logger.Debug($"Received MapID Request :{packet.Data.ToHexString()}");
            byte slot = packet.Data[0];
            uint map_id = await DatabaseManager.SelectAsync($@"SELECT Map FROM Chara WHERE account_id = {session.AccountID} AND Slot = {slot}");
            session.Send(new BasePacket(0xA8, map_id.ToBytes()));
        }

        /// <summary>
        /// Protocol 0032
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        /// <returns></returns>
        public async void RequestMapServer(EcoSession session, BasePacket packet)
        {
            Logger.Debug($"Received MapServer Request :{packet.Data.ToHexString()}");
            uint map_id = packet.Data.ToUInt32();
            //todo: setup map server for different maps
            //here is only one map server for test
            MapServerResult ms = await DatabaseManager.SelectAsync<MapServerResult>($"SELECT address AS ServerAddr, port AS ServerPort FROM MapServer WHERE id=1");
            ms.Result = 1;

            session.Send(ms.ToPacket());
        }

        

        /// <summary>
        /// Protocol 002A
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        /// <returns></returns>
        public void GetMailAndFriendList(EcoSession session, BasePacket packet)
        {
            Logger.Debug($"Received CheckCharaID Request :{packet.Data.ToHexString()}");
            uint chara_id = packet.Data.ToUInt32();
            session.Send(new BasePacket(0x002b));

            //todo: send mail and friend list
            var friendList = new SelfFriendListInfo();
            friendList.State = 0;
            friendList.Comment = "comment";
            session.Send(friendList.ToPacket());
        }

    }
}
