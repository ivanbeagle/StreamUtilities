using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timer = System.Windows.Forms.Timer;

namespace StreamUtilities
{
    public class HitLabel : Label
    {
        public HitLabel() : base()
        {
            DoubleBuffered = true;
        }
    }
}
