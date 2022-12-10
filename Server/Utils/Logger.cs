using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Windows;
using Server.Events;

namespace Server.Utils {
    public class Logger {

        private static Logger _Instance;
        public static Logger Instance {
            get {
                if (_Instance == null) {
                    _Instance = new Logger();
                }
                return _Instance;
            }
        }


        private string infofile  = "Logs/info.log";
        private string warnfile  = "Logs/warn.log";
        private string errorfile = "Logs/error.log";
        private string cmdfile   = "Logs/cmd.log";

        public Logger() {
            if (!Directory.Exists("Logs")) {
                Directory.CreateDirectory("Logs");
            }
        }

        public void Info(string msg) {
            string date = $"{DateTime.Now.ToString("MM/dd/yy hh:mm:ss tt")}";
            File.AppendAllText(infofile, $"{date} [Info] {msg}\n");
            OnLog(new LoggerEventArgs(LoggerEventType.Info, msg));
        }
        public void Warn(string msg) {
            string date = $"{DateTime.Now.ToString("MM/dd/yy hh:mm:ss tt")}";
            File.AppendAllText(warnfile, $"{date} [Warn] {msg}\n");
            OnLog(new LoggerEventArgs(LoggerEventType.Warn, msg));
        }
        public void Error(string msg) {
            string date = $"{DateTime.Now.ToString("MM/dd/yy hh:mm:ss tt")}";
            File.AppendAllText(errorfile, $"{date} [Error] {msg}\n");
            OnLog(new LoggerEventArgs(LoggerEventType.Error, msg));
        }
        public void Command(string msg) {
            string date = $"{DateTime.Now.ToString("MM/dd/yy hh:mm:ss tt")}";
            File.AppendAllText(cmdfile, $"{date} [Command] {msg}\n");
            OnLog(new LoggerEventArgs(LoggerEventType.Command, msg));
        }

        public event LogEventHandler Log;
        protected virtual void OnLog(LoggerEventArgs e) {
            if (Log != null)
                Log(this, e);
        }
    }
}
