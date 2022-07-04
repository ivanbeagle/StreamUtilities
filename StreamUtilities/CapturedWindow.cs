using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using user32 = Vanara.PInvoke.User32;
using hwnd = Vanara.PInvoke.HWND;
using cfg = StreamUtilities.StreamUtilitiesSettings;
using Vanara.PInvoke;

namespace StreamUtilities
{
    internal sealed class CapturedWindow
    {
        #region Fields
        private hwnd _handle;
        private int _style;
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
        }
        #endregion

        public void MoveOnTop(bool value) { }
        public void EnableTransparency(bool value) { }

        public void AllowInteraction(bool value) { }

        public void SetOpacityValue(int opacity)
        {
            throw new NotImplementedException();
        }

        public void Restore() { }
    }
}
