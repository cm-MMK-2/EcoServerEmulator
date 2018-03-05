using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
    public class Logger
    {
        private static ILog instance = null;
        private Logger(){}
        public static void Initialize()
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            instance = LogManager.GetLogger(typeof(Logger));
        }

        public static void Debug(string log)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($@"{DateTime.Now.ToString()}  {log}");
            instance.Debug(log);
        }

        public static void Info(string log)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($@"{DateTime.Now.ToString()}  {log}");
            instance.Info(log);
        }

        public static void Error(string log)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($@"{DateTime.Now.ToString()}  {log}");
            instance.Error(log);
        }

        public static void Error(Exception ex)
        {
            string error_str = $"{ex.StackTrace}\n{ex.Message}";
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(error_str);
            instance.Error(error_str);
        }
    }
}
