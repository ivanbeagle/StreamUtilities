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
    #region Support
    internal enum TwichBotEventKind
    {
        Message,
        Whisper,
        NewSub,
        GiftedSub,
        PrimePaidSub,
        Raid,
        ReSub,
        UserJoin,
        UserLeft
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

    internal static class TwitchBotEventExtensions
    {
        public static void GetContent(this TwitchBotEvent e, out string owner, out string msg)
        {
            owner = null;
            msg = null;

            if(e.SourceEvent is OnMessageReceivedArgs message)
            {
                owner = message.ChatMessage.Username;
                msg = message.ChatMessage.Message;
                return;
            }

            else if(e.SourceEvent is OnWhisperReceivedArgs whisper)
            {
                owner = whisper.WhisperMessage.Username;
                msg = whisper.WhisperMessage.Message;
            }

            else if(e.SourceEvent is OnNewSubscriberArgs newsub)
            {
                owner = newsub.Subscriber.DisplayName;
                msg = newsub.Subscriber.ResubMessage + " " + newsub.Subscriber.SubscriptionPlanName;
            }

            else if (e.SourceEvent is OnReSubscriberArgs resub)
            {
                owner = resub.ReSubscriber.DisplayName;
                msg = resub.ReSubscriber.ResubMessage + " " + resub.ReSubscriber.SubscriptionPlanName;
            }

            else if (e.SourceEvent is OnUserLeftArgs userleft)
            {
                owner = userleft.Username;
                msg = "[BOT] has left! :(";
            }

            else if (e.SourceEvent is OnUserJoinedArgs userjoin)
            {
                owner = userjoin.Username;
                msg = "[BOT] has joined!!! :)";
            }

            else if (e.SourceEvent is OnRaidNotificationArgs raid)
            {
                owner = raid.RaidNotification.DisplayName;
                msg = raid.RaidNotification.MsgParamDisplayName;
            }

            else if (e.SourceEvent is OnPrimePaidSubscriberArgs prime)
            {
                owner = prime.PrimePaidSubscriber.DisplayName;
                msg = prime.PrimePaidSubscriber.ResubMessage + " " + prime.PrimePaidSubscriber.SubscriptionPlanName;
            }

            else if (e.SourceEvent is OnGiftedSubscriptionArgs gift)
            {
                owner = gift.GiftedSubscription.DisplayName;
                msg = gift.GiftedSubscription.MsgParamRecipientDisplayName + " " + gift.GiftedSubscription.MsgParamSubPlanName;
            }
        }
    }
    #endregion

    internal sealed class TwitchBot : IDisposable
    {
        #region Fields
        private string _connectionErrorMessage = null;
        private TwitchClient _client;
        private WebSocketClient _websockClient;
        #endregion

        #region Events
        public event EventHandler<TwitchBotEvent> OnTwitchEvent;
        #endregion

        #region Properties
        public bool IsConnected => _client != null ? _client.IsConnected : false;

        public bool HasFailed => _connectionErrorMessage != null;

        public bool IsSuccessfullyConnected => IsConnected && !HasFailed;

        public string ConnectionErrorMessage => _connectionErrorMessage;
        #endregion

        public async Task<Task> Connect()
        {
            _connectionErrorMessage = null;

            return Task.Run(() =>
            {
                ConnectionCredentials credentials = new ConnectionCredentials(cfg.Default.TwitchUsername, cfg.Default.TwitchAccessToken);

                var clientOptions = new ClientOptions
                {
                    MessagesAllowedInPeriod = 750,
                    ThrottlingPeriod = TimeSpan.FromSeconds(30),
                };

                _websockClient = new WebSocketClient(clientOptions);

                _client = new TwitchClient(_websockClient);
                _client.Initialize(credentials, cfg.Default.TwitchChannel);

                _client.OnConnectionError += Client_OnConnectionError;
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
                _client.OnUserLeft += Client_OnUserLeft;

                _client.Connect();
            });
        }

        public void Disconnect()
        {
            Dispose();
            _connectionErrorMessage = null;
        }

        public void Dispose()
        {
            if (_client != null && _client.IsConnected)
            {
                _client.Disconnect();
                _websockClient.Dispose();
                
                _client = null;
                _websockClient = null;
            }
        }

        #region Events
        private void Client_OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            _connectionErrorMessage = e.Error.Message;
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            string log = $"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}";
            // raw way to check auth fails
            if (e.Data.ToLower().Contains("authentication failed"))
            {
                _connectionErrorMessage = log;
            }

            Debug.WriteLine(log);
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Debug.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Debug.WriteLine("StreamUtilities creates the BOT connected via TwitchLib! :)");
            _client.SendMessage(e.Channel, "StreamUtilities creates the BOT connected via TwitchLib! :)");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            //if (e.ChatMessage.Message.Contains("badword"))
            //    _client.TimeoutUser(e.ChatMessage.Channel, e.ChatMessage.Username, TimeSpan.FromMinutes(30), "Bad word! 30 minute timeout!");

            OnTwitchEvent?.Invoke(this, new TwitchBotEvent(TwichBotEventKind.Message, e));
        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            //if (e.WhisperMessage.Username == "my_friend")
            //    _client.SendWhisper(e.WhisperMessage.Username, "Hey! Whispers are so cool!!");

            OnTwitchEvent?.Invoke(this, new TwitchBotEvent(TwichBotEventKind.Whisper, e));
        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            //if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
            //    _client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points! So kind of you to use your Twitch Prime on this channel!");
            //else
            //    _client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points!");

            OnTwitchEvent?.Invoke(this, new TwitchBotEvent(TwichBotEventKind.NewSub, e));
        }

        private void Client_OnUserJoined(object sender, OnUserJoinedArgs e)
        {
            Debug.WriteLine($"User joined: {e.Username}");

            _client.SendMessage(e.Channel, $"Welcome {e.Username}! :)");
            OnTwitchEvent?.Invoke(this, new TwitchBotEvent(TwichBotEventKind.UserJoin, e));
        }

        private void Client_OnUserLeft(object sender, OnUserLeftArgs e)
        {
            OnTwitchEvent?.Invoke(this, new TwitchBotEvent(TwichBotEventKind.UserLeft, e));
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
