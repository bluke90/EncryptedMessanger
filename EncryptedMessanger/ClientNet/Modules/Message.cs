using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EncryptedMessanger.ClientNet.Controllers;
using EncryptedMessanger.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace EncryptedMessanger.ClientNet.Modules
{
    public class Message
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int RecipientId { get; set; }
        public string Data { get; set; }
    }

    public static class MessageUtils
    {
        public static Task SendMessageToServer(this Client client, Message msg) {
            try
            {
                // Get data context
                ClientContext context = new ClientContext();
                // Serialize Message to json format
                var serializedMsg = msg.SerializeMessage();
                // Generate a client packet using the seialized message data
                client.GeneratePacket(Handler.Store, "messages", serializedMsg);
                Console.WriteLine(
                    $"Packet Generated => PacketID: {client.Packet.Id} | PacketData-PacketID {client.Packet.PacketData.PacketId}" +
                    $"\n      PacketType: {client.Packet.PacketData.Handler} | PacketParam: {client.Packet.PacketData.Params}");
                // Transmit packet
                client.QueuePacket();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return Task.CompletedTask;
        }

        public static string SerializeMessage(this Message message)
        {
            var data = JsonSerializer.Serialize(message);
            return data;
        }

        public static List<Message> DeserializeMessageResponse(this PacketData data)
        {
            var msgs = data.Response.Split("||");
            var msgList = new List<Message>();
            foreach (var msg in msgs)
            {
                if (msg.Length > 1)
                {
                    msgList.Add(JsonSerializer.Deserialize<Message>(msg));
                }
            }

            return msgList;
        }
    }

    public class MessageHandler
    {
        public readonly ClientContext _context;
        public Queue<Message> _msgQueue;
        private Thread _serviceThread;
        private Client _client;
        public MessageHandler(Client client)
        {
            _client = client;
            _context = new ClientContext();
            _msgQueue = new Queue<Message>();
            _serviceThread = new Thread(ServiceThread);
        }

        public void Start()
        {
            _serviceThread.Start();
        }

        /// <summary>
        /// MessageHandler Service Thread
        /// </summary>
        private async void ServiceThread()
        {
            // Check for cached messages
            await QueuePastMessages();
            while (true)
            {
                // If Queue is not empty, attempt to transmit message                
                await CheckForOutboundMessages();
                Thread.Sleep(2000);
            }
        }

        public async Task CheckForOutboundMessages() {
            if (_msgQueue.Count > 0)
            {
                for (int i = 0; i <= _msgQueue.Count; i++)
                {
                    //Get next message in queue and attempt transmission
                    var msg = _msgQueue.Peek();
                    await _client.SendMessageToServer(msg);
                }
            }
        }

        /// <summary>
        /// Get Cached Messages and queue them to be sent
        /// </summary>
        /// <returns></returns>
        private async Task QueuePastMessages()
        {
            var messages = await _context.Messages.ToListAsync();
            foreach (var message in messages)
            {
                _msgQueue.Enqueue(message);
            }
        }
        /// <summary>
        /// Add message to message cache and queue for transmission
        /// </summary>
        /// <param name="msg">The message that you would like to queue</param>
        public void QueueNewMessage(Message msg)
        {
            _context.Messages.Add(msg);
            _msgQueue.Enqueue(msg);
            _context.SaveChanges();
        }
    }
}
