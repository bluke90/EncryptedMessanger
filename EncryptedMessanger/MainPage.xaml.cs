using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Essentials;

namespace EncryptedMessanger
{
	public partial class MainPage : ContentPage
	{
		int count = 0;
        private AppManger _appManger;
		public MainPage(AppManger appManger) {
            _appManger = appManger;
            _appManger.ClientInstance.StartClientService();
			InitializeComponent();


		}

		private void OnAddContactClicked(object sender, EventArgs e) {
			App.Current.MainPage = new AddContactPage(_appManger);
        }

		private void OnCounterClicked(object sender, EventArgs e)
		{
			count++;
			// CounterLabel.Text = $"Current count: {count}";
            App.Current.MainPage = new MsgPage(_appManger);
            // SemanticScreenReader.Announce(CounterLabel.Text);
        }

		private static Button GenerateContactButton(string Text, string contactNickname, string contactId) {
			Button button = new Button()
            {
				Text = ContactNickname
            };
        }

	}
}
