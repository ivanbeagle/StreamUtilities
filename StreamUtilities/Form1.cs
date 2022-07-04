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
        private SoundPlayer _player;
        private Color _btnCaptureModeForeColor;
        private CapturedWindow _currentWindow;
        private List<CapturedWindow> _trackedWindows;
        private TwitchBot _bot;
        #endregion

        #region Ctors
        public Form1()
        {
            InitializeComponent();
            _btnCaptureModeForeColor = btnCaptureMode.ForeColor;

            PutUserSettings();

            string ver = GetVersion();
            this.Text += " " + ver;

            _trackedWindows = new List<CapturedWindow>();
            _bot = new TwitchBot();
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

        private void ActivateAllWindows()
        {
            _trackedWindows.ForEach(o =>
            {
            });
        }

        private void RestoreAllWindows()
        {
            _trackedWindows.ForEach(o =>
            {
            });
        }

        #region Events
        private void Form1_Load(object sender, EventArgs e)
        {
            _player = new SoundPlayer(Resources.notify);

            // first rec
            var instance = FakeWindow.Singleton;

            instance.HotKeyPressed += Instance_MagicHotKeyPressed;
            instance.CustomMessageReceived += Instance_CustomMessageReceived;
            instance.ShellWindowActivated += Instance_ShellWindowActivated;
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

            if (_trackedWindows.Contains(_currentWindow) || _trackedWindows.Any(o => o.Handle == _currentWindow.Handle))
            {
                MessageBox.Show("This window is already tracked!", "Can't add the window", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
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
            PlaySound();

            WinDecorator.Singleton.MoveOnWindow(e);
            WinDecorator.Singleton.HideAfter(1300);
        }

        private void Instance_CustomMessageReceived(object sender, Message e)
        {
            PlaySound();
        }

        private void Instance_MagicHotKeyPressed(object sender, Message e)
        {
            PlaySound();
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
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
                settings.PutUIToUserConfig();

                bool needUpdateHotkey = false;
                if (UserConfig.Singleton.Hotkey != cfg.Default.Hotkey)
                    needUpdateHotkey = true;

                UserConfig.Singleton.Save();

                if (needUpdateHotkey)
                    if(!FakeWindow.Singleton.UpdateHotkey(UserConfig.Singleton.Hotkey))
                    {
                        MessageBox.Show("Failed to apply new hotkey config, restart the application to see it work!", 
                            "Failed register hotkey",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                // after save new config is available
                PutUserSettings();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            cfg.Default.WinOpacity = trackBar1.Value;
            cfg.Default.Save();

            _bot.Disconnect();
        }
        #endregion

        private void btnConnectTwitch_Click(object sender, EventArgs e)
        {
            _bot.Connect();
        }
    }
}
