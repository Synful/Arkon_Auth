using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Utils {
    public class Console {
        public class Line {
            public KeyValuePair<string, ConsoleColor> single_line { get; set; }
            public Dictionary<string, ConsoleColor> multi_line { get; set; }
            public int line_number { get ; set; }
        }
        static List<Line> lines = new List<Line>();

        public static void write(string s) {
            System.Console.Write(s);
        }
        public static void write(string s, ConsoleColor c) {
            System.Console.ForegroundColor = c;
            System.Console.Write(s);
            System.Console.ResetColor();
        }

        public static void add_line(string s) {
            lines.Add(new Line() {
                single_line = new KeyValuePair<string, ConsoleColor>(s, ConsoleColor.Black),
                line_number = lines.Count + 1
            });
        }
        public static void add_line(string s, ConsoleColor c) {
            lines.Add(new Line() {
                single_line = new KeyValuePair<string, ConsoleColor>(s, c),
                line_number = lines.Count + 1
            });
        }

        public static void render_lines() {
            foreach(Line l in lines) {
                System.Console.SetCursorPosition(0, l.line_number);
                if (l.multi_line != null) {
                    foreach (KeyValuePair<string, ConsoleColor> pair in l.multi_line) {
                        System.Console.ForegroundColor = pair.Value;
                        System.Console.Write(pair.Key);
                        System.Console.ResetColor();
                    }
                } else {
                    if (l.single_line.Value != ConsoleColor.Black) {
                        System.Console.ForegroundColor = l.single_line.Value;
                    }
                    System.Console.Write(l.single_line.Key);
                    if (l.single_line.Value != ConsoleColor.Black) {
                        System.Console.ResetColor();
                    }
                }
            }
        }

        public static void add_title_line(string s) {
            Dictionary<string, ConsoleColor> value = new Dictionary<string, ConsoleColor>();
            value.Add("====[ ", ConsoleColor.Gray);
            value.Add($"{s}", ConsoleColor.Cyan);
            value.Add(" ]====\n", ConsoleColor.Gray);
            lines.Add(new Line() {
                multi_line = value,
                line_number = lines.Count+1
            });
        }
        public static void add_command_line(string name, string desc) {
            Dictionary<string, ConsoleColor> value = new Dictionary<string, ConsoleColor>();
            value.Add($"{name}", ConsoleColor.Cyan);
            value.Add(" - ", ConsoleColor.Gray);
            value.Add($"{desc}\n", ConsoleColor.Cyan);
            lines.Add(new Line() {
                multi_line = value,
                line_number = lines.Count+1
            });
        }
    }
}
