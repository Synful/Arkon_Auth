using Server.Commands;
using Server.Events;
using Server.Utils;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Console = Server.Utils.Console;

namespace Server.Users {
    public class Client {
        private BackgroundWorker recv_bw;
        private BackgroundWorker send_bw;
        private Semaphore send_sem = new Semaphore(1, 1);

        private Socket socket;
        private NetworkStream stream;
        private Message msg;

        public IPAddress ip {
            get {
                if (socket != null) {
                    return ((IPEndPoint)this.socket.RemoteEndPoint).Address;
                } else {
                    return null;
                }
            }
        }
        public int port {
            get {
                if (socket != null) {
                    return ((IPEndPoint)this.socket.RemoteEndPoint).Port;
                } else {
                    return 0;
                }
            }
        }

        public Client(Socket socket) {
            this.socket = socket;
            this.stream = new NetworkStream(this.socket);
            this.msg = new Message(this.stream, false);

            this.recv_bw = new BackgroundWorker();
            this.recv_bw.DoWork += new DoWorkEventHandler(Recv);
            this.recv_bw.RunWorkerAsync();

            this.send_bw = new BackgroundWorker();
            this.send_bw.DoWork += new DoWorkEventHandler(Send);
            this.send_bw.RunWorkerCompleted += Send_Complete;
        }

        private void Recv(object sender, DoWorkEventArgs e) {
            while(msg.load_data()) {
                try {
                    Types cmd_type = (Types)msg.read_int();

                    switch (cmd_type) {
                        case Types.Command:
                            break;
                        case Types.Login:
                            break;
                        default:
                            break;
                    }
                } catch { }
            }
            this.OnDisconnected(new ClientEventArgs(this.socket));
            this.Disconnect();
        }

        private void Send(object sender, DoWorkEventArgs e) {
            switch (e.Argument) {
                case Login:
                    Console.add_line("Found Login.");
                    break;
                case Command:
                    Console.add_line("Found Command.");
                    break;
            }
        }
        private void Send_Complete(object sender, RunWorkerCompletedEventArgs e) {
            GC.Collect();
        }

        public void Send_Command(object cmd) {
            send_bw.RunWorkerAsync(cmd);
        }

        public bool Disconnect() {
            if (this.socket != null && this.socket.Connected) {
                try {
                    this.socket.Shutdown(SocketShutdown.Both);
                    this.socket.Close();
                    GC.Collect();
                    return true;
                } catch {
                    return false;
                }
            } else
                return true;
        }

        #region Events
        public event CommandReceivedEventHandler CommandReceived;
        protected virtual void OnCommandReceived(CommandEventArgs e) {
            if (CommandReceived != null)
                CommandReceived(this, e);
        }

        //public event CommandSentEventHandler CommandSent;
        //protected virtual void OnCommandSent(CommandEventArgs e) {
        //    if (CommandSent != null)
        //        CommandSent(this, e);
        //}

        //public event CommandSendingFailedEventHandler CommandFailed;
        //protected virtual void OnCommandFailed(EventArgs e) {
        //    if (CommandFailed != null)
        //        CommandFailed(this, e);
        //}

        public event DisconnectedEventHandler Disconnected;
        protected virtual void OnDisconnected(ClientEventArgs e) {
            if (Disconnected != null)
                Disconnected(this, e);
        }
        #endregion
    }
}
