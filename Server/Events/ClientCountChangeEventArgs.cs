using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server.Events {
    public delegate void ClientCountChangeEventHandler(object sender, ClientCountChangeEventArgs e);
    public class ClientCountChangeEventArgs : EventArgs {
        public string Count;

        public ClientCountChangeEventArgs(string count) {
            Count = count;
        }
    }
}
