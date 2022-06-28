using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using user32 = Vanara.PInvoke.User32;
using hwnd = Vanara.PInvoke.HWND;
using System.Diagnostics;

namespace StreamUtilities
{
    /// <summary>
    /// Identify a secret window with full access to WinAPI
    /// </summary>
    internal class FakeWindow : Form
    {
        #region Fields
        private static FakeWindow _singleton;
        #endregion

        #region Properties
        /// <summary>
        /// Get singleton
        /// </summary>
        public static FakeWindow Singleton => _singleton ?? (_singleton = new FakeWindow());

        /// <summary>
        /// Get custom message identifier
        /// </summary>
        public static uint CustomMessageId { get; private set; }
        #endregion

        #region Events
        /// <summary>
        /// Raised when the hotkey was pressed
        /// </summary>
        public event EventHandler<Message> HotKeyPressed;

        /// <summary>
        /// Raised when custom message is catched
        /// </summary>
        public event EventHandler<Message> CustomMessageReceived;
        #endregion

        #region Ctors
        private FakeWindow()
        {
            // get the window handle necessary!
            CreateHandle();

            // register hot key
            user32.RegisterHotKey(Handle, 1, user32.HotKeyModifiers.MOD_SHIFT | user32.HotKeyModifiers.MOD_CONTROL,
                (uint)StreamUtilitiesSettings.Default.Hotkey); // CTRL + SHIFT + D0 default

            // register the custom message (default: WM_BEAGLEINTERACTIVE_STREAMU)
            CustomMessageId = user32.RegisterWindowMessage(WinUtils.WM_CUSTOM_MESSAGE);

            // register click on other external windows and win events (win closed...)
            user32.RegisterShellHookWindow(Handle); // handle to this win to receive messages
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            user32.UnregisterHotKey(Handle, 1);
            user32.DeregisterShellHookWindow(Handle);

            base.Dispose(disposing);
        }

        protected override void WndProc(ref Message m)
        {
            // handle hotkey
            if(m.Msg == (int)WM.HOTKEY)
            {
                HotKeyPressed?.Invoke(this, m);
            }
            else
            // handle custom message
            if (m.Msg == CustomMessageId)
            {
                CustomMessageReceived?.Invoke(this, m);
            }

#if DEBUG
            Debug.WriteLine("FK WND: " + m.ToString());
#endif

            base.WndProc(ref m);
        }
    }
}
