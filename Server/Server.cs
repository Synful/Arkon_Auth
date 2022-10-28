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

namespace Server {
    public class Server {
        private static Server instance;
        public static Server Instance {
            get {
                if (instance == null) {
                    instance = new Server();
                }
                return instance;
            }
        }
        public bool IsAlive = false;

        public List<User> users = new List<User>();
        BackgroundWorker user_listener_bw;
        Socket user_listener_s;
        IPEndPoint user_listener_ep;

        public Server() {

            IsAlive = true;

            user_listener_ep = new IPEndPoint(IPAddress.Parse("192.168.1.2"), 666);

            user_listener_bw = new BackgroundWorker();
            user_listener_bw.WorkerSupportsCancellation = true;
            user_listener_bw.DoWork += new DoWorkEventHandler(User_Listen);
            user_listener_bw.RunWorkerAsync();

            admin_listener_ep = new IPEndPoint(IPAddress.Parse("192.168.1.2"), 667);

            admin_listener_bw = new BackgroundWorker();
            admin_listener_bw.WorkerSupportsCancellation = true;
            admin_listener_bw.DoWork += new DoWorkEventHandler(Admin_Listen);
            admin_listener_bw.RunWorkerAsync();

            Menu menu = Menu.inst;
        }

        private void User_Listen(object sender, DoWorkEventArgs e) {
            user_listener_s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            user_listener_s.Bind(user_listener_ep);
            user_listener_s.Listen(200);

            while (IsAlive) {
                try {
                    Add_User(user_listener_s.Accept());
                } catch { }
            }
        }

        private void Add_User(Socket socket) {
            User user = new User(socket);
            user.Disconnected += new DisconnectedEventHandler(User_Disconnected);
            Check_User_Abnormal_Disconnect(user.ip);
            users.Add(user);
            Logger.inst.Info($"User {user.ip}:{user.port} Connected.");
        }
        private bool Check_User_Abnormal_Disconnect(IPAddress ip) {
            lock (this) {
                User f_user = users.Find(o => (o.ip == ip));
                if (f_user != null) {
                    users.Remove(f_user);
                    Logger.inst.Warn($"User {ip} abnormally disconnected.");
                    return true;
                }
                return false;
            }
        }
        private void User_Disconnected(object sender, ClientEventArgs e) {
            lock (this) {
                User f_user = users.Find(o => (o.ip == e.IP));
                if (f_user != null) {
                    users.Remove(f_user);
                    Logger.inst.Info($"User {e.IP}:{e.Port} Disconnected.");
                }
            }
        }

        public List<Admin> admins = new List<Admin>();
        BackgroundWorker admin_listener_bw;
        Socket admin_listener_s;
        IPEndPoint admin_listener_ep;

        private void Admin_Listen(object sender, DoWorkEventArgs e) {
            admin_listener_s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            admin_listener_s.Bind(admin_listener_ep);
            admin_listener_s.Listen(200);

            while (IsAlive) {
                try {
                    Add_Admin(admin_listener_s.Accept());
                } catch { }
            }
        }

        private void Add_Admin(Socket socket) {
            Admin admin = new Admin(socket);
            admin.Disconnected += new DisconnectedEventHandler(Admin_Disconnected);
            Check_Admin_Abnormal_Disconnect(admin.ip);
            admins.Add(admin);
            Logger.inst.Info($"Admin {admin.ip}:{admin.port} Connected.");
        }
        private bool Check_Admin_Abnormal_Disconnect(IPAddress ip) {
            lock (this) {
                Admin f_admin = admins.Find(o => (o.ip == ip));
                if (f_admin != null) {
                    admins.Remove(f_admin);
                    Logger.inst.Warn($"Admin {ip} abnormally disconnected.");
                    return true;
                }
                return false;
            }
        }
        private void Admin_Disconnected(object sender, ClientEventArgs e) {
            lock (this) {
                Admin f_admin = admins.Find(o => (o.ip == e.IP));
                if (f_admin != null) {
                    admins.Remove(f_admin);
                    Logger.inst.Info($"Admin {e.IP}:{e.Port} Disconnected.");
                }
            }
        }

        public void Shutdown() {
            System.Console.Title = "Server Shutting Down....";

            IsAlive = false;
            Logger.inst.Info("IsAlive = false");

            if (users != null)
                foreach (User user in users)
                    user.Disconnect();

            user_listener_bw.CancelAsync();
            user_listener_bw.Dispose();

            try {
                user_listener_s.Shutdown(SocketShutdown.Both);
                user_listener_s.Disconnect(true);
            } catch { }
            user_listener_s.Close();

            Logger.inst.Info("Disconnected All Users & Disposed Listeners.");

            if (admins != null)
                foreach (Admin admin in admins)
                    admin.Disconnect();

            admin_listener_bw.CancelAsync();
            admin_listener_bw.Dispose();

            try {
                admin_listener_s.Shutdown(SocketShutdown.Both);
                admin_listener_s.Disconnect(true);
            } catch { }
            admin_listener_s.Close();

            Logger.inst.Info("Disconnected All Admins & Disposed Listeners.");

            GC.Collect();
            //Environment.Exit(0);
        }
    }
}
