using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Dynamic;
namespace ShopeeAuto
{
    class ShopeeWorker
    {
        public bool Login()
        {

            bool needToLogin = false;
            Global.AddLog("Kiểm tra Shopee đã đăng nhập chưa");
            Global.driver.Navigate().GoToUrl("https://banhang.shopee.vn/account/signin");
            try
            {
                if (Global.wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id=\"app\"]/div[2]/div/div[1]/div/div/div[3]/div/div/div/form/button/span"))).Text == "Đăng nhập")
                {
                    Global.AddLog("Chưa đăng nhập, đang đăng nhập");
                    needToLogin = true;
                }
            }
            catch (OpenQA.Selenium.WebDriverTimeoutException)
            {
                Global.AddLog("Đã đăng nhập");
            }

            if(needToLogin)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                dynamic results = new ExpandoObject();

                // Get username and password from API server
                parameters["route"] = "client/info";               
                results = Global.api.Request(parameters);
                if(results.status == "success")
                {
                    Console.WriteLine(results.data.username);
                    return true;
                } else
                {
                    Global.AddLog("Lỗi lấy username, pass từ server. Nội dung: " + results.message);
                    return false;
                }
            }
            
            return true;
        }
    }
}
