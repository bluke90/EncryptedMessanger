﻿using EncryptedMessanger.ClientNet;
using EncryptedMessanger.Data;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.PlatformConfiguration.WindowsSpecific;
using Application = Microsoft.Maui.Controls.Application;

namespace EncryptedMessanger
{
	public partial class App : Application {
        
        private AppManger _appManger;
        public App() {
            _appManger = new AppManger();
			
            InitializeComponent();

            MainPage = new MainPage(_appManger);
        }
    }
}
