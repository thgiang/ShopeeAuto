using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShopeeAuto
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Init web driver
            Global.Init();

            // Close event
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            // Open Main Form
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
           
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            Global.driver.Close();
            Global.driver.Quit();
        }
    }
}
