using System;
using System.Collections.Generic;
using System.IO;

namespace Server.Utils {
    public class Logger {
        private class Log {
            public enum lType {
                Info,
                Warn,
                Error,
                Command
            }

            lType type;
            string date;
            string msg;

            private void writetype(lType t) {
                switch (t) {
                    case lType.Info:
                        Console.write("[", ConsoleColor.Gray);
                        Console.write("Info", ConsoleColor.Magenta);
                        Console.write("]", ConsoleColor.Gray);
                        break;
                    case lType.Warn:
                        Console.write("[", ConsoleColor.Gray);
                        Console.write("Warn", ConsoleColor.Yellow);
                        Console.write("]", ConsoleColor.Gray);
                        break;
                    case lType.Error:
                        Console.write("[", ConsoleColor.Gray);
                        Console.write("Error", ConsoleColor.Red);
                        Console.write("]", ConsoleColor.Gray);
                        break;
                    case lType.Command:
                        Console.write("[", ConsoleColor.Gray);
                        Console.write("Command", ConsoleColor.Cyan);
                        Console.write("]", ConsoleColor.Gray);
                        break;
                }
                Console.write(" ");
            }

            public void render() {
                Console.write($"{date} ", ConsoleColor.Gray);
                writetype(type);
                Console.write($"{msg}\n", ConsoleColor.Gray);
            }

            public Log(lType t, string d, string m) {
                type = t;
                date = d;
                msg = m;
            }
        }

        private static Logger _inst;
        public static Logger inst {
            get {
                if (_inst == null) {
                    _inst = new Logger();
                }
                return _inst;
            }
        }

        private List<Log> logs = new List<Log>(10);

        private string infofile  = "Logs/info.log";
        private string warnfile  = "Logs/warn.log";
        private string errorfile = "Logs/error.log";
        private string cmdfile   = "Logs/cmd.log";

        private void pushlog(Log l) {
            if (logs.Count == 10) {
                logs.RemoveAt(0);
                logs.Add(l);
            } else {
                logs.Add(l);
            }
        }

        public Logger() {
            if (!Directory.Exists("Logs")) {
                Directory.CreateDirectory("Logs");
            }
        }

        public void render_logs() {
            foreach (Log log in logs) {
                log.render();
            }
        }

        public void Info(string msg) {
            pushlog(new Log(Log.lType.Info, $"{DateTime.Now.ToString("MM/dd/yy hh:mm:ss tt")}", msg));
            File.AppendAllText(infofile, $"{DateTime.Now.ToString("MM/dd/yy hh:mm:ss tt")} [Info] {msg}\n");
        }
        public void Warn(string msg) {
            pushlog(new Log(Log.lType.Warn, $"{DateTime.Now.ToString("MM/dd/yy hh:mm:ss tt")}", msg));
            File.AppendAllText(warnfile, $"{DateTime.Now.ToString("MM/dd/yy hh:mm:ss tt")} [Warn] {msg}\n");
        }
        public void Error(string msg) {
            pushlog(new Log(Log.lType.Error, $"{DateTime.Now.ToString("MM/dd/yy hh:mm::ss tt")}", msg));
            File.AppendAllText(errorfile, $"{DateTime.Now.ToString("MM/dd/yy hh:mm:ss tt")} [Error] {msg}\n");
        }
        public void Command(string msg) {
            pushlog(new Log(Log.lType.Command, $"{DateTime.Now.ToString("MM/dd/yy hh:mm:ss tt")}", msg));
            File.AppendAllText(cmdfile, $"{DateTime.Now.ToString("MM/dd/yy hh:mm:ss tt")} [Command] {msg}\n");
        }
    }
}
