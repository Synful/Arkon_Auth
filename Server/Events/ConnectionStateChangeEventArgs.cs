using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Events {
    public delegate void ConnectionStateChangeEventHandler(object sender, ConnectionStateChangeEventArgs e);
    public class ConnectionStateChangeEventArgs : EventArgs {
        public bool State;

        public ConnectionStateChangeEventArgs(bool state) {
            State = state;
        }
    }
}
