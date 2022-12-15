using System;
using System.Windows.Forms;

namespace BlueByte.SOLIDWORKS.PDMProfessional.Services
{
    /// <summary>
    /// Simple implementation of the IWin32Window.
    /// </summary>
    /// <seealso cref="System.Windows.Forms.IWin32Window" />
    public class Win32Window : IWin32Window
    {

        public IntPtr Handle { get; set; }

        public Win32Window(int handle)
        {
            this.Handle = new IntPtr(handle);
        }

        public Win32Window(long handle)
        {
            this.Handle = new IntPtr(handle);
        }

        public Win32Window()
        {

        }
    }
}
