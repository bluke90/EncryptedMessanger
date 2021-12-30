using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EncryptedMessanger.Data;
using EncryptedMessanger.ClientNet.Modules;
using Microsoft.VisualBasic.CompilerServices;


namespace EncryptedMessanger.ClientNet.Handlers
{
    public static class RequestHandler
    {
        public static void AnalyzeRequestResponse(this Packet packet)
        {
            ClientContext context = new ClientContext();
            var param = packet.PacketData.Params;
            switch (param)
            {
                case "messages":
                    // Deserialize Messages
                    var list = packet.PacketData.DeserializeMessageResponse();
                    if (list == null || list.Count == 0) break;
                    var intList = new List<int>();
                    // Store messages in client context
                    foreach (var msg in list)
                    {
                        intList.Add(msg.Id);
                        msg.Id = 0;
                        context.Messages.Add(msg);
                    }
                    context.SaveChanges();
                    // Convert id list to array and serialize
                    var array = intList.ToArray();
                    var data = JsonSerializer.Serialize(array);
                    // Display new messages
                    // list.DisplayNewMessages();
                    packet.PacketData.Handler = Handler.Confirmation;
                    packet.PacketData.Data = data;
                    packet.PacketData.Response = null;
                    break;
            }

        }
        public static int AnalyzeStoreResponse(this Packet packet) {
            var param = packet.PacketData.Params;
            switch (param) {
                case "messages":
                    if (packet.PacketData.Response == "1") {
                        Console.WriteLine($"Message Was Sent successfully to the server...");
                        return 1;
                    } else {
                        Console.WriteLine($"Unable to send message to server...");
                        return 0;
                    }
            }

            return 0;
        }

        private static void DisplayNewMessages(this List<Message> msgList)
        {
            Console.WriteLine($"You have received {msgList.Count} new messages!");
            foreach (var msg in msgList)
            {
                Console.WriteLine($"Message ID {msg.Id}:\n{msg.Data}");
            }
        }
    }
}
