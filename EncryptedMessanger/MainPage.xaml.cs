using System;
using System.Collections.Generic;
using System.Linq;
using EncryptedMessanger.Data;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Essentials;
using Microsoft.Maui.Graphics;
using Microsoft.EntityFrameworkCore;

namespace EncryptedMessanger
{
	public partial class MainPage : ContentPage
	{
		int count = 0;
        private AppManger _appManger;
		private List<EncryptedMessanger.Modules.Contact> Contacts;
		public MainPage(AppManger appManger) {
            _appManger = appManger;
            _appManger.ClientInstance.StartClientService();
			InitializeComponent();
			PopulateContacts();
			//headerTitle.Text = FileSystem.AppDataDirectory;
		}

		private void OnAddContactClicked(object sender, EventArgs e) {
			App.Current.MainPage = new AddContactPage(_appManger);
        }


		private void PopulateContacts() {
			ClientContext context = new ClientContext();
			Contacts = context.Contacts.ToList();
			foreach (var contact in Contacts) {
				var btn = GenerateContactButton(contact);
				contactStack.Add(btn);
            }
        }

		private Button GenerateContactButton(EncryptedMessanger.Modules.Contact contact) {
			Button button = new Button()
            {
				Text = contact.Name,
				BindingContext = contact.ContactId.ToString(),
				BorderColor = Colors.MidnightBlue,
				VerticalOptions = LayoutOptions.FillAndExpand
            };
			button.Clicked += OpenMsgPage;
			return button;
        }

        private void OpenMsgPage(object sender, EventArgs e) {
			string contactId = ((Button)sender).BindingContext as string;
			App.Current.MainPage = new MsgPage(_appManger, contactId);
		}
    }
}
