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
using Vanara.PInvoke;
using Timer = System.Windows.Forms.Timer;

namespace StreamUtilities
{
    public partial class TopNotification : Form
    {
        #region Fields
        private static TopNotification _singleton;
        Timer _timer;
        #endregion

        #region Properties
        public static TopNotification Singleton => _singleton ?? (_singleton = new TopNotification());
        #endregion

        #region Ctors
        private TopNotification()
        {
            InitializeComponent();

            _timer = new Timer();
            _timer.Tick += _timer_Tick;
        }
        #endregion

        public void MoveOnWindow(hwnd e)
        {
            user32.GetWindowRect(e, out RECT rect);

            WinDecorator.Singleton.Size = rect.Size + new Size(5, 5);
            WinDecorator.Singleton.Left = rect.Left - 5;
            WinDecorator.Singleton.Top = rect.Top - 5;

            WinDecorator.Singleton.Invalidate();
            WinDecorator.Singleton.Show();
        }

        public void HideAfter(int ms)
        {
            _timer.Stop();
            _timer.Interval = ms;
            _timer.Start();
        }

        #region Events
        private void _timer_Tick(object sender, EventArgs e)
        {
            Hide();
        }
        #endregion
    }
}
