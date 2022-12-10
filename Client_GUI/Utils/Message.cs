using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Client.Utils {
    public class Message {
        private NetworkStream stream;
        private bool reverse_bytes;

        public Message(NetworkStream s, bool rb = true) {
            stream = s;
            reverse_bytes = rb;
        }
        public void Close() {
            this.stream.Close();
        }

        #region Write
        private byte[] send_buffer = new byte[2048];
        private int send_offset = 0;
        public void write_data(byte[] buffer, int size) {
            for (int i = 0; i < size; i++) {
                send_buffer[send_offset + i] = buffer[i];
            }
            send_offset += size;
        }
        public void write_int(int i) {
            byte[] buffer = BitConverter.GetBytes(i);
            if (reverse_bytes) {
                Array.Reverse(buffer);
            }
            write_data(buffer, 4);
        }
        public void write_uint(uint i) {
            byte[] buffer = BitConverter.GetBytes(i);
            if (reverse_bytes) {
                Array.Reverse(buffer);
            }
            write_data(buffer, 4);
        }
        public void write_float(float f) {
            byte[] buffer = BitConverter.GetBytes(f);
            if (reverse_bytes) {
                Array.Reverse(buffer);
            }
            write_data(buffer, 4);
        }
        public void write_bool(bool b) {
            byte[] buffer = BitConverter.GetBytes(b);
            write_data(buffer, 1);
        }
        public void write_string(string s) {
            byte[] buffer = Encoding.ASCII.GetBytes(s);
            write_data(buffer, buffer.Length);
        }
        public void send_data() {
            stream.Write(send_buffer, 0, send_buffer.Length);
            stream.Flush();
            send_buffer = new byte[2048];
            send_offset = 0;
        }
        #endregion

        #region Read
        private byte[] recv_buffer = new byte[2048];
        private int recv_offset = 0;
        public bool load_data() {
            recv_buffer = new byte[2048];
            recv_offset = 0;
            int read_bytes = stream.Read(recv_buffer, 0, 2048);
            return (read_bytes == 0 ? false : true);
        }
        public byte[] read_data(int size) {
            byte[] buf = new byte[size];
            Array.Copy(recv_buffer, recv_offset, buf, 0, size);
            recv_offset += size;
            return buf;
        }
        public int read_int() {
            byte[] buffer = read_data(4);
            if (reverse_bytes) {
                Array.Reverse(buffer);
            }
            return 0;//BitConverter.ToInt32(buffer);
        }
        public bool read_bool() {
            return false;//BitConverter.ToBoolean(read_data(1));
        }
        public string read_string() {
            string ret = "";
            for (int i = recv_offset; i < recv_buffer.Length; i++) {
                char c = (char)recv_buffer[i];
                byte u = recv_buffer[i];
                if (u == 0x00) {
                    recv_offset++;
                    break;
                } else {
                    ret += c;
                    recv_offset++;
                }
            }
            return Regex.Replace(ret, @"[^\u0000-\u007F]+", string.Empty).Replace("\0", "");
        }
        #endregion
    }
}
