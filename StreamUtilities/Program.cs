using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StreamUtilities
{
    internal static class Program
    {
        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var mutex = new Mutex(false, "StreamUtilities-Mutex"))
            {
                bool isAnotherInstanceOpen = !mutex.WaitOne(TimeSpan.Zero);
                if (isAnotherInstanceOpen)
                {
                    const string MSG = "Another instance of StreamUtilities is running, quit.";

                    MessageBox.Show(MSG);
                    Console.WriteLine(MSG);
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                WinUtils.ConfigureCustomMessageFilters();

                Application.Run(new Form1());

                mutex.ReleaseMutex();
            }
        }
    }
}
