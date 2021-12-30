using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EncryptedMessanger.ClientNet.Modules
{
    public class Packet
    {
        public int Id { get; set; }
        public const int BufferSize = 1024;
        public byte[] Buffer = new byte[BufferSize];
        public StringBuilder StringBuilder = new StringBuilder();
        public Socket Socket = null;
        public PacketData? PacketData { get; set; }
    }

    public class PacketData
    {
        public int Id { get; set; }
        public int PacketId { get; set; }
        public int ClientId { get; set; }
        public Handler Handler { get; set; }
        public string? Params { get; set; }
        public string? Data { get; set; }
        public string? Response { get; set; }
    }

    public enum Handler
    {
        Confirmation,
        Store,
        Request,
        Response
    }

    public enum Status
    {
        Failed,
        Success,
        Waiting
    }
}
