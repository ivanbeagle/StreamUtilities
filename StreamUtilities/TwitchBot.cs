using TwitchLib.Client;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;

using cfg = StreamUtilities.StreamUtilitiesSettings;
using System.Diagnostics;

namespace StreamUtilities
{
    internal enum TwichBotEventKind
    {
        Message,
        Whisper,
        NewSub,
        GiftedSub,
        PrimePaidSub,
        Raid,
        ReSub,
        UserJoin
    }

    internal class TwitchBotEvent : EventArgs
    {
        public TwichBotEventKind Kind { get; set; }

        public EventArgs SourceEvent { get; set; }

        public TwitchBotEvent(TwichBotEventKind kind, EventArgs sourceEvent)
        {
            Kind = kind;
            SourceEvent = sourceEvent;
        }
    }

    internal class TwitchBot : IDisposable
    {
        #region Fields
        TwitchClient _client;
        WebSocketClient _wsClient;
        #endregion

        #region Events
        public EventHandler<TwitchBotEvent> OnTwitchEvent;
        #endregion

        public async Task<Task> Connect()
        {
            return Task.Run(() =>
            {
                ConnectionCredentials credentials = new ConnectionCredentials(cfg.Default.TwitchUsername, cfg.Default.TwitchAccessToken);

                var clientOptions = new ClientOptions
                {
                    MessagesAllowedInPeriod = 750,
                    ThrottlingPeriod = TimeSpan.FromSeconds(30),
                };

                _wsClient = new WebSocketClient(clientOptions);

                _client = new TwitchClient(_wsClient);
                _client.Initialize(credentials, cfg.Default.TwitchChannel);

                _client.OnLog += Client_OnLog;
                _client.OnJoinedChannel += Client_OnJoinedChannel;
                _client.OnMessageReceived += Client_OnMessageReceived;
                _client.OnWhisperReceived += Client_OnWhisperReceived;
                _client.OnNewSubscriber += Client_OnNewSubscriber;
                _client.OnConnected += Client_OnConnected;
                _client.OnGiftedSubscription += Client_OnGiftedSubscription;
                _client.OnHostingStarted += Client_OnHostingStarted;
                _client.OnPrimePaidSubscriber += Client_OnPrimePaidSubscriber;
                _client.OnRaidNotification += Client_OnRaidNotification;
                _client.OnReSubscriber += Client_OnReSubscriber;
                _client.OnUserJoined += Client_OnUserJoined;

                _client.Connect();

                //while(true) { };
            });
        }

        public void Disconnect()
        {
            Dispose();
        }

        public void Dispose()
        {
            _client.Disconnect();
            _wsClient.Dispose();
        }

        #region Events

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            Debug.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Debug.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Debug.WriteLine("Hey guys! I am a bot connected via TwitchLib!");
            _client.SendMessage(e.Channel, "Hey guys! I am a bot connected via TwitchLib!");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.Message.Contains("badword"))
                _client.TimeoutUser(e.ChatMessage.Channel, e.ChatMessage.Username, TimeSpan.FromMinutes(30), "Bad word! 30 minute timeout!");

            OnTwitchEvent?.Invoke(this, new TwitchBotEvent(TwichBotEventKind.Message, e));
        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            if (e.WhisperMessage.Username == "my_friend")
                _client.SendWhisper(e.WhisperMessage.Username, "Hey! Whispers are so cool!!");

            OnTwitchEvent?.Invoke(this, new TwitchBotEvent(TwichBotEventKind.Whisper, e));
        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
                _client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points! So kind of you to use your Twitch Prime on this channel!");
            else
                _client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points!");

            OnTwitchEvent?.Invoke(this, new TwitchBotEvent(TwichBotEventKind.NewSub, e));
        }

        private void Client_OnUserJoined(object sender, OnUserJoinedArgs e)
        {
            Debug.WriteLine($"User joined {e.Username}");

            OnTwitchEvent?.Invoke(this, new TwitchBotEvent(TwichBotEventKind.UserJoin, e));
        }

        private void Client_OnReSubscriber(object sender, OnReSubscriberArgs e)
        {
            OnTwitchEvent?.Invoke(this, new TwitchBotEvent(TwichBotEventKind.ReSub, e));
        }

        private void Client_OnRaidNotification(object sender, OnRaidNotificationArgs e)
        {
            OnTwitchEvent?.Invoke(this, new TwitchBotEvent(TwichBotEventKind.Raid, e));
        }

        private void Client_OnPrimePaidSubscriber(object sender, OnPrimePaidSubscriberArgs e)
        {
            OnTwitchEvent?.Invoke(this, new TwitchBotEvent(TwichBotEventKind.PrimePaidSub, e));
        }

        private void Client_OnHostingStarted(object sender, OnHostingStartedArgs e)
        {
        }

        private void Client_OnGiftedSubscription(object sender, OnGiftedSubscriptionArgs e)
        {
            OnTwitchEvent?.Invoke(this, new TwitchBotEvent(TwichBotEventKind.GiftedSub, e));
        }
        #endregion
    }
}
