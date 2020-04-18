using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FlightSimulatorApp.Model
{
    public partial class MyTelnetClient : ITelnetClient
    {
        // fields
        private TcpClient client;
        public int connect(string ip, int port)
        {
            this.client = new TcpClient();
            client.ReceiveTimeout = 10000;
            try
            {
                client.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
            }
            catch
            {
                throw new Exception("Could not connect");
            }

            return 1;
        }

        public void disconnect()
        {
            if (client != null)
                this.client.Close();
        }

        public bool isConnected()
        {
            return this.client.Connected;
        }

        public string read()
        {
            byte[] buffer = new byte[1024];
            var receivedBytesCount = client.GetStream().Read(buffer, 0, buffer.Length);
            var receivedString = Encoding.ASCII.GetString(buffer, 0, receivedBytesCount);
            return receivedString;
        }

        public void write(string command)
        {
            byte[] buffer = new byte[1024];
            var encoding = Encoding.ASCII;
            NetworkStream stream = client.GetStream();
            var bytesToSend = encoding.GetBytes(command);
            stream.Write(bytesToSend, 0, bytesToSend.Length);
            stream.Flush();
        }
    }
}