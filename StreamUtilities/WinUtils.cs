using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

using user32 = Vanara.PInvoke.User32;
using hwnd = Vanara.PInvoke.HWND;

namespace StreamUtilities
{
    public class WinUtils
    {
        public const string WM_CUSTOM_MESSAGE = "WM_BEAGLEINTERACTIVE_STREAMU";

        /// <summary>
        /// Handled custom messages before propagate to windows
        /// </summary>
        internal static void ConfigureCustomMessageFilters()
        {
            //Application.AddMessageFilter(new MyMessageFilter());
        }
    }
}
