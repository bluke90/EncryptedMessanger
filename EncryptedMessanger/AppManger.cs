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
        public Client ClientInstance { get; } = new Client(2042, host: "127.0.0.1");

        public AppManger() {
            
        }
    }
}
