using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EncryptedMessanger.ClientNet.Controllers;
using EncryptedMessanger.Data;
using EncryptedMessanger.ClientNet.Modules;
using Microsoft.EntityFrameworkCore;

namespace EncryptedMessanger.ClientNet
{
    public static class GenericClient
    {
        public static async Task<int> RequestContactId() {
            Client client = new Client(clientId: 1);
            client.GeneratePacket(Handler.Request, "newContactId", "");
            await client.TransmitPacket();
            client.DeserializePacketData();
            string resp = null;
            while (resp == null) {
                try {
                    resp = client.GetResponse();
                } catch { continue; }
            }
            return Convert.ToInt32(resp);
        }
    }

    public class Client
    {
        public int ClientId { get; set; }
        public int Port { get; set; }
        public string HostAddress { get; set; }
        public Packet? Packet { get; set; }
        private Queue<Packet> _packetQueue;
        public ClientContext ClientContext { get; private set; }
        private Thread _serviceThread;
        //Reset Events
        private ManualResetEvent _msgRequestComplete;
        private ManualResetEvent _msgCheckComplete;
        private ManualResetEvent _packetCheckComplete;

        //debug method
        private void PurgePackets() {
            var msgList = ClientContext.Packets.ToList();
            ClientContext.Packets.RemoveRange(msgList);
            ClientContext.SaveChanges();
        }
        public Client(int clientId = 0, int port = 5542, string host = "127.0.0.1")
        {
            ClientId = clientId == 0 ? RandomNumberGenerator.GetInt32(1000, 99999) : clientId;
            Port = port;
            HostAddress = host;
            _packetQueue = new Queue<Packet>();
            ClientContext = new ClientContext();
            _serviceThread = new Thread(ServiceThread);
            PurgePackets();
        }
        public async void MessageRequest()
        {
            try {
                this.GeneratePacket(Handler.Request, "messages", "");
                await TransmitPacket();
                this.HandlePacketResponse();
            } catch (Exception e) {
                Console.WriteLine(e);
                Thread.Sleep(5000);
            }
            _msgRequestComplete.Set();
        }

        public async Task TransmitPacket() {
            var packetData = Packet.PacketData;
            var searializedData = JsonSerializer.Serialize(packetData);
            await this.StartClient(searializedData);
        }

        public void StartClientService() {
            if (_serviceThread.IsAlive) { return; }
            _serviceThread.Start();
        }

        private async void ServiceThread() {
            while (true)
            {
                await QueuePastPackets();
                _msgRequestComplete = new ManualResetEvent(false);
                _packetCheckComplete = new ManualResetEvent(false);
                Thread.Sleep(2000);
                try
                {
                    // Msg Request
                    MessageRequest();
                    _msgRequestComplete.WaitOne();
                    //Thread.Sleep(1000);
                    await CheckForOutboundPackets();
                    _packetCheckComplete.WaitOne();
                } catch (Exception e) {
                    Console.WriteLine(e.ToString());
                }

            }

        }

        private async Task CheckForOutboundPackets() {
            if (_packetQueue.Count > 0)
            {
                try
                {
                    for (int i = 0; i <= _packetQueue.Count; i++)
                    {
                        Packet = _packetQueue.Peek();
                        await TransmitPacket();
                        var response = this.HandlePacketResponse();
                        if (response == 1)
                        {
                            _packetQueue.Dequeue();
                            ClientContext.Packets.Remove(Packet);
                            await ClientContext.SaveChangesAsync();
                        }

                    }
                } catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            _packetCheckComplete.Set();
        }
        /// <summary>
        /// Get Cached Messages and queue them to be sent
        /// </summary>
        /// <returns></returns>
        private async Task QueuePastPackets() {
            var messages = await ClientContext.Packets.ToListAsync();
            foreach (var message in messages) {
                _packetQueue.Enqueue(message);
            }
            Console.WriteLine($"Found {messages.Count} Packets");
        }
        public void QueuePacket() {
            _packetQueue.Enqueue(Packet);
            ClientContext.Packets.Add(Packet);
            ClientContext.SaveChanges();
        }

        public string GetResponse() => Packet.PacketData.Response;

    }

    public static class Connector {
        private static Client _clientObj { get; set; }

        // ManualResetEvents for signal completion
        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);

        // Response from server
        public static String Response = String.Empty;

        public static void RefreshConnector()
        {
            connectDone = new ManualResetEvent(false);
            sendDone = new ManualResetEvent(false);
            receiveDone = new ManualResetEvent(false);
        }

        public static async Task<int> StartClient(this Client clientObj, string data)
        {
            RefreshConnector();
            _clientObj = clientObj;
            // Connect to server
            try
            {
                // Establish Connection with server
                IPHostEntry ipHostEntry = Dns.GetHostEntry(_clientObj.HostAddress);
                IPAddress ipAddress = ipHostEntry.AddressList[0];
                IPEndPoint remoteEndPoint = new IPEndPoint(ipAddress, _clientObj.Port);

                // Create client socket
                Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                
                // Connect to the server
                client.BeginConnect(remoteEndPoint,
                    new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();
                if (!client.Connected) return 0;
                // Send data to server
                string readyData = data + "<EOF>";
                Send(client, readyData);
                sendDone.WaitOne();

                // Receive Response from server
                Receive(client);
                receiveDone.WaitOne();
                if (!client.Connected) return 0;

                // Do something with Response
                // *** RESPONSE RETURNED IN CLIENT OBJECT***
                
                // Release Socket
                await CheckConnection(client);
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }

            clientObj.Packet.PacketData.Response = Response;
            return 1;
        }

        private static async Task<bool> CheckConnection(Socket sock) {
            await Task.Yield();
            if (sock.Connected) return true;
            
            sock.Shutdown(SocketShutdown.Both);
            sock.Close();
            return false;
        }

        private static void Receive(Socket client) {
            try
            {
                // Create Packet
                Packet packet = new Packet();
                packet.Socket = client;

                // Begin receiving data from server
                client.BeginReceive(packet.Buffer, 0, Packet.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), packet);

            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void ReceiveCallback(IAsyncResult ar) {
            try
            {
                // Get packet and socket from async state object
                Packet packet = (Packet)ar.AsyncState;
                Socket client = packet.Socket;

                //Read Data
                int bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    packet.StringBuilder.Append(Encoding.Default.GetString(packet.Buffer, 0, bytesRead));

                    client.BeginReceive(packet.Buffer, 0, Packet.BufferSize, 0,
                        new AsyncCallback(ReceiveCallback), packet);
                } else {
                    // All data has arrived; put it in Response
                    if (packet.StringBuilder.Length > 1)
                    {
                        Response = packet.StringBuilder.ToString();
                    }
                    // Signal all bytes received
                    receiveDone.Set();
                }
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ConnectCallback(IAsyncResult ar) {
            try
            {
                // Get socket from packet object
                Socket client = (Socket) ar.AsyncState;

                // complete the connection
                client.EndConnect(ar);

            } catch (SocketException e)
            {

                Console.WriteLine(e.ToString());
            } catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            // Signal that the connection has been made
            connectDone.Set();
        }

        private static void Send(Socket client, string data) {
            // Convert string to bytes
            byte[] byteData = Encoding.Default.GetBytes(data);

            // Begin sending data to server
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar) {
            try {
                // Get socket from state object
                Socket client = (Socket) ar.AsyncState;

                // Complete sending data to server
                int bytesSent = client.EndSend(ar);

                // Signal send completed
                sendDone.Set();
            } catch (Exception e) {
                Console.WriteLine(e);
            }
        }
    }
}
