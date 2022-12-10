using Client.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client.Connections {
    public class UserConnection {

        Socket socket;
        NetworkStream stream;
        Message message;

        public void Connect() {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(IPAddress.Parse("24.102.209.242"), 666));
            stream = new NetworkStream(socket);
            message = new Message(stream, false);
        }

        public bool Disconnect() {
            if (socket == null || !socket.Connected)
                return false;

            try {
                this.message.Close();
                this.socket.Shutdown(SocketShutdown.Both);
                this.socket.Close();
                return true;
            } catch {
                return false;
            }
        }
    }
}
