using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace ShopeeAuto
{
    public static class Global
    {
        public static bool DebugMode = false;

        private static Dictionary<string, string> myDictionary = new Dictionary<string, string>();

        public static string myShopId = "";

        public static string myAccountId = "";

        public static string authToken = "";

        // Debug text area
        public static RichTextBox txtDebug;

        // ApiBuilder 
        public static ApiBuilder api = new ApiBuilder();

        // Chrome driver
        public static ChromeDriver driver;
        public static WebDriverWait wait;

        public static void Init()
        {

            //  StartDriver
            foreach (var processChrome in Process.GetProcessesByName("chrome"))
            {
                // processChrome.Kill();
            }

            /*
            Random rd = new Random();
            //string portremote = rd.Next(6666, 9999).ToString();
            string portremote = "9981";
            Process process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = "chrome";
            process.StartInfo.Arguments = "https://banhang.shopee.vn --disable-gpu --new-window --remote-debugging-port=" + portremote; //--window-position=0,0 --new-window --window-size=1200,800        
            //process.EnableRaisingEvents = true;
            //process.Exited += new EventHandler(myProcess_Exited);
            process.Start();
           
            var options = new ChromeOptions { DebuggerAddress = "localhost:" + portremote };
            //options.AddArguments("--disable-infobars");
            options.AddArgument("load-extension=C:\\Users\\Admin\\Desktop\\choom_shopee_header");
            ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();
            //chromeDriverService.HideCommandPromptWindow = true;
           */
            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("user-data-dir=C:/Users/Admin/AppData/Local/Google/Chrome/User Data Fake/");
            options.AddArguments("profile-directory=Profile 2");
            options.AddArguments("--disable-popup-blocking");
            options.AddArguments("start-maximized");

            try
            {
                driver = new ChromeDriver(chromeDriverService, options);
                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("Object.defineProperty(navigator, 'webdriver', { get: () => undefined })");

            }
            catch
            {
                driver = new ChromeDriver(chromeDriverService, options);
                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("Object.defineProperty(navigator, 'webdriver', { get: () => undefined })");
            }
            
        }

        public static string AntiDangStyle(string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
            {
                str = char.ToUpper(str[0]) + str.Substring(1);
            }
   
            str = str.Replace(",", ", ").Replace(".", ". ").Replace("  ", " ").Replace(" ,", ", ").Replace(" .", ". ").Replace("đẹpk", "đẹp").Replace("đepk", "đẹp");
            str = str.Trim();
            str = str.TrimEnd(',');
            str = str.Trim();
            str = str.TrimEnd(',');
            return str;
        }

        public static string Translate(string str)
        {
            ApiResult apiResult;
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                ["route"] = "translate/ahihi",
                ["text"] = str,
            };
            apiResult = Global.api.RequestMyApi(parameters);
            if (apiResult.success)
            {
                try
                {
                    dynamic results = JsonConvert.DeserializeObject<dynamic>(apiResult.content);
                    if(results.status == "error")
                    {
                        Global.AddLog("ERROR: Dịch bị lỗii " + results.message);
                    }
                    return results.data.vi;
                }
                catch (Exception e)
                {
                    Global.AddLog("ERROR: Dịch bị lỗi " + e.Message);
                    return str;
                }
               
            } else
            {
                return str;
            }
            /*
            str = str.ToLower();
            str = str.Replace(":", " : ");
            str = str.Replace(".", " ");

            List<string> splits = str.Split(' ').ToList<string>();
            string result = "";
            foreach (string split in splits)
            {
                if (int.TryParse(split, out int n))
                {
                    result += split + " ";
                }
                else if (myDictionary.ContainsKey(split))
                {
                    result += myDictionary[split] + " ";
                }

            }
            return result.Trim();
            */
        }


        public static void AddLog(string text)
        {
            txtDebug.Invoke((MethodInvoker)delegate { txtDebug.Text += "[" + DateTime.Now.ToLongTimeString() + "] : " + text + "\n"; });
            try
            {
                File.AppendAllText("log.txt", "[" + DateTime.Now.ToLongTimeString() + "] : " + text + "\n");
            }
            catch { }
           
            return;
        }
    }
}
