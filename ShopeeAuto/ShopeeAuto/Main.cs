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
using RestSharp;

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
                Thread.Sleep(1000000);
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
                    Global.AddLog("Đăng nhập thất bại lần thứ " + i.ToString());
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
                            // Lấy thông tin từ shopee cái đã
                            dynamic cloneMe = Shopee.GetProductData((string)job.jobData.shopee_ids[0].item_id, (string)job.jobData.shopee_ids[0].shop_id);
                            if (cloneMe == null)
                            {
                                job.jobStatus = "error";
                                // TODO: BÁO LÊN SERVER, LỖI KHÔNG LẤY ĐƯỢC THÔNG TIN SẢN PHẨM
                            };

                            // Copy thông tin của đối thủ shopee
                            postMe.catId = cloneMe.categories[2].catId;
                            postMe.name = cloneMe.name;
                            // Lần đầu list thì cứ cho giá rẻ hơn đối thủ 1k, còn từ sau đó tính toán sau.
                            postMe.price = cloneMe.price - 1000;

                            // Lấy ra taobao có profit tốt nhất
                            dynamic maxProfitItem = new ExpandoObject();
                            float maxProfitPercent = 0;
                            foreach(dynamic taobao_item in job.jobData.taobao_ids)
                            {
                                if(taobao_item.profit.percent > maxProfitPercent)
                                {
                                    maxProfitPercent = taobao_item.profit.percent;
                                    maxProfitItem = taobao_item;
                                }
                            }

                            // Kiểm tra số lượng bên TQ còn đang bán và copy ảnh của nó.
                            //Lấy ảnh của sản phẩm từ Taobao
                            var client = new RestClient("https://laonet.online/index.php?route=api_tester/call&api_name=item_get&lang=vi&num_iid=" + maxProfitItem.item_id.ToString() + "&key=profile.nvt@gmail.com");
                            ; client.Timeout = -1;
                            var request = new RestRequest(Method.GET);
                            IRestResponse response = client.Execute(request);
                            dynamic results = JsonConvert.DeserializeObject<dynamic>(response.Content);
                            dynamic taobaoProductInfo = results.item;

                            Shopee.PublishOnlyOneProduct(cloneMe, taobaoProductInfo);


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

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            Global.driver.Quit();
            Environment.Exit(Environment.ExitCode);
            Application.Exit();
        }
    }
}
