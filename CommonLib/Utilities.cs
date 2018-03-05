using CommonLib.Packets;
using CommonLib.Socket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

/***********************************************
  *
  *　　　　　　iヽ　　　　　　/ヽ 
  *　　　 　 　|　ﾞ、　　　　/ 　ﾞi 
  *　 　 　 　 |　　  ﾞ''─'''"　  l 
  *　　　　　,/　　　 　 　 　 　　ヽ 
  *　　　　 ,iﾞ 　　　  　 　 　 　  \
  *　　　 　i!　　　●　 　 　　●　  |i　　　　　　　 
  *　　　　 ﾞi,,　　* （__人__）　　,/　　　　　　　　　　　 
  *　　　　　 ヾ､,,　　　/￣￣￣￣￣￣￣￣￣/　　　　　 
  *　　 　　 /ﾞ "　　　 /　　　　          /
  *　　　   "⌒ﾞヽ　　 /　　　 　 　　　  /
  *　　　　 |　　 　 i/　　　　　　      /
  *  ====== ヽ,＿,,ノ/＿＿＿＿＿＿＿＿＿/ ========
  *
  *  <author>cm</author>
  *
  ***********************************************/

namespace CommonLib
{
    public class InterfaceProtocol
    {
        public string ID { get; set; }
        public string InterfaceName { get; set; }
    }

    public class Settings
    {
        public string ConnectionString { get; set; }
    }

    public class Utilities
    {
        public static InterfaceProtocol[] ReadInterfaceConfig(string path)
        {
            try
            {
                string interface_data = string.Empty;
                if (File.Exists(path))
                {
                    interface_data = File.ReadAllText(path);
                    return JsonConvert.DeserializeObject<InterfaceProtocol[]>(interface_data);
                }
                else
                {
                    Logger.Error($"Can not find interface config file at path: <{path}>");
                    return null;
                }

            }
            catch (Exception e)
            {
                Logger.Error(e.Message + "\n" + e.StackTrace);
                return null;
            }
        }

        public static Settings ReadSettings(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    string settings_data = File.ReadAllText(path);
                    return JsonConvert.DeserializeObject<Settings>(settings_data);
                }
                else
                {
                    Logger.Error($"Can not find settings file at path: <{path}>");
                    return null;
                }

            }
            catch (Exception e)
            {
                Logger.Error(e.Message + "\n" + e.StackTrace);
                return null;
            }
        }


        public static void CallInterfaceByName(object interfaceInstance, ref InterfaceProtocol[] protocols, EcoSession session, BasePacket packet)
        {
            try
            {
                int index = Array.FindIndex(protocols, i => (i.ID.ToUpper() == packet.ProtocolID.ToString("X4")));
                //object dataOut = null;
                var interfaceType = interfaceInstance.GetType();
                if (index >= 0)
                {
                    MethodInfo method = interfaceType.GetMethod(protocols[index].InterfaceName);
                    
                    if (method != null)
                    {
                        //Logger.Debug($"{method.Name} get called in {interfaceInstance.ToString()}");
                        method.Invoke(interfaceInstance, new object[] { session, packet });
                    }
                    else
                    {
                        Logger.Error($"Can not find function: <{protocols[index].InterfaceName}> in interface: <{interfaceType.ToString()}>");
                    }
                }
                else
                {
                    Logger.Error($"Cannot find protocol id:<{packet.ProtocolID.ToString("X4")}>");
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Call interface error, {e.Message}\n{e.StackTrace}");
            }
        }

        public static byte[] PasswordHash(string password, string front_word, string back_word)
        {
            using (SHA1 sha = new SHA1CryptoServiceProvider())
            //using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                string hashstr = front_word + password + back_word;
                var hash_bytes = Encoding.ASCII.GetBytes(hashstr);
                return sha.ComputeHash(hash_bytes);
            }
        }

        //public static byte[] CallInterfaceByName(object interfaceInstance, ref InterfaceProtocol[] protocols,EcoSession session, string interfaceId, byte[] data)
        //{
        //    try
        //    {
        //        int index = Array.FindIndex(protocols, i => (i.ID == interfaceId));
        //        object dataOut = null;
        //        var interfaceType = interfaceInstance.GetType();
        //        if (index >= 0)
        //        {
        //            MethodInfo method = interfaceType.GetMethod(protocols[index].InterfaceName);
        //            if (method != null)
        //            {
        //                dataOut = method.Invoke(interfaceInstance, new object[] { session, data });
        //            }
        //            else
        //            {
        //                Logger.Error($"Can not find function: <{protocols[index].InterfaceName}> in interface: <{interfaceType.ToString()}>");
        //            }
        //        }
        //        else
        //        {
        //            Logger.Error($"Cannot find interface id:<{interfaceId}>");
        //        }
        //        return (byte[])dataOut;
        //    }
        //    catch (Exception e)
        //    {
        //        Logger.Error($"Call interface error, {e.Message}\n{e.StackTrace}");
        //        return null;
        //    }
        //}
    }
}
