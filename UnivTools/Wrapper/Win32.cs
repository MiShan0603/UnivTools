using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnivTools.Wrapper
{
    public class Win32
    {
        public static int WS_EX_NOACTIVATE = 0x08000000;
        public static IntPtr HWND_TOPMOST = (IntPtr)(-1);
    }
}
