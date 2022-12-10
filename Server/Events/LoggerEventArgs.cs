using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server.Events {
    public enum LoggerEventType {
        Info,
        Warn,
        Error,
        Command
    }

    public delegate void LogEventHandler(object sender, LoggerEventArgs e);
    public class LoggerEventArgs : EventArgs {
        public LoggerEventType EventType;
        public string Message;

        public LoggerEventArgs(LoggerEventType logType, string message) {
            this.EventType = logType;
            this.Message = message;
        }
    }
}
