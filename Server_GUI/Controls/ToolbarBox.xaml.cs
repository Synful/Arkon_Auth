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
    public partial class ToolbarBox : UserControl {
        public ToolbarBox() {
            InitializeComponent();
        }

        private void dragMoveBar_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                Window.GetWindow(this).DragMove();
            }
        }
        private void closeBtn_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }
        private void miniBtn_Click(object sender, RoutedEventArgs e) {
            Window.GetWindow(this).WindowState = WindowState.Minimized;
        }
    }
}
