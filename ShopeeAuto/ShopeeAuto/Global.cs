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

        // Debug text area
        public static RichTextBox txtDebug;

        // ApiBuilder 
        public static ApiBuilder api = new ApiBuilder();

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
