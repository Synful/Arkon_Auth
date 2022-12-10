using Server;
using Server.Connections;
using Server.Utils;
using Server_GUI.Util;
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;

namespace Server_GUI.Controls {
    public partial class CommandBox : UserControl {

        List<string> cmd_history = new List<string>();
        int cmd_history_index = 0;

        public CommandBox() {
            InitializeComponent();
        }

        private void txtCommand_PreviewKeyDown(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.Enter:
                    cmd_history_index = 0;
                    cmd_history.Add(txtCommand.Text);
                    handle_command(txtCommand.Text.Split(' '));
                    txtCommand.Clear();
                    break;
                case Key.Up:
                    if (cmd_history.Count > 0) {
                        cmd_history_index++;
                        if (cmd_history_index > cmd_history.Count-1) {
                            cmd_history_index = 0;
                        }
                        txtCommand.Text = cmd_history[cmd_history_index];
                    }
                    break;
                case Key.Down:
                    if (cmd_history.Count > 0) {
                        cmd_history_index--;
                        if(cmd_history_index < 0) {
                            cmd_history_index = cmd_history.Count-1;
                        }
                        txtCommand.Text = cmd_history[cmd_history_index];
                    }
                    break;
                default:
                    break;
            }
        }

        private void handle_command(string[] args) {
            if (args == null) {
                return;
            }

            switch(args[0]) {
                case "start":
                    Logger.Instance.Log += AppLog.Log;
                    Logger.Instance.Info("Starting Up Server...");
                    UserConnection.Instance.Startup();
                    AdminConnection.Instance.Startup();
                    break;
                case "shutdown":
                    Logger.Instance.Info("Server Shutting Down....");

                    UserConnection.Instance.Shutdown();
                    Logger.Instance.Info("Disconnected All Users & Disposed Listeners.");

                    AdminConnection.Instance.Shutdown();
                    Logger.Instance.Info("Disconnected All Admins & Disposed Listeners.");

                    GC.Collect();
                    Logger.Instance.Info("Server Has Shutdown.");
                    Logger.Instance.Log -= AppLog.Log;
                    break;
            }
        }
    }
}
