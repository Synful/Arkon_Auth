using Server.Events;
using Server.Users;
using Server.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Connections {
    public class UserConnection {

        // Instance of the UserConnection class
        private static UserConnection _instance;
        public static UserConnection Instance {
            get {
                if (_instance == null) {
                    // Create a new instance of the UserConnection class if one does not exist
                    _instance = new UserConnection();
                }
                return _instance;
            }
        }

        // List of connected clients
        public List<User> users = new List<User>();

        // BackgroundWorker for listening for incoming connections
        BackgroundWorker listener_bw;

        // Socket for listening for incoming connections
        Socket listener_s;

        // IPEndPoint for the listener socket
        IPEndPoint listener_ep;

        // Flag indicating whether the server is running
        bool is_alive = false;

        // Constructor for the UserConnection class
        public UserConnection() { }

        // Method for starting the server
        public void Startup() {
            // Set the is_alive flag to true to indicate that the server is running
            is_alive = true;

            // Create a new IPEndPoint for the listener socket
            listener_ep = new IPEndPoint(IPAddress.Parse("192.168.1.2"), 666);

            // Create a new BackgroundWorker for the listener socket
            listener_bw = new BackgroundWorker();
            listener_bw.WorkerSupportsCancellation = true;
            listener_bw.DoWork += new DoWorkEventHandler(Listen);
            listener_bw.RunWorkerAsync();

            // Invoke the ConnectionStateChange event with a ConnectionStateChangeEventArgs object
            // containing the current state of the server
            OnConnectionStateChange(new ConnectionStateChangeEventArgs(is_alive));
        }

        // Method for stopping the server
        public void Shutdown() {
            // Set the is_alive flag to false to indicate that the server is not running
            is_alive = false;

            // Disconnect all currently connected clients
            if (users != null)
                foreach (User user in users)
                    user.Disconnect();

            // Cancel and dispose of the listener_bw
            listener_bw.CancelAsync();
            listener_bw.Dispose();

            // Shutdown and close the listener socket
            try {
                listener_s.Shutdown(SocketShutdown.Both);
                listener_s.Disconnect(true);
            } catch { }
            listener_s.Close();

            // Invoke the ConnectionStateChange event with a ConnectionStateChangeEventArgs object
            // containing the current state of the server
            OnConnectionStateChange(new ConnectionStateChangeEventArgs(is_alive));
        }

        // Method for listening for incoming connections
        private void Listen(object sender, DoWorkEventArgs e) {
            // Create a new listener socket and bind it to the IPEndPoint
            listener_s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener_s.Bind(listener_ep);
            listener_s.Listen(200);

            // Continuously accept incoming connections while the server is running
            while (is_alive) {
                try {
                    // Add the incoming connection to the users list
                    Add(listener_s.Accept());
                } catch { }
            }
        }

        // Method for adding a new client to the users list
        private void Add(Socket socket) {
            // Create a new User object for the incoming connection
            User user = new User(socket);

            // Subscribe to the Disconnected event for the user
            user.Disconnected += new DisconnectedEventHandler(Disconnected);

            // Check for an abnormal disconnection from the client
            Check_Abnormal_Disconnect(user.ip);

            // Add the user to the users list
            users.Add(user);

            // Invoke the CountChange event with a ClientCountChangeEventArgs object
            // containing the current number of connected clients
            OnCountChange(new ClientCountChangeEventArgs(users.Count.ToString()));

            // Log a message indicating that the user has connected
            Logger.Instance.Info($"User {user.ip}:{user.port} Connected.");
        }

        // Method for checking for an abnormal disconnection from a client
        private bool Check_Abnormal_Disconnect(IPAddress ip) {
            lock (this) {
                // Find the user with the specified IP address in the users list
                User f_user = users.Find(o => (o.ip == ip));

                // Return false if the user was not found
                if (f_user == null)
                    return false;

                // Remove the user from the users list
                users.Remove(f_user);

                // Invoke the CountChange event with a ClientCountChangeEventArgs object
                // containing the current number of connected clients
                OnCountChange(new ClientCountChangeEventArgs(users.Count.ToString()));

                // Log a warning message indicating that the user has disconnected abnormally
                Logger.Instance.Warn($"User {ip} abnormally disconnected.");
                return true;
            }
        }

        // Method for handling a client disconnection
        private void Disconnected(object sender, ClientEventArgs e) {
            lock (this) {
                // Find the user with the specified IP address in the users list
                User f_user = users.Find(o => (o.ip == e.IP));

                // Return if the user was not found
                if (f_user == null)
                    return;

                // Remove the user from the users list
                users.Remove(f_user);

                // Invoke the CountChange event with a ClientCountChangeEventArgs object
                // containing the current number of connected clients
                OnCountChange(new ClientCountChangeEventArgs(users.Count.ToString()));

                // Log a message indicating that the user has disconnected
                Logger.Instance.Info($"User {e.IP}:{e.Port} Disconnected.");
            }
        }

        // Event for handling changes to the number of connected clients
        public event ClientCountChangeEventHandler CountChange;

        // Method for raising the CountChange event
        protected virtual void OnCountChange(ClientCountChangeEventArgs e) {
            if (CountChange != null)
                CountChange(this, e);
        }

        // Event for handling changes to the connection state of the server
        public event ConnectionStateChangeEventHandler ConnectionStateChange;

        // Method for raising the ConnectionStateChange event
        protected virtual void OnConnectionStateChange(ConnectionStateChangeEventArgs e) {
            if (ConnectionStateChange != null)
                ConnectionStateChange(this, e);
        }
    }
}
