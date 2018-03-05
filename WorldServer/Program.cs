using CommonLib;
using CommonLib.Packets;
using CommonLib.Socket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldServer
{

    class Program
    {

        static InterfaceProtocol[] protocols;
        static WorldServerProtocol worldServerInstance;

        static void EcoServer_NewRequestReceived(EcoSession session,BasePacket packet)
        {
            Logger.Debug($"New Message from {session.SessionID} received, MessageID: {packet.ProtocolID.ToString("X").PadLeft(4, '0')}, Data: {packet.Data.ToHexString()}");

            /*var rtnPacket = */Utilities.CallInterfaceByName(worldServerInstance, ref protocols, session, packet);
            //if(rtnPacket != null)
            //{
            //    session.Send(rtnPacket.ToBytes());
            //}
        }

        static void Main(string[] args)
        {
            Logger.Initialize();

            protocols = Utilities.ReadInterfaceConfig("WorldServerInterface.json");
            worldServerInstance = new WorldServerProtocol();
            var serverApp = new EcoServerApp(17831, EcoServer_NewRequestReceived);
            serverApp.Start();
            Console.ReadLine();
        }
    }
}
