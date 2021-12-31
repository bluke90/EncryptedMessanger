using System;
using System.Collections.Generic;
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

    public partial class MsgPage : ContentPage {
        private AppManger _appManger;
        private readonly ClientContext _context;
        public MsgPage(AppManger appManger) {
            _appManger = appManger;
            _context = _appManger.ClientInstance.ClientContext;
            InitializeComponent();
            PopulateMessages();
        }
        private async void PopulateMessages(Message newMsg = null) {
            List<Message> msgList;
            if (newMsg == null)
            {
                msgList = await _context.Messages.ToListAsync();
            } else {
                msgList = new List<Message>();
                msgList.Add(newMsg);
            }

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
                ClientId = _appManger.ClientInstance.ClientId,
                RecipientId = 2042
            };

            PopulateMessages(newMsg);
            _appManger.ClientInstance.SendMessageToServer(newMsg);
            _context.Messages.Add(newMsg);
            _context.SaveChanges();
        }



    }
}