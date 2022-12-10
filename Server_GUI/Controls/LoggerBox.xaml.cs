using Server_GUI.Util;
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
    /// <summary>
    /// Interaction logic for LoggerBox.xaml
    /// </summary>
    public partial class LoggerBox : UserControl {
        public LoggerBox() {
            InitializeComponent();
            AppLog.Logger = rtbLogger;
        }
    }
}
