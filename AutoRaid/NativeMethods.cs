using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace AutoRaid
{
    class NativeMethods
    {
        const uint WM_KEYDOWN = 0x0100;
        const uint WM_KEYUP = 0x0101;

        //const uint WM_SYSKEYDOWN = 0x0104;
        //const int VK_F5 = 0x74;
        //const int VK_NUM1 = 0x31;
        //const int VK_NUM2 = 0x32;
        const int VK_NUM4 = 0x34;

        [DllImport("User32.Dll", EntryPoint = "PostMessageA", SetLastError = true)]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public static void PostVirtualKeyMessage()
        {
            uint repeatCount = 1;
            uint scanCode = 0x05;
            uint extended = 0;
            uint context = 0;
            uint previousState = 0;
            uint transition = 0;

            uint lParam = repeatCount
                | (scanCode << 16)
                | (extended << 24)
                | (context << 29)
                | (previousState << 30)
                | (transition << 31);

            //var proc = new IntPtr(0x002B076E);
            var proc = new IntPtr(0x00010582);

            //foreach (var proc in procs)
            //{
            PostMessage(proc, WM_KEYDOWN, (IntPtr)VK_NUM4, (IntPtr)lParam);

            Thread.Sleep(50);


            //KEY UP
            uint keyUpParam = repeatCount
                | (scanCode << 16)
                | (extended << 24)
                | ((uint)0 << 29)
                | ((uint)1 << 30)
                | ((uint)1 << 31);

            PostMessage(proc, WM_KEYUP, (IntPtr)VK_NUM4, (IntPtr)keyUpParam);
        }
    }
}
