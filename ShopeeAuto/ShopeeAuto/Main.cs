using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
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


        public Main()
        {
            InitializeComponent();
        }

        private void SApi()
        {
            QueueElement job = new QueueElement();
            job.jobName = "hihi";
            job.jobStatus = "haha";
            job.jobData = "data";
            Jobs.Add(job);
        }

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
                    Global.AddLog(job.jobName);
                }
                Thread.Sleep(500);
            }
            
        }


        private void Main_Load(object sender, EventArgs e)
        {
            // Cho txtDebug làm biến Global
            Global.txtDebug = this.txtDebug;

            // Init web driver
            Global.InitDriver();

            // Tìm file cấu hình và giá trị
            if (String.IsNullOrEmpty(Global.api.apiUrl) || String.IsNullOrEmpty(Global.api.accessToken))
            {
                MessageBox.Show("Please put " + System.AppDomain.CurrentDomain.FriendlyName + ".config in same folder");
                Application.Exit();
            }

            // Khởi tạo ShopeeWorker
            Shopee = new ShopeeWorker();

            
            SApi();
            timerApi.Start();
            // Thread này định kì gọi lên API tổng để xin công việc mới
            //Thread threadListing = new Thread(new ThreadStart(SApi));
            //threadListing.Start();

            // Thread này thực hiện các công việc đang có và báo ngược về Server kết quả
            Thread threadUpdate = new Thread(new ThreadStart(SWorker));
            threadUpdate.Start();
        }

        private void timerApi_Tick(object sender, EventArgs e)
        {
            SApi();
        }
    }
}
