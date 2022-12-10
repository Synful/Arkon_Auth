using Server.Events;
using Server.Users;
using Server.Utils;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;

namespace Server.Connections {
    public class AdminConnection {

        private static AdminConnection _instance;
        public static AdminConnection Instance {
            get {
                if (_instance == null) {
                    _instance = new AdminConnection();
                }
                return _instance;
            }
        }

        public List<Admin> admins = new List<Admin>();
        BackgroundWorker listener_bw;
        Socket listener_s;
        IPEndPoint listener_ep;

        bool is_alive = false;

        public AdminConnection() { }

        public void Startup() {
            is_alive = true;

            listener_ep = new IPEndPoint(IPAddress.Parse("192.168.1.2"), 667);

            listener_bw = new BackgroundWorker();
            listener_bw.WorkerSupportsCancellation = true;
            listener_bw.DoWork += new DoWorkEventHandler(Listen);
            listener_bw.RunWorkerAsync();

            OnConnectionStateChange(new ConnectionStateChangeEventArgs(is_alive));
        }

        public void Shutdown() {
            is_alive = false;

            if (admins != null)
                foreach (Admin admin in admins)
                    admin.Disconnect();

            listener_bw.CancelAsync();
            listener_bw.Dispose();

            try {
                listener_s.Shutdown(SocketShutdown.Both);
                listener_s.Disconnect(true);
            } catch { }
            listener_s.Close();

            OnConnectionStateChange(new ConnectionStateChangeEventArgs(is_alive));
        }

        private void Listen(object sender, DoWorkEventArgs e) {
            listener_s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener_s.Bind(listener_ep);
            listener_s.Listen(200);

            while (is_alive) {
                try {
                    Add(listener_s.Accept());
                } catch { }
            }
        }

        private void Add(Socket socket) {
            Admin admin = new Admin(socket);
            admin.Disconnected += new DisconnectedEventHandler(Disconnected);
            Check_Abnormal_Disconnect(admin.ip);
            admins.Add(admin);
            OnCountChange(new ClientCountChangeEventArgs(admins.Count.ToString()));
            Logger.Instance.Info($"Admin {admin.ip}:{admin.port} Connected.");
        }
        private bool Check_Abnormal_Disconnect(IPAddress ip) {
            lock (this) {
                Admin f_admin = admins.Find(o => (o.ip == ip));
                if (f_admin == null)
                    return false;

                admins.Remove(f_admin);
                OnCountChange(new ClientCountChangeEventArgs(admins.Count.ToString()));
                Logger.Instance.Warn($"Admin {ip} abnormally disconnected.");
                return true;
            }
        }
        private void Disconnected(object sender, ClientEventArgs e) {
            lock (this) {
                Admin f_admin = admins.Find(o => (o.ip == e.IP));
                if (f_admin == null)
                    return;

                admins.Remove(f_admin);
                OnCountChange(new ClientCountChangeEventArgs(admins.Count.ToString()));
                Logger.Instance.Info($"Admin {e.IP}:{e.Port} Disconnected.");
            }
        }

        public event ClientCountChangeEventHandler CountChange;
        protected virtual void OnCountChange(ClientCountChangeEventArgs e) {
            if (CountChange != null)
                CountChange(this, e);
        }

        public event ConnectionStateChangeEventHandler ConnectionStateChange;
        protected virtual void OnConnectionStateChange(ConnectionStateChangeEventArgs e) {
            if (ConnectionStateChange != null)
                ConnectionStateChange(this, e);
        }
    }
}
