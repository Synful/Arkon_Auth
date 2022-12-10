using Server.Utils;
using System;
using System.Net;

namespace Server.Packets {
    public class Login : Packet {
        public Login(IPAddress from_ip) : base(from_ip) {

        }

        public override void Serialize(ref Message msg) {

            base.Serialize(ref msg);
        }

        public override void Deserialize(Message msg) {

            base.Deserialize(msg);
        }

        public override void Execute() {

        }

        
    }
}
