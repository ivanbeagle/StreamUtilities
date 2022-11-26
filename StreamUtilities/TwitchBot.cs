using TwitchLib.Client;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;

using cfg = StreamUtilities.StreamUtilitiesSettings;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualBasic.Logging;

namespace StreamUtilities
{
    #region Support
    internal enum TwichBotEventKind
    {
        UserJoin,
        UserLeft,
        Message,

        Whisper,
        NewSub,
        GiftedSub,
        PrimePaidSub,
        Raid,
        ReSub,
        ExistingUsersDetected,
        CommunitySub
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
                msg = "[BOT] è uscito/a! :(";
            }

            else if (e.SourceEvent is OnUserJoinedArgs userjoin)
            {
                owner = userjoin.Username;
                msg = "[BOT] è entrato/a in live :D";
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

            else if (e.SourceEvent is OnExistingUsersDetectedArgs userdetect)
            {
                owner = userdetect.Channel;

                string join = String.Join(',', userdetect.Users);
                if (join.Length > 255)
                    join = join.Substring(0, 255) + "...";
                msg = join + $" ({userdetect.Users.Count})";
            }

            else if(e.SourceEvent is OnCommunitySubscriptionArgs community)
            {
                owner = community.GiftedSubscription.DisplayName;
                msg = community.GiftedSubscription.MsgParamMultiMonthGiftDuration;
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

                // added in 1.0.1
                _client.OnCommunitySubscription += Client_OnCommunitySubscription;
                _client.OnError += Client_OnError;
                _client.OnExistingUsersDetected += Client_OnExistingUsersDetected;

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
            Console.WriteLine($"[StreamUtilities] connesso a {e.AutoJoinChannel}.");
            Debug.WriteLine($"[StreamUtilities] connesso a {e.AutoJoinChannel}.");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine("[StreamUtilities] ha creato il BOT!");
            Debug.WriteLine("[StreamUtilities] ha creato il BOT!");

            _client.SendMessage(e.Channel, "[StreamUtilities] ha creato il BOT!");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            //if (e.ChatMessage.Message.Contains("badword"))
            //    _client.TimeoutUser(e.ChatMessage.Channel, e.ChatMessage.Username, TimeSpan.FromMinutes(30), "Bad word! 30 minute timeout!");

            OnTwitchEvent?.Invoke(this, new TwitchBotEvent(TwichBotEventKind.Message, e));
        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {

            OnTwitchEvent?.Invoke(this, new TwitchBotEvent(TwichBotEventKind.Whisper, e));
        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            OnTwitchEvent?.Invoke(this, new TwitchBotEvent(TwichBotEventKind.NewSub, e));
        }

        private void Client_OnUserJoined(object sender, OnUserJoinedArgs e)
        {
            if (StreamUtilitiesSettings.Default.TwitchIgnoreNames.Contains(e.Username))
                return;

            Console.WriteLine($"[BOT] {e.Username} è entrato/a in live!");
            Debug.WriteLine($"[BOT] {e.Username} è entrato/a in live!");

            _client.SendMessage(e.Channel, $"[BOT] Benvenuto/a @{e.Username}! :)");
            OnTwitchEvent?.Invoke(this, new TwitchBotEvent(TwichBotEventKind.UserJoin, e));

            Task.Run(async () =>
            {
                await Task.Delay(15000);
                _client.SendMessage(e.Channel, "[BOT] Reminder: digita !rules per le regole della chat e se ancora non lo hai fatto... seguimi! :)");
            });
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

        private void Client_OnExistingUsersDetected(object sender, OnExistingUsersDetectedArgs e)
        {
            OnTwitchEvent?.Invoke(this, new TwitchBotEvent(TwichBotEventKind.ExistingUsersDetected, e));
        }

        private void Client_OnError(object sender, TwitchLib.Communication.Events.OnErrorEventArgs e)
        {
            Debug.WriteLine(e.Exception?.Message);
            Console.WriteLine(e.Exception?.Message);

            Debug.WriteLine(e.Exception?.StackTrace);
            Console.WriteLine(e.Exception?.StackTrace);
        }

        private void Client_OnCommunitySubscription(object sender, OnCommunitySubscriptionArgs e)
        {
            OnTwitchEvent?.Invoke(this, new TwitchBotEvent(TwichBotEventKind.CommunitySub, e));
        }
        #endregion
    }
}
