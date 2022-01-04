using System;
using EncryptedMessanger.ClientNet;
using EncryptedMessanger.Data;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Essentials;


namespace EncryptedMessanger
{
    public partial class AddContactPage : ContentPage
    {
        private AppManger _appManager;
        public AddContactPage(AppManger appManger) {
            _appManager = appManger;
            InitializeComponent();
        }

        private void OnAddContact(object sender, EventArgs args) {
            var nickname = ContactNickname.Text;
            var contactId = Convert.ToInt32(ContactId.Text);

            var Contact = new EncryptedMessanger.Modules.Contact() { ContactId = contactId, Name = nickname };
            ClientContext context = new ClientContext();
            context.Contacts.Add(Contact);
            context.SaveChanges();
            App.Current.MainPage = new MainPage(_appManager);
        }
    }
}