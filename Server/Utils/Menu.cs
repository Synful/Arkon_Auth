using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Utils {
    public class Menu {
        static Menu _inst;
        public static Menu inst {
            get {
                if (_inst == null)
                    _inst = new Menu();
                return _inst;
            }
        }

        Thread render_t;
        Thread choice_t;
        Thread settings_t;

        public Menu() {



            render_t = new Thread(() => render_thread());
            render_t.Start();
            choice_t = new Thread(() => choice_thread());
            choice_t.Start();
            //settings_t = new Thread(() => settings_thread());
            //settings_t.Start();
        }

        void render_thread() {
            while (Server.Instance.IsAlive) {
                try {
                    System.Console.Clear();

                    Console.add_title_line("Arkon Server Log");

                    Logger.inst.render_logs();

                    Console.add_line("");

                    render_menu();
                } catch { }
                Thread.Sleep(3 * 1000);
            }
        }
        void choice_thread() {
            while (Server.Instance.IsAlive) {
                try {
                    string cmd = System.Console.ReadLine().ToLower();
                    switch (cmd) {
                        case "kill":
                            Server.Instance.Shutdown();
                            break;
                        case "reload":
                            //Settings.inst.Load();
                            break;
                    }
                } catch { };
                Thread.Sleep(1000);
            }
        }
        void settings_thread() {
            while (true) {
                try {
                    //Settings.inst.Load();
                } catch { }
                Thread.Sleep(60 * 1000);
            }
        }

        void render_header() {
            Console.add_title_line("Arkon Server Info");

            Console.write("Admins Online", ConsoleColor.Cyan);
            Console.write(": ", ConsoleColor.Gray);
            Console.write($"{Server.Instance.admins.Count}", ConsoleColor.White);

            Console.write(" | ", ConsoleColor.Gray);

            Console.write("Users Online", ConsoleColor.Cyan);
            Console.write(": ", ConsoleColor.Gray);
            Console.write($"{Server.Instance.users.Count}\n\n", ConsoleColor.White);
        }
        void render_commands() {
            Console.add_title_line("Arkon Server Command Line");

            Console.add_command_line("kill", "Stops the server & disconnects all users.");
            Console.add_command_line("reload", "Reloads the server settings.");
            Console.write("\n");
        }
        void render_menu() {
            render_header();
            render_commands();

            writeinput();
        }
        void writeinput() {
            int x = System.Console.CursorLeft;
            int y = System.Console.CursorTop;
            System.Console.CursorTop = System.Console.WindowTop + System.Console.WindowHeight - 1;
            Console.write("root", ConsoleColor.Cyan);
            Console.write("@", ConsoleColor.Gray);
            Console.write("Arkon", ConsoleColor.Cyan);
            Console.write(".", ConsoleColor.Gray);
            Console.write("auth", ConsoleColor.Cyan);
            Console.write("# ", ConsoleColor.Gray);
            System.Console.ForegroundColor = ConsoleColor.Cyan;
            // Restore previous position
            //System.Console.SetCursorPosition(x, y);
        }
    }
}
