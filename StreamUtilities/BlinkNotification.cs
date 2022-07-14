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
using cfg = StreamUtilities.StreamUtilitiesSettings;
using Vanara.PInvoke;

namespace StreamUtilities
{
    public partial class BlinkNotification : Form
    {
        #region Fields
        private BlinkLabel _lastLabel;
        private List<BlinkLabel> _labels;
        #endregion

        #region Ctors
        public BlinkNotification()
        {
            InitializeComponent();

            _labels = new List<BlinkLabel>();

            flowLayoutPanel1.MaximumSize = new Size(Screen.PrimaryScreen.Bounds.Width, 150);
            
            // TEST pourpose only!
            //timer1.Start();
        }
        #endregion

        public void AddNotify(bool special, string type, string owner, string msg) 
        {
            var l = new BlinkLabel(special, type, owner, msg);
            l.WordsTimeReached += L_WordsTimeReached;
            l.FadeOutCompleted += L_FadeOutCompleted;

            _labels.Add(l);
            _lastLabel = l;

            flowLayoutPanel1.Invoke(() =>
            {
                flowLayoutPanel1.Controls.Add(l);

                // with one label i can start events of scroll, no one blinklabel has to wait before this closes!
                if (_labels.Count == 1)
                    l.ListenEvents();
            });
        }

        #region Events
        #region TEST
        //private void timer1_Tick(object sender, EventArgs e)
        //{
        //    //timer1.Stop();
        //    //timer1.Enabled = false;
        //    if (DateTime.Now.Second % 2 == 0)
        //    {
        //        if (new Random((int)DateTime.Now.Ticks).Next(0, 10) > 5)
        //            return;
        //    }

        //    var frases = new[]
        //    {
        //        "ciao!",
        //        "sto morendo xD",
        //        "lol",
        //        "quando farai una live?",
        //        "figa sta cosa",
        //        "mi faresti un autografo? :)",
        //        "secondo me dovresti cambiare strada, li è pieno di nemici! xD",
        //        "ma ad Apex?",
        //        "ci facciamo una partita dopo?",
        //        "porteresti un gioco horror?",
        //        "non sai giocare!",
        //        "a quando la prossima?",
        //        "ho un pò di sonno, ci vediamo",
        //        "hai appena vinto un milione!!!",
        //        "cosaaa???",
        //        "è pieno di bug sto gioco",
        //    };

        //    var pg = new[]
        //    {
        //        "FraCristofaro",
        //        "Skull88",
        //        "Alice93",
        //        "Kronos",
        //        "Yoyo",
        //        "__underscore__",
        //        "Bot",
        //        "Mag0g",
        //        "Lenfisho",
        //        "0xbabbe0",
        //        "IlMitico!"
        //    };

        //    Random r = new Random((int)DateTime.Now.Ticks);
        //    var frase = frases[r.Next(frases.Length)];
        //    var p = pg[r.Next(pg.Length)];

        //    bool special = r.Next(0, frase.Length) % 2 == 0;

        //    AddNotify(special, p, frase);
        //}
        #endregion

        private void L_WordsTimeReached(object sender, EventArgs e)
        {
            // at end of word count this label fade out!
            BlinkLabel l = sender as BlinkLabel;
            l.FadeOut();
        }
        private void L_FadeOutCompleted(object sender, EventArgs e)
        {
            BlinkLabel l = sender as BlinkLabel;

            flowLayoutPanel1.Invoke(() =>
            {
                flowLayoutPanel1.Controls.Remove(l);
                _labels.Remove(l);

                // just repeat scroll for the first label available in list
                if (_labels.Count > 0)
                    _labels[0]?.ListenEvents();
            });
        }
        #endregion
    }
}
