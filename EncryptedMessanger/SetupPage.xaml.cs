using System;
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
        }

        public void OnGenerateContactId(object sender, EventArgs args) {
            ContinueBtn.Clicked += OnSaveDetails;

            _appManager.SettingsHandler.Settings.NickName = NicknameEntry.Text;

        }

        public void OnSaveDetails(object sender, EventArgs args) { 
            
        }
    }
}