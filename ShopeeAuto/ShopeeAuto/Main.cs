using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace ShopeeAuto
{
    public partial class Main : Form
    {
        // ShopeeWorker
        private ShopeeWorker Shopee;

        // Danh sách job
        class QueueElement
        {
            public string jobName;
            public string jobStatus = "WAITING";
            public dynamic jobData;
        }
        List<QueueElement> Jobs = new List<QueueElement>();
        bool doneAllJob = true;

        public Main()
        {
            InitializeComponent();
        }

        // Lấy việc mới từ API
        private void SApi()
        {
            while (true)
            {
                if (doneAllJob)
                {
                    Jobs.Clear(); /// Xoa sach job cu

                    Dictionary<string, string> parameters = new Dictionary<string, string>();
                    parameters.Add("route", "product");
                    parameters.Add("action", "list");
                    parameters.Add("limit", "1");

                    dynamic requestResults = new ExpandoObject();
                    requestResults = Global.api.Request(parameters);

                    foreach (dynamic element in requestResults)
                    {
                        Global.AddLog("Da add cong viec " + element._id);
                        QueueElement job = new QueueElement();
                        job.jobName = "listing";
                        job.jobStatus = "waiting";
                        job.jobData = element;
                        Jobs.Add(job);
                    }
                    doneAllJob = false;
                }
                Thread.Sleep(10000);
            }
        }

        // Thực hiện công việc
        private void SWorker()
        {
            #region Kiểm tra login
            bool isLoggedIn = false;
            // Đăng nhập Shopee và lấy cookie
            for (int i = 0; i < 3; i++)
            {
                isLoggedIn = Shopee.Login();
                if (!isLoggedIn)
                {
                    // TODO: Gọi lên server báo lỗi đăng nhập
                    Thread.Sleep(30000);
                    continue;
                }
                else
                {
                    break;
                }
            }
            // Nếu quá 3 lần mà vẫn login ko thành công thì dừng chương trình
            if (!isLoggedIn)
            {
                // TODO: GỬI REPORT LÊN SERVER
                Global.AddLog("Đăng nhập thất bại quá nhiều lần. Chương trình sẽ dừng để tránh bị block IP");
                MessageBox.Show("Đăng nhập thất bại quá nhiều lần. Chương trình sẽ dừng để tránh bị block IP");
                Application.Exit();
            }
            #endregion

            while (true)
            {
                // BẮT ĐẦU CÔNG VIỆC CHÍNH
                foreach (QueueElement job in Jobs)
                {
                    // Xử lý product chờ được list
                    if(job.jobStatus == "waiting")
                    {
                        Global.AddLog("Đang up sản phẩm " + job.jobData._id);
                        dynamic postMe = new ExpandoObject(); // Thông tin sẽ đăng lên Shopee

                        // Tactic = 0 nghia la tim tu taobao ve, tactic = 1 hoac 2 nghia la copy tu shopee
                        if (job.jobData.tactic != 0)
                        {

                            // Lấy thông tin cái đã ^^
                            dynamic cloneMe = Shopee.GetProductData((string)job.jobData.shopee_ids[0].item_id, (string)job.jobData.shopee_ids[0].shop_id);
                            if (cloneMe == null)
                            {
                                job.jobStatus = "error";
                                // TODO: BÁO LÊN SERVER, LỖI KHÔNG LẤY ĐƯỢC THÔNG TIN SẢN PHẨM
                            };

                            postMe.catid = cloneMe.categories[2].catid;
                            postMe.name = cloneMe.name;
                            postMe.price = "50000";

                            dynamic maxProfitItem;
                            float maxProfitPercent = 0;
                            foreach(dynamic taobao_item in job.jobData.taobao_ids)
                            {
                                Global.AddLog("Dang quet qua taobao ID: " + taobao_item.item_id );
                                if(taobao_item.profit.percent > maxProfitPercent)
                                {
                                    maxProfitPercent = taobao_item.profit.percent;
                                    maxProfitItem = taobao_item;
                                }
                            }
                            Global.AddLog("Tim ra ngon nhat, ti le la " + maxProfitPercent);
                            // Map thông tin từ cloneMe sang postMe
                            /*
                            Global.AddLog("Lấy data sản phẩm thành công \n\n");
                            Global.AddLog("Mã ngành hàng: " + results.categories[2].catid + "\n");
                            Global.AddLog("Tên sản phẩm: " + results.name + "\n");
                            Global.AddLog("Mô tả sản phẩm: " + results.description + "\n");
                            Global.AddLog("Giá bán hiện tại: " + results.price + "\n");
                            Global.AddLog("Tổng số đã bán: " + results.historical_sold + "\n");
                            Global.AddLog("Thuộc tính sản phẩm: " + results.attributes + "\n");
                            Global.AddLog("Phân loại sản phẩm: " + results.models + "\n");
                            */

                        }
                        // Ngược lại thì copy thông tin từ Taobao rồi dịch
                        else
                        {

                        }


                        job.jobStatus = "done";
                    }
                   
                    // Xử lý product chờ được update
                }
                doneAllJob = true;
                Thread.Sleep(500);
            }
            
        }


        private void Main_Load(object sender, EventArgs e)
        {
            // Cho txtDebug làm biến Global
            Global.txtDebug = this.txtDebug;

            // Tìm file cấu hình và giá trị
            if (String.IsNullOrEmpty(Global.api.apiUrl) || String.IsNullOrEmpty(Global.api.accessToken))
            {
                MessageBox.Show("Please put " + System.AppDomain.CurrentDomain.FriendlyName + ".config in same folder");
                Application.Exit();
            }

            // Khởi tạo ShopeeWorker
            Shopee = new ShopeeWorker();

            

            // Thread này định kì gọi lên API tổng để xin công việc mới
            Thread threadUpdate= new Thread(SApi);
            threadUpdate.Start();

            // Thread này thực hiện các công việc đang có và báo ngược về Server kết quả
            Thread threadListing = new Thread(SWorker);
            threadListing.Start();
        }

        private void timerApi_Tick(object sender, EventArgs e)
        {
            SApi();
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Global.driver.Quit();
            Environment.Exit(Environment.ExitCode);
            Application.Exit();
        }
    }
}
