using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

using user32 = Vanara.PInvoke.User32;
using hwnd = Vanara.PInvoke.HWND;
using cfg = StreamUtilities.StreamUtilitiesSettings;
using Vanara.PInvoke;
using System.Diagnostics;

namespace StreamUtilities
{
    public sealed partial class BlinkLabel : UserControl
    {
        #region Fields
        private bool _isBroken = false;
        private bool _listen = false;

        private Timer _timer;
        private Bitmap _bitmap;
        private PictureBox _picture;
        private Color _backColor, _foreColor;

        private int _w;
        #endregion

        #region Events
        public event EventHandler WordsTimeReached;
        public event EventHandler FadeOutCompleted;
        #endregion

        #region Properties
        public int CalculatedDelay { get; protected set; }
        public bool IsFadingOut { get; protected set; }
        public bool IsBroken => _isBroken;
        public bool IsWorking => _listen && _isBroken;
        #endregion

        #region Ctors
        internal BlinkLabel() : base()
        {
            InitializeComponent();

            _backColor = Color.FromArgb(BackColor.R, BackColor.G, BackColor.B);
            _foreColor = Color.FromArgb(ForeColor.R, ForeColor.G, ForeColor.B);
        }

        public BlinkLabel(bool special, string type, string owner, string msg, int delayOffset = 0) : base()
        {
            InitializeComponent();

            if(special)
            {
                this.type.BackColor = Color.Yellow;
                this.type.ForeColor = Color.FromArgb(5, 5, 5);
            }

            this.type.Text = type;
            this.owner.Text = owner;
            this.message.Text = msg;

            _backColor = Color.FromArgb(BackColor.R, BackColor.G, BackColor.B);
            _foreColor = Color.FromArgb(ForeColor.R, ForeColor.G, ForeColor.B);

            var words = msg.Split(' ');
            var t = words.Length * 400;
            t += 1000;
            CalculatedDelay = t;

            _timer = new();
            _timer.Interval = t + delayOffset;
            _timer.Tick += _timer_Tick;
        }
        #endregion
        public void ListenEvents()
        {
            if (_isBroken || _listen)
                return;

            _listen = true;
            _timer.Start();
        }
        
        public void Restore()
        {
            BackColor = _backColor;
            ForeColor = _foreColor;
        }

        public void FadeOut()
        {
            if (_isBroken)
                return;

            _bitmap = new Bitmap(flowLayoutPanel1.Size.Width, flowLayoutPanel1.Size.Height);
            _picture = new PictureBox();
            _picture.Width = flowLayoutPanel1.Size.Width;
            _picture.Height = flowLayoutPanel1.Size.Height;
            //_picture.SizeMode = PictureBoxSizeMode.Normal;
            _picture.Margin = new Padding(0);
            _picture.Padding = new Padding(0);
            flowLayoutPanel1.DrawToBitmap(_bitmap, flowLayoutPanel1.Bounds);
            _picture.Image = _bitmap;

            this.flowLayoutPanel1.Controls.Clear();
            this.flowLayoutPanel1.Controls.Add(_picture);

            if (_timer != null)
                ClearTimer();

            _timer = new();
            _timer.Interval = 20; //5;
            _timer.Enabled = true;
            _timer.Tick += _fadeOut_Tick;
            _timer.Start();
        }

        private void ClearTimer()
        {
            _timer.Stop();
            _timer.Enabled = false;
            _timer.Dispose();
            _timer = null;
        }


        #region Events
        private void _timer_Tick(object sender, EventArgs e)
        {
            ClearTimer();

            BackColor = _backColor;
            WordsTimeReached?.Invoke(this, EventArgs.Empty);
        }

        private void _fadeOut_Tick(object sender, EventArgs e)
        {
            if (!_isBroken)
            {
                _isBroken = true;
                IsFadingOut = true;
                AutoSize = false;

                _w = _picture.Width - 4;
            }

            Size = new Size(_w, Size.Height);
            _w -= 4;

            if (_w <= 0)
            {
                _w = 0;
                IsFadingOut = false;
                FadeOutCompleted?.Invoke(this, EventArgs.Empty);
            }
        }
        #endregion
    }
}
