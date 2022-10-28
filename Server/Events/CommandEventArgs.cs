using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Events {
    public delegate void CommandReceivedEventHandler(object sender, CommandEventArgs e);
    public delegate void CommandSentEventHandler(object sender, CommandEventArgs e);
    public delegate void CommandSendingFailedEventHandler(object sender, EventArgs e);

    public class CommandEventArgs : EventArgs {
        public object Command;

        public CommandEventArgs(object cmd) {
            this.Command = cmd;
        }
    }
}
