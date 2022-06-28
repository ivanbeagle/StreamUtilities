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

namespace StreamUtilities
{
    public partial class SettingsForm : Form
    {
        #region Fields
        #endregion

        #region Ctors
        public SettingsForm()
        {
            InitializeComponent();

            UserConfig.Singleton.LoadFromConfig();

            PutUserConfigToUI();
        }
        #endregion

        private void PutUserConfigToUI()
        {
            var cfg = UserConfig.Singleton;

            txtKey.Text = "CTRL + SHIFT " + cfg.Hotkey;
            txtKey.Tag = cfg.Hotkey;

            txtUsername.Text = cfg.TwitchUsername;
            txtToken.Text = cfg.TwitchAccessToken;
            txtChannel.Text = cfg.TwitchChannel;
        }

        public void PutUIToUserConfig()
        {
            var cfg = UserConfig.Singleton;

            cfg.Hotkey = (Keys)txtKey.Tag;
            cfg.TwitchUsername = txtUsername.Text;
            cfg.TwitchAccessToken = txtToken.Text;
            cfg.TwitchChannel = txtChannel.Text;
        }

        #region Events
        private void btnDefault_Click(object sender, EventArgs e)
        {
            UserConfig.Singleton.LoadFactoryDefault();

            PutUserConfigToUI();

            MessageBox.Show(this, "Factory reset DONE!", "Factory reset", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void txtKey_Enter(object sender, EventArgs e)
        {
            sugg.Visible = true;
            button1.Visible = true;
        }

        private void txtKey_Leave(object sender, EventArgs e)
        {
            sugg.Visible = false;
            button1.Visible = false;
        }

        private void txtKey_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Escape:
                    button1.Focus();
                    return;

                case Keys.Control:
                case Keys.ControlKey:
                case Keys.LControlKey:
                case Keys.RControlKey:

                case Keys.Shift:
                case Keys.ShiftKey:
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                    return;
            }

            txtKey.Text = "CTRL + SHIFT + " + e.KeyCode;
            txtKey.Tag = e.KeyCode;
        }
        #endregion

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {
            if(txtUsername.Text.Length > 0)
                txtChannel.Text = txtUsername.Text;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://twitchtokengenerator.com/");
        }
    }
}
