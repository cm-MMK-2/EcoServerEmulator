﻿using CommonLib;
using CommonLib.Packets;
using CommonLib.Socket;
using System;
using System.Collections.Generic;

namespace MapServer
{
    class Program
    {
        static InterfaceProtocol[] protocols;
        static MapServerProtocol mapServerInstance;

        static List<EcoSession> allSessions = new List<EcoSession>();

        static void EcoServer_NewRequestReceived(EcoSession session, BasePacket packet)
        {
            Logger.Debug($"New Message from {session.SessionID} received, MessageID: {packet.ProtocolID.ToString("X").PadLeft(4, '0')}, Data: {packet.Data.ToHexString()}");
            allSessions.Add(session);
            session.AllSessions = allSessions;
            Utilities.CallInterfaceByName(mapServerInstance, ref protocols, session, packet);
        }

        static void Main(string[] args)
        {
            Logger.Initialize();
            protocols = Utilities.ReadInterfaceConfig("MapServerInterface.json");
            mapServerInstance = new MapServerProtocol();
            var serverApp = new EcoServerApp(17833, EcoServer_NewRequestReceived);
            serverApp.Start();
            while (Console.ReadKey().Key != ConsoleKey.Escape) { }
        }
    }
}
