using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Client.Utils;
using Message = Client.Utils.Message;

namespace Client {
    public partial class MainForm : Form {
        public MainForm() {
            InitializeComponent();
        }

        Socket user_sock;
        NetworkStream user_stream;
        Message user_msg;

        public bool User_Disconnect() {
            if (this.user_sock != null && this.user_sock.Connected) {
                try {
                    this.user_msg.Close();
                    this.user_sock.Shutdown(SocketShutdown.Both);
                    this.user_sock.Close();
                    return true;
                } catch {
                    return false;
                }

            } else
                return true;
        }

        private void button1_Click(object sender, EventArgs e) {
            user_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            user_sock.Connect(new IPEndPoint(IPAddress.Parse("24.102.209.242"), 666));
            user_stream = new NetworkStream(user_sock);
            user_msg = new Message(user_stream, false);
        }

        private void button2_Click(object sender, EventArgs e) {
            user_msg.write_int(1);
            user_msg.send_data();
        }

        private void button3_Click(object sender, EventArgs e) {
            User_Disconnect();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            try {
                User_Disconnect();
            } catch { }
            try {
                Admin_Disconnect();
            } catch { }
        }

        Socket admin_sock;
        NetworkStream admin_stream;
        Message admin_msg;

        public bool Admin_Disconnect() {
            if (this.admin_sock != null && this.admin_sock.Connected) {
                try {
                    this.admin_msg.Close();
                    this.admin_sock.Shutdown(SocketShutdown.Both);
                    this.admin_sock.Close();
                    return true;
                } catch {
                    return false;
                }

            } else
                return true;
        }

        private void button6_Click(object sender, EventArgs e) {
            admin_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            admin_sock.Connect(new IPEndPoint(IPAddress.Parse("24.102.209.242"), 667));
            admin_stream = new NetworkStream(admin_sock);
            admin_msg = new Message(admin_stream, false);
        }

        private void button5_Click(object sender, EventArgs e) {
            admin_msg.write_int(1);
            admin_msg.send_data();
        }

        private void button4_Click(object sender, EventArgs e) {
            Admin_Disconnect();
        }
    }
}
