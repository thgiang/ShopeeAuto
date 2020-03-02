using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShopeeAuto
{
    public static class Global
    {
       
        // Debug text area
        public static RichTextBox txtDebug;

        // ApiBuilder 
        public static ApiBuilder api = new ApiBuilder();

        // Chrome driver
        public static IWebDriver driver;
        public static WebDriverWait wait;

        public static void InitDriver()
        {
            //  StartDriver
            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("user-data-dir=C:/Users/Admin/AppData/Local/Google/Chrome/User Data Fake/");
            options.AddArguments("profile-directory=Profile 1");
            options.AddArguments("start-maximized");
            driver = new ChromeDriver(chromeDriverService, options);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
        }

        public static void AddLog(string text)
        {
            txtDebug.Invoke((MethodInvoker)delegate { txtDebug.Text += "[" + DateTime.Now.ToLongTimeString() + "] : " + text + "\n"; });
            return;
        }

        public static void AddEndProcessLog()
        {
            txtDebug.Invoke((MethodInvoker)delegate { txtDebug.Text += "======================= \n"; });
            return;
        }
    }
}
