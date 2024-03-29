﻿using System;
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
        private int _contactId;
        public MsgPage(AppManger appManger, string contact) {
            _contactId = Convert.ToInt32(contact);
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
            try {
                while (true) {
                    foreach (var message in _context.Messages.Where(m => m.ClientId == _contactId || m.RecipientId == _contactId).ToList()) {
                        if (!_messages.Contains(message)) {
                            _messagesQueue.Enqueue(message);
                            _messages.Add(message);
                        }
                    }
                    Thread.Sleep(1000);
                }
            } catch (InvalidOperationException ex) {
                Thread.Sleep(1000);
                MessageRefreash();
            }
        }

        private void PopulateQueuedMessage(object sender, EventArgs args) {
            for (int i = 0; i <= _messagesQueue.Count; i++) {

                var msg = _messagesQueue.Dequeue();
                App.Current.Dispatcher.Dispatch(() => PopulateNewMessage(msg));
            }
        }

        private void PopulateNewMessage(Message msg) {
            LayoutOptions layoutOptions = LayoutOptions.FillAndExpand;
            string msgData;
            if (msg.ClientId == _appManger.SettingsHandler.Settings.ContactId) {
                msgData = $"To: {msg.RecipientId} \n {msg.Data}";
            } else { 
                msgData = $"From: {msg.ClientId} \n {msg.Data}";
            }
            var blnkLbl = new Label() { HeightRequest = 50 };
            var lbl = new Label
            {
                Text = msgData,
                HorizontalOptions = layoutOptions,
                TextColor = Colors.White,
                BackgroundColor = Color.FromArgb("121212"),
                Padding = 15
            };
            if (msg.ClientId == _appManger.SettingsHandler.Settings.ContactId) {
                toMsgs.Add(lbl);
                frmMsgs.Add(blnkLbl);
            } else {
                frmMsgs.Add(lbl);
                toMsgs.Add(blnkLbl);
            }
        }

        private void OnSendMsg(object sender, EventArgs e) {
            var _msg = msg.Text;

            Message newMsg = new Message()
            {
                Data = _msg,
                ClientId = _appManger.ClientInstance.ClientId,
                RecipientId = _contactId
            };

            _appManger.ClientInstance.SendMessageToServer(newMsg);
            _context.Messages.Add(newMsg);
            _context.SaveChanges();
        }

        private void OnGoBack(object sender, EventArgs e) {
            App.Current.MainPage = new MainPage(_appManger);
        }

    }
}