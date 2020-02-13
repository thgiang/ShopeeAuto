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

        public Main()
        {
            InitializeComponent();
        }

        private void SWorker()
        {
  
        }

        private void SApi()
        {
            // Đăng nhập Shopee và lấy cookie
            Shopee.Login();

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

            // Create threads
            Thread threadListing = new Thread(new ThreadStart(SApi));
            threadListing.Start();

            Thread threadUpdate = new Thread(new ThreadStart(SWorker));
            threadUpdate.Start();
        }
    }
}
