using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using user32 = Vanara.PInvoke.User32;
using hwnd = Vanara.PInvoke.HWND;
using System.Diagnostics;
using StreamUtilities;

namespace TestStreamUtilities
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var customMessageId = user32.RegisterWindowMessage(WinUtils.WM_CUSTOM_MESSAGE);
            user32.PostMessage(new hwnd(new IntPtr((int)numericUpDown1.Value)), customMessageId, IntPtr.Zero, IntPtr.Zero);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
