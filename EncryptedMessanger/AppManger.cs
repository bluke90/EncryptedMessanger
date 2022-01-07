using EncryptedMessanger.ClientNet;
using EncryptedMessanger.Handlers;
using EncryptedMessanger.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EncryptedMessanger
{
    public class AppManger
    {
        public Client ClientInstance { get; }
        public SettingsHandler SettingsHandler { get; private set; }

        public AppManger() {
            SettingsHandler = new SettingsHandler();
            if (SettingsHandler.Settings.ContactId > 1) {
                ClientInstance = new Client(clientId: SettingsHandler.Settings.ContactId.Value);
                ClientInstance.StartClientService();
            } else { ClientInstance = null; }

        }
    }
}
