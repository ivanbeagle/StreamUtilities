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
        #region Consts
        private const int HSHELL_RUDEAPPACTIVATED = 32772;
        private const int HSHELL_WINDOWACTIVATED = 4;
        #endregion

        #region Fields
        private static FakeWindow _singleton;
        private static uint _shellHook;

        private IntPtr _handle;
        private IntPtr _parent;
        private uint _pid;
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

        /// <summary>
        /// Raised when a window was activated
        /// </summary>
        public event EventHandler<hwnd> ShellWindowActivated;
        #endregion

        #region Ctors
        private FakeWindow()
        {
            // get the window handle necessary!
            CreateHandle();
            _handle = Handle;

            // register hot key
            user32.RegisterHotKey(Handle, 1, user32.HotKeyModifiers.MOD_SHIFT | user32.HotKeyModifiers.MOD_CONTROL,
                (uint)StreamUtilitiesSettings.Default.Hotkey); // CTRL + SHIFT + D0 default

            // register the custom message (default: WM_BEAGLEINTERACTIVE_STREAMU)
            CustomMessageId = user32.RegisterWindowMessage(WinUtils.WM_CUSTOM_MESSAGE);
            _shellHook = user32.RegisterWindowMessage("SHELLHOOK");

            // register click on other external windows and win events (win closed...)
            user32.RegisterShellHookWindow(Handle); // handle to this win to receive messages

#if DEBUG
            Debug.WriteLine($"[FKWND] this handle: {_handle} ({GetClass(_handle)} ; {GetCaption(_handle)})"); 
#endif
        }
        #endregion


        public bool UpdateHotkey(Keys newHotkey)
        {
            if(user32.UnregisterHotKey(Handle, 1)) {
                // register hot key
                return user32.RegisterHotKey(Handle, 1, user32.HotKeyModifiers.MOD_SHIFT | user32.HotKeyModifiers.MOD_CONTROL,
                    (uint)newHotkey); // CTRL + SHIFT + D0 default            
            }
            return false;
        }

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
            // handle hooks
            if (m.Msg == _shellHook)
            {
                int wp = m.WParam.ToInt32();
                switch(wp)
                {
                    case HSHELL_WINDOWACTIVATED:
                    case HSHELL_RUDEAPPACTIVATED:
                    case 0xc029:
                        var hwnd = user32.GetForegroundWindow();

                        if (hwnd == m.LParam)
                        {
                            var ptrhwnd = (IntPtr)hwnd;
                            // ignore windows of this process
                            var pid = GetPid(ptrhwnd);
                            if (pid == GetPid())
                                break;

#if DEBUG
                            Debug.WriteLine($"[FKWND] activated handle: {ptrhwnd} ({GetClass(ptrhwnd)} ; {GetCaption(ptrhwnd)})");
#endif
                            ShellWindowActivated?.Invoke(this, hwnd);
                        }
                        else
                        {
                            // what is this message? 0xc029
                            var ptrhwnd = (IntPtr)hwnd;
                            var caption = GetCaption(ptrhwnd);

                            Debug.WriteLine($"[FKWND???] {ptrhwnd} ({GetClass(ptrhwnd)} ; {caption})");

                            // if this "window" has a caption is good for me, but this code could be improved!

                            if(!string.IsNullOrWhiteSpace(caption))
                                ShellWindowActivated?.Invoke(this, hwnd);

                            try
                            {
                                var dataGWL_HWNDPARENT = user32.GetWindowLongAuto(hwnd, user32.WindowLongFlags.GWL_HWNDPARENT);
                                var dataGWL_EXSTYLE = user32.GetWindowLongAuto(hwnd, user32.WindowLongFlags.GWL_EXSTYLE);
                                var dataGWL_STYLE = user32.GetWindowLongAuto(hwnd, user32.WindowLongFlags.GWL_STYLE);

                                Debug.WriteLine($"GWL_HWNDPARENT = {dataGWL_HWNDPARENT} ; GWL_EXSTYLE = {dataGWL_EXSTYLE} ; GWL_STYLE = {dataGWL_STYLE}");
                            } 
                            catch { /* not a window */ }
                        }
                        break;

                    default:
#if DEBUG
                        Debug.WriteLine(m);
#endif
                        break;
                }
            }

            base.WndProc(ref m);
        }


        private string GetClass(IntPtr wnd, int capacity = 200)
        {
            var sb = new StringBuilder(capacity);
            user32.GetClassName(wnd, sb, capacity);
            return sb.ToString();
        }
        private string GetCaption(IntPtr wnd, int capacity = 200)
        {
            var sb = new StringBuilder(capacity);
            user32.GetWindowText(wnd, sb, capacity);
            return sb.ToString();
        }
        private uint GetPid(IntPtr wnd)
        {
            user32.GetWindowThreadProcessId(wnd, out uint pid);
            return pid;
        }
        private int GetPid()
        {
            return Process.GetCurrentProcess().Id;
        }
    }
}
