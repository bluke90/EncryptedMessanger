using EncryptedMessanger.ClientNet;
using EncryptedMessanger.Data;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;
using Application = Microsoft.Maui.Controls.Application;

namespace EncryptedMessanger
{
	public partial class App : Application
	{
		public Client Client { get; set; }
		public App()
		{
			InitializeComponent();

			MainPage = new MainPage();
		}

		private void InitializeClient() {
			Client = new Client(clientId: 2042);
			Client.StartClientService();
        }
	}
}
