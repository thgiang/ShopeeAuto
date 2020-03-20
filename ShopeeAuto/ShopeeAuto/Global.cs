﻿using Newtonsoft.Json;
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
        public static IWebDriver driver;
        public static WebDriverWait wait;

        public static void Init()
        {
            /*
            // Init Dictionary
            myDictionary.Add("colour", "Màu");
            myDictionary.Add("size", "Size");
            myDictionary.Add("黑色", "Đen");
            myDictionary.Add("浅粉色", "Hồng nhạt");
            myDictionary.Add("深粉色", "Hồng đậm");
            myDictionary.Add("黄色", "Vàng");
            myDictionary.Add("红色", "Đỏ");
            myDictionary.Add("藏蓝", "Xanh tím than");
            myDictionary.Add("宝蓝", "Xanh nước biển");
            myDictionary.Add("白色", "Trắng");
            myDictionary.Add("斤", " x 0.5kg");

            myDictionary.Add("xs", "XS");
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
            */

            //  StartDriver
            foreach (var process in Process.GetProcessesByName("chrome"))
            {
               // process.Kill();
            }

            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("user-data-dir=C:/Users/Admin/AppData/Local/Google/Chrome/User Data Fake/");
            options.AddArguments("profile-directory=Profile 1");
            options.AddArguments("start-maximized");
            try
            {
                driver = new ChromeDriver(chromeDriverService, options);
                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                Dictionary<string, object> newparams = new Dictionary<string, object>();
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("Object.defineProperty(navigator, 'webdriver', { get: () => undefined })");

            }
            catch
            {
                driver = new ChromeDriver(chromeDriverService, options);
                wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
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

        public static string Translate(string str, int save = 1)
        {
            ApiResult apiResult;
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                ["route"] = "translate/ahihi",
                ["text"] = str,
                ["save"] = save.ToString(), // Dich va luu hay ko, 1 la co luu, 0 la dich ko luu
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
