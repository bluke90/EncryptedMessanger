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
using System.Threading;
using System.Collections.ObjectModel;

namespace EncryptedMessanger
{

    public partial class MsgPage : ContentPage {
        private AppManger _appManger;
        private readonly ClientContext _context;
        private ObservableCollection<Message> _messages;
        private Queue<Message> _messagesQueue;
        private Thread _thread;
        public MsgPage(AppManger appManger) {
            _appManger = appManger;
            _context = _appManger.ClientInstance.ClientContext;
            _messages = new ObservableCollection<Message>();
            _messagesQueue = new Queue<Message>();
            InitializeComponent();
            _messages.CollectionChanged += PopulateQueuedMessage;
            _thread = new Thread(MessageRefreash);
            _thread.Start();
        }


        private void MessageRefreash() {
            while (true) {
                foreach (var message in _context.Messages.ToList()) {
                    if (!_messages.Contains(message)) {
                        _messagesQueue.Enqueue(message);
                        _messages.Add(message);
                    }
                }
                Thread.Sleep(1000);
            }
        }

        private void PopulateQueuedMessage(object sender, EventArgs args) {
            for (int i = 0; i <= _messagesQueue.Count; i++) {

                var msg = _messagesQueue.Dequeue();
                App.Current.Dispatcher.BeginInvokeOnMainThread(() => PopulateNewMessage(msg));
            }
        }

        private void PopulateNewMessage(Message msg) {
            var lbl = new Label
            {
                Text = $"From: {msg.ClientId} \n {msg.Data}",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                TextColor = Colors.White,
                BackgroundColor = Color.FromArgb("121212")
            };
            msgs.Add(lbl);
        }

        private void OnSendMsg(object sender, EventArgs e) {
            var _msg = msg.Text;

            Message newMsg = new Message()
            {
                Data = _msg,
                ClientId = _appManger.ClientInstance.ClientId,
                RecipientId = 2042
            };

            _appManger.ClientInstance.SendMessageToServer(newMsg);
            _context.Messages.Add(newMsg);
            _context.SaveChanges();
        }



    }
}