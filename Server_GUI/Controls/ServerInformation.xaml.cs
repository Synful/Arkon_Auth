using Server.Connections;
using Server.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Server_GUI.Controls {
    public partial class ServerInformation : UserControl {
        public ServerInformation() {
            InitializeComponent();
            UserConnection.Instance.CountChange += OnUserCountChanged;
            UserConnection.Instance.ConnectionStateChange += OnUserConnectionStateChanged;

            AdminConnection.Instance.CountChange += OnAdminCountChanged;
            AdminConnection.Instance.ConnectionStateChange += OnAdminConnectionStateChanged;
        }

        private void OnUserCountChanged(object sender, ClientCountChangeEventArgs e) {
            Dispatcher.Invoke(new Action(() => {
                txtUsersOnline.Content = e.Count;
            }));
        }
        private void OnUserConnectionStateChanged(object sender, ConnectionStateChangeEventArgs e) {
            Dispatcher.Invoke(new Action(() => {
                txtUserStatus.Content = e.State ? "Online" : "Offline";
                txtUserStatus.Foreground = e.State ? new SolidColorBrush(Color.FromArgb(255, 0, 255, 0)) : new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            }));
        }

        private void OnAdminCountChanged(object sender, ClientCountChangeEventArgs e) {
            Dispatcher.Invoke(new Action(() => {
                txtAdminsOnline.Content = e.Count;
            }));
        }
        private void OnAdminConnectionStateChanged(object sender, ConnectionStateChangeEventArgs e) {
            Dispatcher.Invoke(new Action(() => {
                txtAdminStatus.Content = e.State ? "Online" : "Offline";
                txtAdminStatus.Foreground = e.State ? new SolidColorBrush(Color.FromArgb(255, 0, 255, 0)) : new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            }));
        }
    }
}
