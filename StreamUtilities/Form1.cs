using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using StreamUtilities.Properties;
using System.Media;

using user32 = Vanara.PInvoke.User32;
using hwnd = Vanara.PInvoke.HWND;
using cfg = StreamUtilities.StreamUtilitiesSettings;
using Vanara.PInvoke;

namespace StreamUtilities
{
    public partial class Form1 : Form
    {
        #region Fields
        private Color _btnCaptureModeForeColor;
        private Color _btnTwitchForeColor;

        private SoundPlayer _player;
        private CapturedWindow _currentWindow;
        private List<CapturedWindow> _trackedWindows;
        private TwitchBot _twitch;
        private Task _twitchTask;
        private BlinkNotification _notifier;
        private DateTime _lastTwitchNotification = DateTime.Now;
        private bool _switchToggle = false;
        #endregion

        #region Ctors
        public Form1()
        {
            InitializeComponent();
            _btnCaptureModeForeColor = btnCaptureMode.ForeColor;
            _btnTwitchForeColor = btnConnectTwitch.ForeColor;

            PutUserSettings();

            string ver = GetVersion();
            this.Text += " " + ver;

            _trackedWindows = new List<CapturedWindow>();

            // inizializza twitch e le sue notifiche
            _twitch = new TwitchBot();
            _twitch.OnTwitchEvent += _twitch_OnTwitchEvent;
            
            // notifiche visuali
            _notifier = new BlinkNotification();
            _notifier.Show();
        }

        #endregion

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(value);
        }

        private string GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        /// <summary>
        /// Trasferisce i settaggi utente nell'interfaccia
        /// </summary>
        private void PutUserSettings()
        {
            trackBar1.Value = cfg.Default.WinOpacity;

            opacity.Text = trackBar1.Value.ToString() + "%";

            label2.Text = "CTRL + SHIFT + {$HOTKEY} = toggle top/opacity the selected windows";
            label2.Text = label2.Text.Replace("{$HOTKEY}", cfg.Default.Hotkey.ToString());
        }

        private void PlaySound()
        {
            _player.Play();
        }

        private void ActivateAllWindows(bool value)
        {
            _trackedWindows.ForEach(o =>
            {
                o.Ghostize(value);
            });
        }

        #region Events        
        private void Form1_Activated(object sender, EventArgs e)
        {
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            _player = new SoundPlayer(Resources.notify);

            // registra gli eventi della FakeWindow, una finestra nascosta che è in grado di captare gli eventi in Windows
            var fkWin = FakeWindow.Singleton;

            fkWin.HotKeyPressed += Instance_HotKeyPressed;
            fkWin.CustomMessageReceived += Instance_CustomMessageReceived;
            fkWin.ShellWindowActivated += Instance_ShellWindowActivated;
        }

        private void _twitch_OnTwitchEvent(object sender, TwitchBotEvent e)
        {
            bool goodEvent = (int)e.Kind > (int)TwichBotEventKind.Message;
            bool playSound = ((DateTime.Now - _lastTwitchNotification).TotalSeconds > 120) && goodEvent;

            // X min senza notifiche? alla prima notifica utile fa un suono
            if (playSound)
            {
                PlaySound();
            }

            // ricava evento
            e.GetContent(out string owner, out string msg);

            // necessario farlo come task!
            Task.Run(() =>
            {
                _notifier.AddNotify(e.Kind == TwichBotEventKind.Message, e.Kind.ToString(), owner, msg);
            });

            // imposta l'ultima notifica
            _lastTwitchNotification = DateTime.Now;
        }

        private async void btnConnectTwitch_Click(object sender, EventArgs e)
        {
            btnConnectTwitch.Enabled = false;

            // il task di twitch deve essere riavviato
            if(_twitchTask!=null)
            {
                _twitch.Disconnect();
                _twitchTask = null;

                btnConnectTwitch.BackColor = Color.Navy;
                btnConnectTwitch.ForeColor = _btnTwitchForeColor;
                btnConnectTwitch.Text = "Reconnect Twitch!";
                btnConnectTwitch.Enabled = true;
                return;
            }

            btnConnectTwitch.BackColor = Color.DarkBlue;
            btnConnectTwitch.ForeColor = Color.White;
            btnConnectTwitch.Text = "Connecting...";

            _twitchTask = await _twitch.Connect();

            // verifica la connessione a pool
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(1500);
                if (_twitch.IsSuccessfullyConnected)
                {
                    // disattiva la modalità di cattura
                    if (FakeWindow.Singleton.AllowShellHookCapture)
                    {
                        btnCaptureMode_Click(null, null);

                        // mi sembrava carino mostrare una notifica :)
                        notifyIcon1.Visible = true;
                        notifyIcon1.BalloonTipTitle = "StreamUtilities is connected to Twitch! :)";
                        notifyIcon1.BalloonTipText = "Capture mode is turned OFF, you can enable it again!";
                        notifyIcon1.ShowBalloonTip(3000);
                    }

                    btnConnectTwitch.BackColor = Color.Red;
                    btnConnectTwitch.ForeColor = _btnTwitchForeColor;
                    btnConnectTwitch.Text = "Disconnect Twitch!";
                    btnConnectTwitch.Enabled = true;
                    return;
                }
            }

            // dopo questo timeout se non riesce a connettersi mostra un errore
            if (!_twitch.IsSuccessfullyConnected)
            {
                MessageBox.Show("Check Internet and/or Twitch settings and retry!",
                    "Unable to connect to Twitch", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                if(_twitch.IsConnected && _twitch.HasFailed)
                {
                    Debug.WriteLine("Disconnecting Twitch!");
                    _twitch.Disconnect();
                    _twitchTask = null;
                }

                btnConnectTwitch.BackColor = Color.Black;
                btnConnectTwitch.ForeColor = Color.Red;
                btnConnectTwitch.Text = "Try Again";
                btnConnectTwitch.Enabled = true;
            }
        }

        private async void btnOpacity_Click(object sender, EventArgs e)
        {
            double opacity = Opacity;

            btnOpacity.Text = "Wait!";
            btnOpacity.Enabled = false;
            Opacity = (double)trackBar1.Value / (double)100;

            await Task.Delay(1500);
            btnOpacity.Text = "Test opacity";
            btnOpacity.Enabled = true;
            Opacity = opacity;
        }

        private void btnCaptureMode_Click(object sender, EventArgs e)
        {
            FakeWindow.Singleton.AllowShellHookCapture = !FakeWindow.Singleton.AllowShellHookCapture;

            if (FakeWindow.Singleton.AllowShellHookCapture)
            {
                btnCaptureMode.Text = "Switch capture mode OFF";
                btnCaptureMode.ForeColor = _btnCaptureModeForeColor;
            }
            else
            {
                btnCaptureMode.Text = "Switch capture mode ON";
                btnCaptureMode.ForeColor = Color.Red;
            }
        }

        private void btnCaptureWin_Click(object sender, EventArgs e)
        {
            if (_currentWindow == null)
                return;

            // se una finestra è già tracciata non deve farla mettere in lista, anzi...
            if (_trackedWindows.Contains(_currentWindow) || _trackedWindows.Any(o => o.Handle == _currentWindow.Handle))
            {
                MessageBox.Show("This window is already tracked!", "Can't add the window", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                // la finestra è pronta per il tracciamento (nota: non viene rilevato se la finestra viene chiusa)
                _trackedWindows.Add(_currentWindow);

                var item = new ListViewItem(new[] { _currentWindow.Handle.ToString(), _currentWindow.Caption });
                item.Tag = _currentWindow;

                listView1.Items.Add(item);
            }
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            Global.Opacity = trackBar1.Value;
        }

        private void btnRemoveWin_Click(object sender, EventArgs e)
        {
            var selected = listView1.SelectedItems.Count > 0 ? listView1.SelectedItems[0] : null;

            if (selected == null)
                return;

            _trackedWindows.Remove(selected.Tag as CapturedWindow);
            listView1.Items.Remove(selected);
        }

        private void Instance_ShellWindowActivated(object sender, hwnd e)
        {
            _currentWindow = new CapturedWindow(e);
            btnCaptureWin.Text = "Capture: " + _currentWindow.Caption;

            // da un feedback quando si seleziona una finestra in Windows
            if(_trackedWindows.Any(o => o.Handle == (IntPtr)e))
            {
                WinDecorator.Singleton.MoveOnWindow(e, Brushes.Blue);
            }
            else
            {
                if (!_twitch.IsSuccessfullyConnected)
                {
                    PlaySound();

                    // mostra un decoratore
                    WinDecorator.Singleton.MoveOnWindow(e);
                }
            }

            WinDecorator.Singleton.HideAfter(1300);
        }

        private void Instance_CustomMessageReceived(object sender, Message e)
        {
            // non usato
            //PlaySound();
        }

        private void Instance_HotKeyPressed(object sender, Message e)
        {
            // attiva e disattiva finestra ghost
            _switchToggle = !_switchToggle;

            //PlaySound();
            ActivateAllWindows(_switchToggle);
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            // ripristina da tray mode

            notifyIcon1.Visible = false;
            Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            // setta tray mode

            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();

                notifyIcon1.Visible = true;
                notifyIcon1.BalloonTipTitle = "StreamUtilities works in background now!";
                notifyIcon1.BalloonTipText = "Double click on the icon to restore";
                notifyIcon1.ShowBalloonTip(3000);
            }
        }

        private void notifyIcon1_BalloonTipClicked(object sender, EventArgs e)
        {
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            opacity.Text = trackBar1.Value.ToString() + "%";
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            new AboutForm().ShowDialog();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            var settings = new SettingsForm();
            if(settings.ShowDialog() == DialogResult.OK)
            {
                // trasferisce le impostazioni nel config
                settings.PutUIToUserConfig();

                // calcola i flags
                bool needUpdateHotkey = false;
                if (UserConfig.Singleton.Hotkey != cfg.Default.Hotkey)
                    needUpdateHotkey = true;

                bool needUpdateTwitch = false;
                bool oneOfTheseCondictions = UserConfig.Singleton.TwitchUsername != cfg.Default.TwitchUsername
                    || UserConfig.Singleton.TwitchAccessToken != cfg.Default.TwitchAccessToken
                    || UserConfig.Singleton.TwitchChannel != cfg.Default.TwitchChannel;

                // salva ed applica la config
                UserConfig.Singleton.Save();

                // twitch può richiedere un riavvio se le impostazioni cambiano, ma ad ogni modo ho voluto farlo fare manualmente!
                if (oneOfTheseCondictions && (_twitch.IsSuccessfullyConnected))
                {
                    MessageBox.Show("Twitch is currently running, disconnect it manually, then reconnect again to apply settings!",
                            "Twitch is running",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // l'hotkey richiede un aggiornamento... e basta!
                if (needUpdateHotkey)
                {
                    if (!FakeWindow.Singleton.UpdateHotkey(UserConfig.Singleton.Hotkey))
                    {
                        MessageBox.Show("Failed to apply new hotkey config, restart the application to see it work!",
                            "Failed register hotkey",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                // mostra config
                PutUserSettings();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // salva nel config le impostazioni scelte
            cfg.Default.WinCaptureEnabled = FakeWindow.Singleton.AllowShellHookCapture;
            cfg.Default.WinOpacity = trackBar1.Value;
            cfg.Default.Save();

            // ripristina le finestre
            ActivateAllWindows(false);

            // disconnette twitch
            _twitch.Disconnect();
        }
        #endregion
    }
}
