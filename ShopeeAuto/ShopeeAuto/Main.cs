using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSApiProducts;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using RestSharp;
using RestSharp.Extensions;

namespace ShopeeAuto
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        // Lấy việc mới từ API
        private List<QueueElement> SApi(string myAccountId, string jobName)
        {
            List<QueueElement> Jobs = new List<QueueElement>();
            try
            {
                Global.AddLog("Đã thực hiện xong công việc cũ, chuẩn bị lấy việc mới");
                Jobs.Clear(); /// Xoa sach job cu

                Global.AddLog("JOBNAME: " + jobName);
                // Lấy sản phẩm cần list
                if (jobName == "list" || jobName == "update" || jobName == "random")
                {
                    Dictionary<string, string> parameters = new Dictionary<string, string>
                    {
                        { "route", "product" },
                        { "action", jobName },
                        { "per_page", "5" },
                        { "account_id", myAccountId },
                    };

                    ApiResult apiResult;
                    apiResult = Global.api.RequestMyApi(parameters);
                    if (!apiResult.success)
                    {
                        Global.AddLog("ERROR: Lỗi lấy job từ server");
                        Thread.Sleep(5000);
                    }
                    NSApiProducts.NsApiProducts result = JsonConvert.DeserializeObject<NSApiProducts.NsApiProducts>(apiResult.content);
                    List<NSApiProducts.NsApiProduct> requestResults = result.Data;

                    if(requestResults != null) { 
                        foreach (NSApiProducts.NsApiProduct element in requestResults)
                        {
                            Global.AddLog("Đã thêm vào hàng đợi job: " + element.Id);
                            QueueElement job = new QueueElement
                            {
                                jobName = jobName,
                                jobStatus = "waiting",
                                jobData = element
                            };
                            Jobs.Add(job);
                        }
                    }
                    else
                    {
                        Global.AddLog("Không có job nào cần " + jobName);

                    }
                }
                else if (jobName == "checkorder")
                {
                    Global.AddLog("Đã thêm vào hàng đợi job kiểm tra order mới");
                    QueueElement job = new QueueElement
                    {
                        jobName = jobName,
                        jobStatus = "waiting",
                        jobData = null
                    };
                    Jobs.Add(job);
                }
                return Jobs;
            }
            catch (Exception e)
            {
                if (Global.DebugMode)
                {
                    MessageBox.Show("Lỗi khi lấy job từ server " + e.Message);
                    return Jobs;
                }
                else
                {
                    Global.AddLog("Lỗi khi lấy job từ server " + e.Message + e.StackTrace);
                    Thread.Sleep(2000);
                    return Jobs;
                }
            }
            
        }

        

        private ChromeDriver InitDriver(string proxy)
        {
            try
            {
                Random rd = new Random();
                string remote_port = rd.Next(9000, 9999).ToString();
                // Mở chorme thật qua
                Process process = new Process();
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = "chrome";
                process.StartInfo.Arguments = "https://salehunter.net/my-ip  --load-extension=\"C:\\Users\\Admin\\Desktop\\no_cookies\" --disable-gpu --new-window --remote-debugging-port=" + remote_port + " --user-data-dir=\"C:\\Profile\" --proxy-server=\""+proxy+"\" --disable-infobars --disable-notifications --window-size=1366,768"; //--window-position=0,0 --window-size=1200,800 --disable-images  
                process.Start();

                Thread.Sleep(1000);
                var options = new ChromeOptions { DebuggerAddress = "localhost:" + remote_port };         
                ChromeDriverService sv = ChromeDriverService.CreateDefaultService();
                sv.HideCommandPromptWindow = true;
                var driver = new ChromeDriver(sv, options);

                // Đóng tất cả tab đang mở
                while (true)
                {
                    var tabs = driver.WindowHandles;
                    if (tabs.Count > 1)
                    {
                        try
                        {
                            driver.SwitchTo().Window(tabs[1]);
                            driver.Close();
                            driver.SwitchTo().Window(tabs[0]);
                        }
                        catch { }
                    }
                    else
                    {
                        break;
                    }
                }


                // Fake thông tin trình duyệt tránh detect auto
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("Object.defineProperty(navigator, 'webdriver', { get: () => undefined })");
                driver.Navigate().GoToUrl("https://google.com");
                return driver;
            }
            catch(Exception ex) {
               MessageBox.Show("ERROR: Lỗi khởi tạo Google Chrome Driver "+ex.Message + ex.StackTrace);
                return null;
            }
           
        }

        [Obsolete]
        public bool DownloadChromeDriver()
        {
            foreach (var process in Process.GetProcessesByName("chromedriver"))
            {
                process.Kill();
            }
            Thread.Sleep(500);
            File.Delete(Application.StartupPath + "\\chromedriver.exe");
            Thread.Sleep(500);
            try
            {
                object path;
                path = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe", "", null);
                if (path != null)
                {
                    string chromeVersion = FileVersionInfo.GetVersionInfo(path.ToString()).FileVersion;
                    int last_point = chromeVersion.LastIndexOf('.');
                    if (last_point > 0)
                    {
                        chromeVersion = chromeVersion.Substring(0, last_point);
                        var client = new RestClient("https://chromedriver.storage.googleapis.com/LATEST_RELEASE_" + chromeVersion);
                        var request = new RestRequest(Method.GET);
                        IRestResponse response = client.Execute(request);
                        if(!response.Content.Contains(chromeVersion))
                        {
                            return false;
                        }


                        string driverVersion = response.Content;
                        if (!File.Exists(Application.StartupPath + "\\chromedriver_win32_" + driverVersion + ".zip"))
                        {
                            client = new RestClient("https://chromedriver.storage.googleapis.com/" + driverVersion + "/chromedriver_win32.zip");
                            client.DownloadData(request).SaveAs(Application.StartupPath + "\\chromedriver_win32_" + driverVersion + ".zip");
                            ZipFile.ExtractToDirectory(Application.StartupPath + "\\chromedriver_win32_" + driverVersion + ".zip", Application.StartupPath);
                            return true;
                        }
                        else
                        {
                            ZipFile.ExtractToDirectory(Application.StartupPath + "\\chromedriver_win32_" + driverVersion + ".zip", Application.StartupPath);
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if(Global.DebugMode)
                {
                    MessageBox.Show("Lỗi tải driver "+e.Message + e.StackTrace);
                }
                return false;
            }
           
            return false;
        }

        [Obsolete]
        private void Main_Load(object sender, EventArgs e)
        {
            if (!DownloadChromeDriver())
            {
                MessageBox.Show("Không tải đc chromedriver.exe mới nhất, chương trình sẽ tắt");
                return;
            }
            //string path = Helper.GenCaptcha("123456");
            //txtDebug.Text = path;
            //return;


            /*
            ApiResult apiResult;
            apiResult = Global.api.RequestOthers("https://translate.google.com/translate_a/t?v=2.0&hl=vi&ie=UTF-8&client=at&sc=1&oe=UTF-8&text=珍珠白送充电硅胶洁面刷&sl=zh-CN&tl=vi", Method.POST);
            if (apiResult.success)
            {
                Global.AddLog(apiResult.content);
            }
            */
            // Cho txtDebug làm biến Global
            Global.txtDebug = this.txtDebug;

            // Tìm file cấu hình và giá trị
            if (String.IsNullOrEmpty(Global.api.apiUrl) || String.IsNullOrEmpty(Global.api.accessToken))
            {
                MessageBox.Show("Please put " + System.AppDomain.CurrentDomain.FriendlyName + ".config in same folder");
                Application.Exit();
            }


            // Thread này thực hiện các công việc đang có và báo ngược về Server kết quả
            //Thread threadListing = new Thread(SWorker);
            //threadListing.Start();

            for(int i = 0; i < 1; i++)
            {
                Thread threadAcc = new Thread(() => threadAcc1("5eb50446def4ed287c555892"));
                threadAcc.Start();
            }
        }

        public void threadAcc1(string myAccountId)
        {
            string proxy = "";
            // Khởi tạo Driver
            ChromeDriver driver = null;
            int i = 0;
            do {
                Global.AddLog("Đang khởi tạo Driver lần " + (i+1).ToString());
                driver = InitDriver(proxy);
                Thread.Sleep(1000);
            } while (driver == null);

                // Lấy Job và thực hiện
            ShopeeWorker shopeeWorker = new ShopeeWorker(driver, myAccountId, proxy);
            do
            {
                List<QueueElement> jobs = SApi(myAccountId, "list");
                shopeeWorker.RunNow(jobs, driver);
            } while (true);
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Global.driver.Quit();
            //Environment.Exit(Environment.ExitCode);
            //Application.Exit();
        }

        private void txtDebug_TextChanged(object sender, EventArgs e)
        {
            txtDebug.SelectionStart = txtDebug.Text.Length;
            // scroll it automatically
            txtDebug.ScrollToCaret();
        }
    }
}
