using System;
using System.Text;
using System.Text.Json;
using EncryptedMessanger.ClientNet;
using EncryptedMessanger.ClientNet.Controllers;
using EncryptedMessanger.Data;
using EncryptedMessanger.ClientNet.Modules;
using EncryptedMessanger.ClientNet.Handlers;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Essentials;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Graphics;

namespace EncryptedMessanger
{

    public partial class MsgPage : ContentPage
    {
        private readonly Client _client;
        private readonly ClientContext _context;
        public MsgPage() {
            InitializeComponent();
            _client = AppManger.ClientInstance;
            PopulateMessages();
        }

        private async void PopulateMessages() {
            var msgList = await _context.Messages.ToListAsync();

            foreach (var msg in msgList) {
                var lbl = new Label
                {
                    Text = $"From: {msg.ClientId} \n {msg.Data}",
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    TextColor = Colors.White,
                    BackgroundColor = Colors.DarkGrey,
                };
                msgs.Add(lbl);
            }


        }


        private void OnSendMsg(object sender, EventArgs e) {
            var _msg = msg.Text;

            Message newMsg = new Message()
            {
                Data = _msg,
                ClientId = _client.ClientId,
                RecipientId = 2000
            };

            _client.SendMessageToServer(newMsg);
        }



    }
}