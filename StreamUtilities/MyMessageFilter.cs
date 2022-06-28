using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StreamUtilities
{
    internal class MyMessageFilter : IMessageFilter
    {
        public bool PreFilterMessage(ref Message m)
        {
            // gestire qui il messaggio e ritornare true una volta gestito

            return false;
        }
    }
}
