using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Events {
    public delegate void DisconnectedEventHandler(object sender, ClientEventArgs e);
    public class ClientEventArgs : EventArgs {
        private Socket socket;
        public IPAddress IP {
            get { return ((IPEndPoint)this.socket.RemoteEndPoint).Address; }
        }
        public int Port {
            get { return ((IPEndPoint)this.socket.RemoteEndPoint).Port; }
        }
        public ClientEventArgs(Socket clientManagerSocket) {
            this.socket = clientManagerSocket;
        }
    }
}
