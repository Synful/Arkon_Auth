using System;

namespace Server {
    class Program {
        static Program _inst;
        public static Program inst {
            get {
                if (_inst == null)
                    _inst = new Program();
                return _inst;
            }
        }

        static void Main(string[] args) {
            Program.inst.run();
        }

        public void run() {

           Server serv = Server.Instance;

        }
    }
}
