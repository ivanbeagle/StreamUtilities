using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using user32 = Vanara.PInvoke.User32;
using hwnd = Vanara.PInvoke.HWND;
using cfg = StreamUtilities.StreamUtilitiesSettings;
using Vanara.PInvoke;
using System.Diagnostics;

namespace StreamUtilities
{
    internal sealed class CapturedWindow
    {
        #region Fields
        private hwnd _handle;
        private int _style;
        private int _gwl_exstyle;
        private COLORREF _layerColor;
        private user32.LayeredWindowAttributes _layerFlags;
        private byte _layerAlpha;
        #endregion

        #region Properties
        public string ClassName { get; private set; }

        public string Caption { get; private set; }

        public IntPtr Handle { get; private set; }

        public string Id => ClassName + "_" + Handle;
        #endregion

        #region Ctors
        public CapturedWindow(hwnd handle)
        {
            _handle = handle;

            ClassName = FakeWindow.GetClass(handle);
            Caption = FakeWindow.GetCaption(handle);
            Handle = (IntPtr)_handle;

            _gwl_exstyle = user32.GetWindowLong(handle, user32.WindowLongFlags.GWL_EXSTYLE);
            user32.GetLayeredWindowAttributes(handle, out COLORREF _layerColor, out _layerAlpha, out _layerFlags);
        }
        #endregion

        public static byte GetGlobalTransparency()
        {
            var x = MathF.Round(((float)Global.Opacity / 100f) * 255f);
            return (byte)x;
        }

        public void Ghostize(bool value)
        {
            try
            {
                user32.GetWindowRect(_handle, out RECT winRect);

                if (value)
                {
                    user32.SetWindowLong(_handle, user32.WindowLongFlags.GWL_EXSTYLE,
                        (int)_gwl_exstyle |
                        (int)user32.WindowStylesEx.WS_EX_LAYERED);

                    user32.SetLayeredWindowAttributes(_handle,
                        new COLORREF(255, 255, 255), GetGlobalTransparency(),
                        user32.LayeredWindowAttributes.LWA_COLORKEY | user32.LayeredWindowAttributes.LWA_ALPHA);

                    user32.SetWindowPos(_handle, new IntPtr(-1), 
                        winRect.X, winRect.Y, winRect.Width, winRect.Height, user32.SetWindowPosFlags.SWP_SHOWWINDOW);

                    user32.UpdateWindow(_handle);
                }
                else
                {
                    user32.SetWindowLong(_handle, user32.WindowLongFlags.GWL_EXSTYLE, _gwl_exstyle);
                    user32.SetLayeredWindowAttributes(_handle, _layerColor, _layerAlpha, _layerFlags);

                    user32.SetWindowPos(_handle, new IntPtr(-2),
                        winRect.X, winRect.Y, winRect.Width, winRect.Height, user32.SetWindowPosFlags.SWP_SHOWWINDOW);

                    user32.UpdateWindow(_handle);
                }
            }
            catch(Exception ex) {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
        }
    }
}
