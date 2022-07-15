using CommonLib;
using CommonLib.Packets;
using CommonLib.Socket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoginServer
{
    class Program
    {
        static InterfaceProtocol[] protocols;
        static LoginServerProtocol loginServerInstance;

        static void EcoServer_NewRequestReceived(EcoSession session, BasePacket packet)
        {
            Logger.Debug($"New Message from {session.SessionID} received, MessageID: {packet.ProtocolID.ToString("X").PadLeft(4, '0')}, Data: {packet.Data.ToHexString()}");

            Utilities.CallInterfaceByName(loginServerInstance, ref protocols, session, packet);

        }

        static void Main(string[] args)
        {
            Logger.Initialize();
            protocols = Utilities.ReadInterfaceConfig("LoginServerInterface.json");
            loginServerInstance = new LoginServerProtocol();
            var serverApp = new EcoServerApp(17832, EcoServer_NewRequestReceived);
            serverApp.Start();
            while (Console.ReadKey().Key != ConsoleKey.Escape) { }
        }
    }
}
