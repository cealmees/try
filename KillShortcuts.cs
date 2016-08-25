using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Microsoft.Win32;

namespace LauncherDeny
{
    public class KillShortcuts
    {
        
        public static void disableSpecialKeys()
        {
            int intLLKey = SetWindowsHookEx(WH_KEYBOARD_LL, LowLevelKeyboardProc, System.Runtime.InteropServices.Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0]).ToInt32(), 0);
        }

        //status = 0 to Enable and status = 1 to Disable Task Manager 
        public static void statusTaskManager(int status)
        {
            RegistryKey regkey;
            string subKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";
            string info;

            try
            {
                regkey = Registry.CurrentUser.CreateSubKey(subKey);
                regkey.SetValue("DisableTaskMgr", status);
                regkey.Close();
            }
            catch(Exception ex)
            {
                info = ex.ToString();
                Console.WriteLine(info);
                
            }

        }



        [DllImport("user32", EntryPoint = "SetWindowsHookExA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int SetWindowsHookEx(int idHook, LowLevelKeyboardProcDelegate lpfn, int hMod, int dwThreadId);
        [DllImport("user32", EntryPoint = "UnhookWindowsHookEx", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int UnhookWindowsHookEx(int hHook);
        private delegate int LowLevelKeyboardProcDelegate(int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam);
        [DllImport("user32", EntryPoint = "CallNextHookEx", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        private static extern int CallNextHookEx(int hHook, int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam);
        private const int WH_KEYBOARD_LL = 13; //id for special keys 

        /*code needed to disable start menu*/
        [DllImport("user32", CharSet = CharSet.Auto)]
        private static extern int FindWindow(string className, string windowText);

        [DllImport("user32")]
        private static extern int ShowWindow(int hwnd, int command);
        //[DllImport("user32", CharSet = CharSet.Auto)]
        //private static extern int GetStart (string className, string  windowText);
        //[DllImport("user32")]
        //private static extern int KillStart(int hwnd, int command);


        private struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
        private static int LowLevelKeyboardProc(int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam)
        {

            try
            {
                bool blnEat = false;
                switch (wParam)
                {
                    case 256:
                    case 257:
                    case 260:
                    case 261:
                        //Special Keys: Alt+Tab, Alt+Esc, Ctrl+Esc, Windows Key + A-Z
                        blnEat = ((lParam.vkCode == 9) && (lParam.flags == 32)) | ((lParam.vkCode == 27) && (lParam.flags == 32)) | ((lParam.vkCode == 27) && (lParam.flags == 0)) | ((lParam.vkCode == 91) && (lParam.flags == 1)) | ((lParam.vkCode == 92) && (lParam.flags == 1)) | ((lParam.vkCode == 73) && (lParam.flags == 0));
                        break;
                }

                if (blnEat == true)
                {
                    return 1;
                }
                else
                {
                    return CallNextHookEx(0, nCode, wParam, ref lParam);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return CallNextHookEx(0, nCode, wParam, ref lParam);
            }




        }

        //status = 0 to disable Start Menu, status = 1 to enable Start Menu
        public static void disableStartMenu()
        {
            int hwnd = FindWindow("Shell_TrayWnd", null);
            ShowWindow(hwnd, 0);
            //int start = GetStart("Button", "Start");
            //KillStart(start, 0);

        }

    }

}
