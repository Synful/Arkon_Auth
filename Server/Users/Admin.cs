using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server.Users {
    public class Admin : Client {

        public Admin(Socket socket) : base(socket) {

        }

    }
}
