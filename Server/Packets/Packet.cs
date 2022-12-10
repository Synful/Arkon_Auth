using Server.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Server.Packets {
    public class Packet {

        public IPAddress to_ip;
        public IPAddress from_ip;

        public Packet(IPAddress from_ip) {
            this.from_ip = from_ip;
        }
        public Packet(IPAddress to_ip, IPAddress from_ip) {
            this.to_ip = to_ip;
            this.from_ip = from_ip;
        }

        public virtual void Serialize(ref Message msg) {
            msg.write_string(to_ip.ToString());
            msg.write_string(from_ip.ToString());
        }

        public virtual void Deserialize(Message msg) {

        }

        public virtual void Execute() { }

    }
}
