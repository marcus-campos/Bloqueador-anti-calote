using System;
using System.Reflection;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;

public class KeyboardHook
{
    [DllImport("user32.dll")]
    static extern bool UnhookWindowsHookEx(IntPtr hhk);
    [DllImport("user32.dll")]
    static extern IntPtr SetWindowsHookEx(int idHook, KeyboardHookDelegate lpfn, IntPtr hMod, uint dwThreadId);
    [DllImport("user32.dll")]
    static extern short GetAsyncKeyState(int vKey);
    [DllImport("user32.dll")]
    static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

    private struct KBDLLHOOKSTRUCT
    {
        public int vkCode;
        public int scanCode;
        public int flags;
        public int time;
        public int dwExtraInfo;
    }

    private const int HC_ACTION = 0x000;
    private const int LLKHF_EXTENDED = 0x001;
    private const int LLKHF_INJECTED = 0x010;
    private const int LLKHF_ALTDOWN = 0x020;
    private const int LLKHF_UP = 0x80;

    private const int VK_TAB = 0x009;
    private const int VK_CONTROL = 0x011;
    private const int VK_ALT = 0x12;
    private const int VK_ESCAPE = 0x01B;
    private const int VK_DELETE = 0x02E;
    private const int VK_F4 = 0x73;

    private const int WM_KEYDOWN = 0x100;
    private const int WM_SYSKEYDOWN = 0x104;
    private const int WM_SYSKEYUP = 0x105;
    private const int WM_KEYUP = 0x101;

    private const int WH_KEYBOARD_LL = 0x00D;

    private IntPtr KeyboardHandle;

    // Callback
    private int LowLevelKeyboardProc(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
    {
        if ((nCode == HC_ACTION))
        {
            switch ((int)wParam)
            {
                case WM_KEYDOWN:
                case WM_KEYUP:
                case WM_SYSKEYDOWN:
                case WM_SYSKEYUP:
                    /* ALT+F4 */
                    if ((lParam.flags & LLKHF_ALTDOWN) != 0 && lParam.vkCode == VK_F4)
                    {
                        Debug.WriteLine("ALT+F4");
                        return 1;  // engole mensagem
                    }
                    /* ALT+TAB */
                    if ((lParam.flags & LLKHF_ALTDOWN) != 0 && lParam.vkCode == VK_TAB)
                    {
                        Debug.WriteLine("ALT+TAB");
                        return 1;
                    }
                    /* CTRL+ESC */
                    if (((GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0) && lParam.vkCode == VK_ESCAPE)
                    {
                        Debug.WriteLine("CTRL+ESC");
                        return 1;
                    }
                    break;
            }
        }
        return CallNextHookEx(KeyboardHandle, nCode, wParam, ref lParam);
    }

    private delegate int KeyboardHookDelegate(int Code, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);
    [MarshalAs(UnmanagedType.FunctionPtr)]
    private KeyboardHookDelegate callback;

    public void HookKeyboard()
    {
        callback = new KeyboardHookDelegate(LowLevelKeyboardProc);
        KeyboardHandle = SetWindowsHookEx(WH_KEYBOARD_LL, callback,
                         Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), (uint)0);
        if ((int)KeyboardHandle == 0)
        {
            throw new Exception("Hook Falhou");
        }
    }

    public void UnhookKeyboard()
    {
        if ((int)KeyboardHandle != 0)
        {
            UnhookWindowsHookEx(KeyboardHandle);
        }
    }
}