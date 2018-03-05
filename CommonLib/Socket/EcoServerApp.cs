using CommonLib.Packets;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Config;
using System;
using System.Linq;
using System.Text;

namespace CommonLib.Socket
{
    public delegate void NewRequestCallback(EcoSession session, BasePacket packet);

    public class EcoServerApp
    {

        static readonly byte[] INIT_PACKET = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10 };

        private PacketKey packetKey ;
        private EcoServer server;
        public EcoServerApp(int port, NewRequestCallback newReuqestCallback/*, SessionHandler<EcoSession> newSessionCallBack*/)
        {

            server = new EcoServer();

            //crypt = 

            packetKey = new PacketKey();

            packetKey.serverKey = new ServerKey();

            // Add request handler
            server.NewRequestReceived += (session, reuqestInfo)=> {
                //Logger.Debug("New Message Received.");
                if (session.ExchangeKeyFinished)
                {
                    var data = session.Crypt.Decrypt(reuqestInfo.Body, 8);
                    var packet = new BasePacket(data);
                    newReuqestCallback(session, packet);
                }
                else //first connection
                {
                    if (reuqestInfo.Body.SequenceEqual(INIT_PACKET))
                    {
                        byte[] server_key_bytes = packetKey.serverKey.PublicKeyBytes;
                        session.Send(server_key_bytes, 0, server_key_bytes.Length);
                        Logger.Debug($"Send Data: {server_key_bytes.ToHexString()} to client, Session ID:{session.SessionID}");
                    }
                    else
                    {
                        if(reuqestInfo.Body.Length != 260)
                        {
                            return;
                        }
                        packetKey.clientKey = new ClientKey(reuqestInfo.Body);
                        Logger.Debug($"client public key:{packetKey.clientKey.PublicKey.ToString("x")}, length:{packetKey.clientKey.PublicKeySize}");
                        string sharedKey = packetKey.GetSharedKey();
                        Logger.Debug($"shared key:{sharedKey}, length:{sharedKey.Length}");
                        session.Crypt.SetAesKey(packetKey.GetRijndaelKey(Encoding.ASCII.GetBytes(sharedKey)));
                        session.ExchangeKeyFinished = true;
                    }
                }
            };
            //server.NewSessionConnected += newSessionCallBack;

            var serverConfig = new ServerConfig
            {
                Port = port, //set the listening port
                Ip = "Any",
                Name = "EcoServer",
                //Other configuration options
                Mode = SocketMode.Tcp,
                MaxConnectionNumber = 1000,
                IdleSessionTimeOut = 100,
                SyncSend = false,
                LogFactory = "SuperSocket.SocketBase.Logging.ConsoleLogFactory",
                LogBasicSessionActivity = true,
                KeepAliveTime = 60, // 10 minutes
                KeepAliveInterval = 60,// 60 seconds
                //MaxRequestLength = 8096,
                LogAllSocketException = true                
            };

            if (!server.Setup(serverConfig))
            {
                Console.WriteLine("server setup failed!");
            }
        }

        public void Start()
        {
            if (!server.Start())
            {
                Console.WriteLine("server start failed!");
                return;
            }

            Console.WriteLine("Server Started!");
        }

        public void Stop()
        {
            server?.Stop();
        }
    }
}
