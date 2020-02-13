using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Dynamic;
using SeleniumExtras.WaitHelpers;

namespace ShopeeAuto
{
    class ShopeeWorker
    {
        public bool Login()
        {

            bool needToLogin = false;
            Global.AddLog("Kiểm tra Shopee đã đăng nhập chưa");
            Global.driver.Navigate().GoToUrl("https://banhang.shopee.vn/account/signin");

            // Chờ tối đa 5s xem có thấy form login hay không.
            try
            {
                Global.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("form.signin-form")));
                Global.AddLog("Chưa đăng nhập, đang đăng nhập");
                needToLogin = true;
            } catch
            {
                Global.AddLog("Đã đăng nhập");
                return true;
            }

            // Nếu có form login thì lấy thông tin username và pass từ server
            if(needToLogin)
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                dynamic results = new ExpandoObject();

                // Lấy thông tin username và pass từ server
                parameters["route"] = "client/info";               
                results = Global.api.Request(parameters);

                // Lấy được user và pass, tiến hành login vào shopee
                if(results.status == "success")
                {
                    IWebElement loginForm = Global.driver.FindElement(By.CssSelector("form.signin-form"));
                    loginForm.FindElements(By.TagName("input"))[0].SendKeys(results.data.username.ToString()); // Username
                    loginForm.FindElements(By.TagName("input"))[1].SendKeys(results.data.password.ToString()); // Password
                    loginForm.FindElement(By.ClassName("shopee-checkbox__indicator")).Click(); // Remember me
                    loginForm.FindElement(By.ClassName("shopee-button--primary")).Click(); // Login now

                    // Check lại xem có lỗi gì khi đăng nhập ko, nếu có thì hiển thị, nếu ko thì login thành công
                    try
                    {
                        if(Global.wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName(" route-index"))).Text != "")
                        {
                            string loginError = loginForm.FindElement(By.ClassName("login-error")).Text;
                            Global.AddLog("Đăng nhập lỗi: " + loginError);
                            return false;
                        }
                    } catch
                    {
                        Global.AddLog("Đăng nhập thành công");
                    }
                    return true;
                }
                // Lỗi khi gọi lên server lấy username, pass
                else
                {
                    Global.AddLog("Lỗi lấy username, pass từ server. Nôi: " + results.message);
                    return false;
                }
            }

            return true;
        }
    }
}
