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
using System.Windows.Forms;

namespace ShopeeAuto
{
    public static class Global
    {
        private static Dictionary<string, string> myDictionary = new Dictionary<string, string>();

        // Debug text area
        public static RichTextBox txtDebug;

        // ApiBuilder 
        public static ApiBuilder api = new ApiBuilder();

        // Chrome driver
        public static IWebDriver driver;
        public static WebDriverWait wait;

        public static void Init()
        {
            // Init Dictionary
            myDictionary.Add("colour", "Màu");
            myDictionary.Add("size", "Size");
            myDictionary.Add("黑色", "Đen");
            myDictionary.Add("gray", "Xám");
            myDictionary.Add("yellow", "Vàng");
            myDictionary.Add("红色", "Đỏ");
            myDictionary.Add("藏蓝", "Xanh tím than");
            myDictionary.Add("white", "Trắng");
            myDictionary.Add("blue", "Xanh da trời");
            myDictionary.Add("斤", " x 0.5kg");

            myDictionary.Add("s", "S");
            myDictionary.Add("l", "L");
            myDictionary.Add("m", "M");
            myDictionary.Add("xl", "XL");
            myDictionary.Add("xxl", "XXL");
            myDictionary.Add("xxXl", "XXXL");
            myDictionary.Add("2xl", "2XL");
            myDictionary.Add("3xl", "3XL");
            myDictionary.Add("4xl", "4XL");
            myDictionary.Add("5xl", "5XL");


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

        public static string FirstLetterToUpper(string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }

        public static string SimpleTranslate(string Data)
        {
            Data = Data.ToLower();
            Data = Data.Replace(":", " : ");
            Data = Data.Replace(".", " ");

            List<string> splits = Data.Split(' ').ToList<string>();
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
        }


        public static void AddLog(string text)
        {
            txtDebug.Invoke((MethodInvoker)delegate { txtDebug.Text += "[" + DateTime.Now.ToLongTimeString() + "] : " + text + "\n"; });
            File.AppendAllText("log.txt", "[" + DateTime.Now.ToLongTimeString() + "] : " + text + "\n");
            return;
        }
    }
}
