using CommonLib;
using CommonLib.Data;
using CommonLib.Packets;
using CommonLib.Socket;
using WorldServer.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace WorldServer
{
    public class WorldServerProtocol
    {
        private readonly byte[] firstAck;
        RNGCryptoServiceProvider rng;

        public WorldServerProtocol()
        {
            firstAck = new BasePacket(0xffff, new byte[] { 0xe8, 0x6a, 0x6a, 0xca, 0xdc, 0xe8, 0x06, 0x05, 0x2b, 0x29, 0xf8, 0x96, 0x2f, 0x86, 0x7c, 0xab, 0x2a, 0x57, 0xad, 0x30 }).ToBytes();
            rng = new RNGCryptoServiceProvider();
        }

        ~WorldServerProtocol()
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
                //byte[] front_word = new byte[4];
                //byte[] back_word = new byte[4];
                //rng.GetBytes(front_word);
                //rng.GetBytes(back_word);
                //front_word[0] = 0;
                //back_word[0] = 0;
                //session.FrontWord = front_word.ToUInt32();
                //session.BackWord = back_word.ToUInt32();
                //var pass_hash = new BasePacket(0x001e, front_word.Concat(back_word).ToArray()).ToBytes();
                session.FrontWord = 1111;
                session.BackWord = 1111;
                var pass_hash = new BasePacket(0x001e, session.FrontWord.ToBytes().Concat(session.BackWord.ToBytes()).ToArray()).ToBytes();
                session.Send(pass_hash);
                session.Send(new BasePacket(0x0002, packet.Data));
            }
            catch(Exception e)
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
            var login_data = new LoginData(packet.Data);
            Logger.Debug($"Received Login Request, Username: {login_data.Username}, Password:{login_data.Password}, MacAddress:{login_data.MacAddress.ToHexString()}, SingleSignOn:{login_data.SingleSignOn.ToString()}");
            LoginAuthResult loginAuthResult;
            //select user info from db
            Account userAccount = await DatabaseManager.SelectAsync<Account>("username", $@"'{login_data.Username}'");
            
            if(userAccount.is_online)
            {
                loginAuthResult = new LoginAuthResult(LoginResultEnum.GAME_SMSG_LOGIN_ERR_ALREADY, 1);
                session.Send(loginAuthResult.ToPacket());
                Logger.Debug($"USER: {login_data.Username} has already logged in.");
                return;
            }

            if(userAccount.is_banned)
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

           
            session.Send(loginAuthResult.ToPacket());

        }

        /// <summary>
        /// Protocol 002F
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        /// <returns></returns>
        public void LoginStatusCheck(EcoSession session, BasePacket packet)
        {
            int allowLogin = -1;
            if(session.UserLoginSuccess)
            {
                allowLogin = 0;
            }
            session.Send(new BasePacket(0x0030, allowLogin.ToBytes()));
        }

        /// <summary>
        /// Protocol 0031
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        /// <returns></returns>
        public async void RequestServerList(EcoSession session, BasePacket packet)
        {
            session.Send(new BasePacket(0x0032));

            List<ServerInfo> serverList = await DatabaseManager.SelectMultiAsync<ServerInfo>("SELECT Id, Name, Address, Port, Status FROM LoginServer");

            foreach (var server in serverList)
            {
                session.Send(server.ToPacket());
            }

            session.Send(new BasePacket(0x0034));

            session.Send(new DisconnectionInfo(1).ToPacket());

            session.Close();
        }
    }
}
