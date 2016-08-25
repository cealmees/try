using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Win32;

namespace LauncherDeny
{
    class Program:KillShortcuts
    {

        public static void Main(string[] args)
        {
            try
            {
                Process.Start("calculator:");
                statusTaskManager(1);
                disableSpecialKeys();
                disableStartMenu();

                //1 para desactivar taskManager, 0 para activar.


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.Read();
        }
    }
}
