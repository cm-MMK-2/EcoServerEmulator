using CommonLib.Packets;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;

namespace CommonLib.Socket
{

    public class EcoRequestInfo: IRequestInfo
    {
        public EcoRequestInfo(string _key, byte[] _body)
        {
            Key = _key;
            Body = _body;
        }
        public string Key { get; }

        public byte[] Body { get; }
    }

    public class EcoReceiveFilter : IReceiveFilter<EcoRequestInfo>
    {
        private byte[] buffer;

        //private const int MaxBufferSize = 8096; // 8K

        public EcoRequestInfo Filter(byte[] readBuffer, int offset, int length, bool toBeCopied, out int rest)
        {
            try
            {
                rest = 0;
                buffer = new byte[length];
                Buffer.BlockCopy(readBuffer, offset, buffer, 0, length);
                return new EcoRequestInfo(null, buffer);
            }
            catch (Exception ex)
            {
                rest = 0;
                Reset();
                Logger.Error(ex);
                return null;
            }
        }

        public void Reset()
        {
            buffer = null;
        }

        public int LeftBufferSize { get; }
        public IReceiveFilter<EcoRequestInfo> NextReceiveFilter { get; }
        public FilterState State { get; }
    }

    public class EcoServer : AppServer<EcoSession, EcoRequestInfo>
    {

        public EcoServer():base(new DefaultReceiveFilterFactory<EcoReceiveFilter, EcoRequestInfo>())
        {

        }

        protected override bool Setup(IRootConfig rootConfig, IServerConfig config)
        {

            return base.Setup(rootConfig, config);
        }

        protected override void OnStarted()
        {
            base.OnStarted();
        }

        protected override void OnStopped()
        {
            base.OnStopped();
        }

    }

    public class EcoSession : AppSession<EcoSession, EcoRequestInfo>
    {
        /// <summary>
        /// exchange key status
        /// </summary>
        public bool ExchangeKeyFinished { get; set; }
        /// <summary>
        /// use for encrpt and decrypt
        /// </summary>
        public Encryption Crypt { get; set; }

        /// <summary>
        /// add to front of password before sha1 hash
        /// </summary>
        public UInt32 FrontWord { get; set; }

        /// <summary>
        /// add to back of password before sha1 hash
        /// </summary>
        public UInt32 BackWord { get; set; }

        /// <summary>
        /// login status flag
        /// </summary>
        public bool UserLoginSuccess { get; set; }

        public uint AccountID { get; set; }

        public uint CharaID { get; set; }

        public List<EcoSession> AllSessions { get; set; }

        protected override void OnSessionStarted()
        {
            CommonLib.Logger.Debug($"user: {this.LocalEndPoint.Address} connected");
            Crypt = new Encryption();
            //this.Send("Welcome to Eco Server");
        }

        protected override void HandleUnknownRequest(EcoRequestInfo requestInfo)
        {
            //this.Send("Unknow request");
        }

        protected override void HandleException(Exception e)
        {
            //this.Send("Application error: {0}", e.Message);
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            //add you logics which will be executed after the session is closed
            CommonLib.Logger.Debug($"user: {this.LocalEndPoint.Address} disconnected");
            AllSessions?.Remove(this);
            base.OnSessionClosed(reason);
        }

        public void Send(byte[] data)
        {
            var enBytes = Crypt.Encrypt(data);
            CommonLib.Logger.Debug($"Send Data: {data.ToHexString()}\nEncrypted As:{enBytes.ToHexString()}\nto User: {this.LocalEndPoint.Address}(Session:{this.SessionID})");
            base.Send(enBytes, 0, enBytes.Length);
        }

        public void Send(BasePacket packet)
        {
            Send(packet.ToBytes());
        }
    }
}
