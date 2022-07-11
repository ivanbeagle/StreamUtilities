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
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void linkLabel1_Click(object sender, EventArgs e)
        {
            WinUtils.GoUrl("https://twitch.tv/beagleinteractive");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            WinUtils.GoUrl("https://github.com/dahall/vanara");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            WinUtils.GoUrl("https://github.com/TwitchLib/TwitchLib");
        }

        private void btnDonate_Click(object sender, EventArgs e)
        {
            WinUtils.GoUrl("https://www.paypal.com/donate/?hosted_button_id=FM9FABR9Z9X8N");
        }
    }
}
