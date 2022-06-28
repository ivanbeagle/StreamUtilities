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


namespace StreamUtilities
{
    public partial class Form1 : Form
    {
        #region Fields
        private SoundPlayer _player;
        #endregion

        #region Ctors
        public Form1()
        {
            InitializeComponent();

            PutUserSettings();

            string ver = GetVersion();
            this.Text += " " + ver;
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


        #region Events
        private void Form1_Load(object sender, EventArgs e)
        {
            _player = new SoundPlayer(Resources.notify);

            // first rec
            var instance = FakeWindow.Singleton;
            instance.HotKeyPressed += Instance_MagicHotKeyPressed;
            instance.CustomMessageReceived += Instance_CustomMessageReceived;
        }

        private void Instance_CustomMessageReceived(object sender, Message e)
        {
            throw new NotImplementedException();
        }

        private void Instance_MagicHotKeyPressed(object sender, Message e)
        {
            PlaySound();
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            var hwnd = user32.GetActiveWindow();

            StringBuilder sb = new StringBuilder();
            user32.GetWindowText(hwnd, sb, 10000);

            btnCaptureWin.Text = sb.ToString();
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
                notifyIcon1.BalloonTipTitle = "Stream Utilities works yet! :)";
                notifyIcon1.BalloonTipText = "I'm working in background! (double click on the icon to restore)";
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
                UserConfig.Singleton.Save();

                // after save new config is available
                PutUserSettings();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            cfg.Default.WinOpacity = trackBar1.Value;
            cfg.Default.Save();
        }
        #endregion

        private async void btnOpacity_Click(object sender, EventArgs e)
        {
            double opacity = Opacity;

            btnOpacity.Text = "Wait!";
            btnOpacity.Enabled = false;
            Opacity = (double)trackBar1.Value/(double)100;

            await Task.Delay(1500);
            btnOpacity.Text = "Test opacity";
            btnOpacity.Enabled = true;
            Opacity = opacity;
        }
    }
}
