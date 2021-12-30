using EncryptedMessanger.ClientNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptedMessanger
{
    public class AppManger
    {
        private static readonly Client Client = new Client(2042);
        public static Client ClientInstance { get { return Client; } }

        public AppManger() {
            Client.StartClientService();
        }
    }
}
