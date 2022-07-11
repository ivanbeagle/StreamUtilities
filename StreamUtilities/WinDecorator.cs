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
    public partial class WinDecorator : Form
    {
        #region Fields
        private static WinDecorator _singleton;
        Pen _pen;
        Timer _timer;
        #endregion

        #region Properties
        public static WinDecorator Singleton => _singleton ?? (_singleton = new WinDecorator());
        #endregion

        #region Ctors
        private WinDecorator()
        {
            InitializeComponent();

            _pen = new Pen(Brushes.Yellow, 4);

            _timer = new Timer();
            _timer.Tick += _timer_Tick;
        }
        #endregion

        public void MoveOnWindow(hwnd e)
        {
            user32.GetWindowRect(e, out RECT rect);

            WinDecorator.Singleton.Size = rect.Size + new Size(8, 12);
            WinDecorator.Singleton.Left = rect.Left - 4;
            WinDecorator.Singleton.Top = rect.Top - 8;

            WinDecorator.Singleton.Invalidate();
            WinDecorator.Singleton.Show();
        }

        public void HideAfter(int ms)
        {
            _timer.Stop();
            _timer.Interval = ms;
            _timer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var rect = ClientRectangle;
            rect.Inflate(-10, -10);

            e.Graphics.Clear(Color.Transparent);
            e.Graphics.DrawRectangle(_pen, rect);
        }

        #region Events
        private void _timer_Tick(object sender, EventArgs e)
        {
            Hide();
        }
        #endregion
    }
}
