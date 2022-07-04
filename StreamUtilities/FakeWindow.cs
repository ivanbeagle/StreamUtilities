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

        public bool AllowShellHookCapture { get; set; } = true;
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
                if (AllowShellHookCapture)
                {
                    int wp = m.WParam.ToInt32();
                    switch (wp)
                    {
                        case HSHELL_WINDOWACTIVATED:
                        case HSHELL_RUDEAPPACTIVATED:
                        case 0xc029:
                            var hwnd = user32.GetForegroundWindow();

                            // ignore windows of this process
                            var pid = GetPid(hwnd);
                            var pidp = GetPid();
                            if (pid == pidp)
                                break;

                            if (hwnd == m.LParam)
                            {
                                if(GetClass(hwnd).ToLower().Contains("streamutilities") || GetCaption(hwnd).ToLower().Contains("streamutilities"))
                                {
                                    Debug.WriteLine("[FKWND] Patch!");
                                    break;
                                }

#if DEBUG
                                Debug.WriteLine($"[FKWND] activated handle: {(IntPtr)hwnd} ({GetClass(hwnd)} ; {GetCaption(hwnd)})");
#endif
                                ShellWindowActivated?.Invoke(this, hwnd);
                            }
                            else
                            {
                                if (GetClass(hwnd).ToLower().Contains("streamutilities") || GetCaption(hwnd).ToLower().Contains("streamutilities"))
                                {
                                    Debug.WriteLine("[FKWND] Patch!");
                                    break;
                                }

                                // what is this message? 0xc029
                                var caption = GetCaption(hwnd);

                                Debug.WriteLine($"[FKWND???] {(IntPtr)hwnd} ({GetClass(hwnd)} ; {caption})");

                                try
                                {
                                    var dataGWL_HWNDPARENT = user32.GetWindowLongAuto(hwnd, user32.WindowLongFlags.GWL_HWNDPARENT);
                                    var dataGWL_EXSTYLE = user32.GetWindowLongAuto(hwnd, user32.WindowLongFlags.GWL_EXSTYLE);
                                    var dataGWL_STYLE = user32.GetWindowLongAuto(hwnd, user32.WindowLongFlags.GWL_STYLE);

                                    // if this "window" has a caption and parent window? is good for me, but this code could be improved!

                                    if (dataGWL_HWNDPARENT != null && dataGWL_HWNDPARENT != IntPtr.Zero && !string.IsNullOrWhiteSpace(caption))
                                        ShellWindowActivated?.Invoke(this, hwnd);


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
            }

            base.WndProc(ref m);
        }


        public static string GetClass(hwnd wnd, int capacity = 200)
        {
            var sb = new StringBuilder(capacity);
            user32.GetClassName(wnd, sb, capacity);
            return sb.ToString();
        }
        public static string GetCaption(hwnd wnd, int capacity = 200)
        {
            var sb = new StringBuilder(capacity);
            user32.GetWindowText(wnd, sb, capacity);
            return sb.ToString();
        }
        public static uint GetPid(hwnd wnd)
        {
            user32.GetWindowThreadProcessId(wnd, out uint pid);
            return pid;
        }
        public static int GetPid()
        {
            return Process.GetCurrentProcess().Id;
        }
    }
}
