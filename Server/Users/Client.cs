using Server.Packets;
using Server.Events;
using Server.Utils;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server.Users {
    public class Client {
        // BackgroundWorker for handling incoming data
        private BackgroundWorker recv_bw;

        // BackgroundWorker for handling outgoing data
        private BackgroundWorker send_bw;

        // Semaphore to prevent multiple threads from accessing the
        // send_bw concurrently
        private Semaphore send_sem = new Semaphore(1, 1);

        // Socket for the client connection
        private Socket socket;

        // NetworkStream for the socket
        private NetworkStream stream;

        // Message object for reading and writing data to the network stream
        private Message msg;

        // Property for the client's IP address
        public IPAddress ip {
            get {
                // Use a null-conditional operator to avoid checking if socket is null
                // before accessing its properties
                return ((IPEndPoint)this.socket?.RemoteEndPoint)?.Address;
            }
        }

        // Property for the client's port
        public int port {
            get {
                // Use a null-conditional operator to avoid checking if socket is null
                // before accessing its properties
                return ((IPEndPoint)this.socket?.RemoteEndPoint)?.Port ?? 0;
            }
        }

        // Constructor for the Client class
        public Client(Socket socket) {
            // Store the socket
            this.socket = socket;

            // Create a new network stream for the socket
            this.stream = new NetworkStream(this.socket);

            // Create a new Message object for the stream
            this.msg = new Message(this.stream, false);

            // Create a new BackgroundWorker for handling incoming data
            this.recv_bw = new BackgroundWorker();

            // Set the DoWork event handler for the recv_bw
            this.recv_bw.DoWork += new DoWorkEventHandler(Recv);

            // Start the recv_bw
            this.recv_bw.RunWorkerAsync();

            // Create a new BackgroundWorker for handling outgoing data
            this.send_bw = new BackgroundWorker();

            // Set the DoWork event handler for the send_bw
            this.send_bw.DoWork += new DoWorkEventHandler(Send);

            // Set the RunWorkerCompleted event handler for the send_bw
            this.send_bw.RunWorkerCompleted += Send_Complete;
        }

        // Method for handling incoming data
        private void Recv(object sender, DoWorkEventArgs e) {
            // Keep looping while there is data to be read from the stream
            while (msg.load_data()) {
                try {
                    // Read an integer from the stream and cast it to a Types enum value
                    Types cmd_type = (Types)msg.read_int();

                    switch (cmd_type) {
                        case Types.Login:
                            // Handle a login packet
                            break;
                        case Types.Packet:
                            // Handle a packet
                            break;
                        default:
                            // Handle other packet types
                            break;
                    }

                    // Invoke the CommandReceived event with a CommandEventArgs object
                    // containing the packet data
                    OnCommandReceived(new CommandEventArgs(msg));

                } catch { }
            }

            // The client has disconnected, so invoke the Disconnected event
            this.OnDisconnected(new ClientEventArgs(this.socket));

            // Disconnect the socket
            this.Disconnect();
        }

        // Method for handling outgoing data
        private void Send(object sender, DoWorkEventArgs e) {
            // Cast the argument to a Types enum value
            Types cmd_type = (Types)e.Argument;

            switch (cmd_type) {
                case Types.Login:
                    // Handle sending a login packet
                    break;
                case Types.Packet:
                    // Handle sending a regular packet
                    break;
            }
        }

        // Method for handling the completion of the send_bw
        private void Send_Complete(object sender, RunWorkerCompletedEventArgs e) {
            // Perform garbage collection
            GC.Collect();
        }

        // Method for sending a command to the client
        public void Send_Command(object cmd) {
            // Start the send_bw with the command as an argument
            send_bw.RunWorkerAsync(cmd);
        }

        // Method for disconnecting the client
        public bool Disconnect() {
            if (this.socket != null && this.socket.Connected) {
                try {
                    // Shutdown and close the socket
                    this.socket.Shutdown(SocketShutdown.Both);
                    this.socket.Close();

                    // Perform garbage collection
                    GC.Collect();

                    // Return true if the disconnection was successful
                    return true;
                } catch {
                    // Return false if an error occurred while disconnecting
                    return false;
                }
            } else
                // Return true if the socket is null or not connected
                return true;
        }

        #region Events
        // Event for when a command is received from the client
        public event CommandReceivedEventHandler CommandReceived;
        protected virtual void OnCommandReceived(CommandEventArgs e) {
            if (CommandReceived != null)
                CommandReceived(this, e);
        }

        // Event for when a command is sent to the client
        //public event CommandSentEventHandler CommandSent;
        //protected virtual void OnCommandSent(CommandEventArgs e) {
        //    if (CommandSent != null)
        //        CommandSent(this, e);
        //}

        // Event for when a command fails to be sent to the client
        //public event CommandSendingFailedEventHandler CommandFailed;
        //protected virtual void OnCommandFailed(EventArgs e) {
        //    if (CommandFailed != null)
        //        CommandFailed(this, e);
        //}

        // Event for when the client disconnects
        public event DisconnectedEventHandler Disconnected;
        protected virtual void OnDisconnected(ClientEventArgs e) {
            if (Disconnected != null)
                Disconnected(this, e);
        }
        #endregion
    }
}