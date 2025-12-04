using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ExportData
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            string oldExe = Application.ExecutablePath + ".oldExport";
            try
            {
                if (File.Exists(oldExe))
                    File.Delete(oldExe);
            }
            catch { }
        }
    }
}
