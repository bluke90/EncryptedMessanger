using System;
using EncryptedMessanger.ClientNet;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Essentials;


namespace EncryptedMessanger
{
    public partial class SetupPage : ContentPage
    {
        private AppManger _appManager;
        public SetupPage(AppManger appManger) {
            _appManager = appManger;
            InitializeComponent();
            ContinueBtn.Clicked += OnGenerateContactId;
        }

        public async void OnGenerateContactId(object sender, EventArgs args) {
            // Reqeust new ContactId
            var contactId = await GenericClient.RequestContactId();
            // Change generate id button to continue button
            ContinueBtn.Clicked -= OnGenerateContactId;
            ContinueBtn.Clicked += OnSaveDetails;
            ContinueBtn.Text = "Continue";
            // Add New Contact ID to Contact ID Field
            ContactIdEntry.Text = contactId.ToString();
            // Add nickName and ContactId to settings
            _appManager.SettingsHandler.Settings.NickName = NicknameEntry.Text;
            _appManager.SettingsHandler.Settings.ContactId = contactId;
            _appManager.SettingsHandler.SaveChanges();
            return;
        }

        public void OnSaveDetails(object sender, EventArgs args) {
            App.Current.MainPage = new MainPage(_appManager);
        }
    }
}