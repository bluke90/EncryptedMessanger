using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EncryptedMessanger.ClientNet.Handlers;
using EncryptedMessanger.ClientNet.Modules;

namespace EncryptedMessanger.ClientNet.Controllers
{
    public static class PacketController
    {
        public static void GeneratePacket(this Client client, Handler handler, string param, string data)
        {
            if (client.Packet != null) client.Packet = null;

            var packet = new Packet();
            packet.Id = RandomNumberGenerator.GetInt32(1000, 9999);
            packet.PacketData = new PacketData()
            {
                PacketId = packet.Id,
                ClientId = client.ClientId,
                Handler = handler,
                Params = param,
                Data = data
            };
            client.Packet = packet;
        }
        public static async Task GeneratePacketAsync(this Client client, Handler handler, string param, string data) {
            await Task.Yield();
            if (client.Packet != null) client.Packet = null;

            var packet = new Packet();
            packet.Id = RandomNumberGenerator.GetInt32(1000, 9999);
            packet.PacketData = new PacketData()
            {
                PacketId = packet.Id,
                ClientId = client.ClientId,
                Handler = handler,
                Params = param,
                Data = data
            };
            client.Packet = packet;
        }

        /*
        public static void TransmitPacket(this Client client)
        {
            var packetData = client.Packet.PacketData;
            var searializedData = JsonSerializer.Serialize(packetData);
            client.StartClient(searializedData);
        }
        */
        public static void DeserializePacketData(this Client client) {
            if (client.Packet.PacketData.Response == null || client.Packet.PacketData.Response.Length == 0) throw new ArgumentNullException();
            var packet = client.Packet;
            var resp = packet.PacketData.Response;
            packet.PacketData = JsonSerializer.Deserialize<PacketData>(resp);
        }

        public static int HandlePacketResponse(this Client client)
        {
            if (client.Packet.PacketData.Response == null || client.Packet.PacketData.Response.Length == 0) return 0;
            var packet = client.Packet;
            var resp = packet.PacketData.Response;
            packet.PacketData = JsonSerializer.Deserialize<PacketData>(resp);
            switch (packet.PacketData.Handler)
            {
                case Handler.Request:
                    packet.AnalyzeRequestResponse();
                    if (packet.PacketData.Handler == Handler.Confirmation)
                        client.TransmitPacket();
                    return 1;

                case Handler.Store:
                    return packet.AnalyzeStoreResponse();
            }

            return 0;
        }
    }
}
