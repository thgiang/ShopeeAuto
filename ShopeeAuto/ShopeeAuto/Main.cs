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
using Newtonsoft.Json.Linq;
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
            string jobName = "checkorder";
            while (true)
            {
                try
                {
                    if (doneAllJob)
                    {
                        if (jobName == "checkorder") { jobName = "listing"; } else if (jobName == "listing") { jobName = "updating"; } else { jobName = "checkorder"; };
                        Global.AddLog("Đã thực hiện xong công việc, chuẩn bị lấy việc mới");
                        Jobs.Clear(); /// Xoa sach job cu

                        // Lấy sản phẩm cần list
                        if (jobName == "listing")
                        {
                            Dictionary<string, string> parameters = new Dictionary<string, string>
                        {
                            { "route", "product" },
                            { "action", "list" },
                            { "limit", "5" }
                        };

                            ApiResult apiResult;
                            apiResult = Global.api.RequestMyApi(parameters);
                            if (!apiResult.success)
                            {
                                Global.AddLog("ERROR: Lỗi lấy job từ server");
                                Thread.Sleep(5000);
                                continue;
                            }
                            dynamic xx = JsonConvert.DeserializeObject<dynamic>(apiResult.content);
                            var dataArray = xx["data"];
                            List<NSApiProduct.ProductList> requestResults = dataArray.ToObject<List<NSApiProduct.ProductList>>();
                            foreach (NSApiProduct.ProductList element in requestResults)
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
                        else if (jobName == "updating")
                        {

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
                        doneAllJob = false;
                    }
                    Thread.Sleep(2000);
                } catch
                {
                    Thread.Sleep(2000);
                    continue;
                }
            }
        }

        // Thực hiện công việc
        private void SWorker()
        {
            #region Kiểm tra login
            bool isLoggedIn = false;
            while (!isLoggedIn) {
                try
                {
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
                            if (Shopee.GetShopId() == "")
                            {
                                Global.AddLog("ERROR: Không lấy đc giá trị shopId");
                                // TODO: Gọi lên server báo lỗi đăng nhập
                                Thread.Sleep(30000);
                                continue;
                            }
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
                   
                }
                catch (Exception e)
                {
                    Global.AddLog("ERROR: Lỗi khi đăng nhập " + e.Message);
                    Thread.Sleep(2000);
                    continue;
                }
                if (isLoggedIn)
                {
                    try
                    {
                        IWebElement elementName = Global.driver.FindElement(By.Id("auth_content"));
                        if (elementName != null)
                        {
                            Global.authToken = elementName.Text;
                            Global.AddLog("Auth token = " + Global.authToken);
                        }
                    }
                    catch
                    {
                        Global.AddLog("ERROR: Lỗi ko lấy đc Auth Token");
                        isLoggedIn = false;
                        continue;
                    }
                }
            }
           
            #endregion

            while (true)
            {
                try {
                // BẮT ĐẦU CÔNG VIỆC CHÍNH
                foreach (QueueElement job in Jobs.ToList())
                {
                    
                    if(job.jobStatus == "waiting")
                    {
                        // Xử lý product chờ được list
                        if (job.jobName == "listing")
                        {
                            // TẠM ĐẶT NHƯ NÀY ĐỂ TEST THÔI, CHUYỂN SANG VIỆC KHÁC
                            job.jobStatus = "done";
                            continue;

                            NSApiProduct.ProductList jobData = job.jobData;
                            Global.AddLog("Đang làm việc trên job " + jobData.Id);
                            #region Tìm sản phẩm rẻ nhất taobao ở taobao
                            // Kiểm tra xem mình có phải shop phụ trách chính ko. Nếu đúng thì phải scan tất cả các taobaos, nếu ko thì chỉ  cần làm đúng công việc của mình là scan cái thằng is_the_best
                            // Lấy data từ Taobao
                            float minTaobaoPrice = 999999;
                            NSTaobaoProduct.TaobaoProduct taobaoProductInfo = new NSTaobaoProduct.TaobaoProduct();
                            if (jobData.Shops.First().IsPrimary)
                            {
                                Global.AddLog("Đang tìm thằng taobao nào bán rẻ nhất");
                                // Nếu là primary thì phải scan lại tất cả taobaos để xem bây giờ thằng nào đang rẻ nhất.
                                foreach (NSApiProduct.TaobaoId taobaoIdObj in jobData.TaobaoIds)
                                {
                                    Global.AddLog("Quét qua taobaoId " + taobaoIdObj.ItemId);
                                    ApiResult apiResult;
                                    apiResult = Global.api.RequestOthers("https://h5api.m.taobao.com/h5/mtop.taobao.detail.getdetail/6.0/?appKey=12574478&t=1583495561716&api=mtop.taobao.detail.getdetail&ttid=2017%40htao_h5_1.0.0&data=%7B%22exParams%22%3A%22%7B%5C%22countryCode%5C%22%3A%5C%22CN%5C%22%7D%22%2C%22itemNumId%22%3A%22" + taobaoIdObj.ItemId.ToString() + "%22%7D", Method.GET);
                                    Thread.Sleep(600); // Request nhanh quá bị taobao chặn
                                    if (apiResult.success)
                                    {
                                        NSTaobaoProduct.TaobaoProduct tbp = JsonConvert.DeserializeObject<NSTaobaoProduct.TaobaoProduct>(apiResult.content);
                                        // JSON decode thêm 1 lần nữa vì nó là JSON bên trong của JSON cha

                                        if (tbp.Data.ApiStack == null)
                                        {
                                            // SẢN PHẨM NÀY CHẢ CÓ THÔNG TIN MẸ GÌ CẢ, BÁN CŨNG KHÓ. THÔI NEXT
                                            continue;
                                        }

                                        tbp.Data.Details = JsonConvert.DeserializeObject<NSTaobaoProductDetail.TaobaoProductDetails>(tbp.Data.ApiStack.First().Value);
                                        // Tìm đc thằng rẻ hơn

                                        string[] tempPrices = tbp.Data.Details.Price.Price.PriceText.Split('-');
                                        float tempPrice = float.Parse(tempPrices[0]);
                                        if (tbp.Data.Details.Price.ExtraPrices != null && tbp.Data.Details.Price.ExtraPrices.Count > 0)
                                        {
                                            tempPrices = tbp.Data.Details.Price.ExtraPrices.First().PriceText.Split('-');
                                            tempPrice = float.Parse(tempPrices[0]);
                                        }
                                        if (tbp.Data.Details.Price.NewExtraPrices != null && tbp.Data.Details.Price.NewExtraPrices.Count > 0)
                                        {
                                            tempPrices = tbp.Data.Details.Price.NewExtraPrices.First().PriceText.Split('-');
                                            tempPrice = float.Parse(tempPrices[0]);
                                        }
                                        if (tempPrice < minTaobaoPrice)
                                        {
                                            taobaoProductInfo = tbp;
                                            minTaobaoPrice = tempPrice;
                                        }
                                    }
                                }

                                if (minTaobaoPrice == 999999)
                                {
                                    // Nếu ko tìm đc thằng rẻ nhất thì job này fail
                                    Global.AddLog("ERROR: Lỗi không tìm đc sản phẩm rẻ nhất");
                                    // Chạy job tiếp theo
                                    continue;
                                }
                                else
                                {
                                    // Tìm được thằng rẻ nhất thì báo về server để update
                                    Dictionary<string, string> parameters = new Dictionary<string, string>
                                {
                                    { "route", "product/"+jobData.Id },
                                    { "source", "taobao" },
                                    { "item_id",  taobaoProductInfo.Data.Item.ItemId},
                                    { "action", "update_best_id" }
                                };
                                    ApiResult apiResult = Global.api.RequestMyApi(parameters, Method.PUT);
                                    if (apiResult.success)
                                    {
                                        Global.AddLog("Đã gửi thông tin update is_the_best lên server " + taobaoProductInfo.Data.Item.ItemId);
                                    }
                                    else
                                    {
                                        Global.AddLog("Lỗi khi gửi thông tin is_the_best lên server nhưng mà mình kệ nó hihi, cứ chạy tiếp");
                                    }
                                }
                            }
                            else
                            {
                                // Nếu ko phải primay thì cứ lấy thằng isTheBest
                                foreach (NSApiProduct.TaobaoId taobaoIdObj in jobData.TaobaoIds)
                                {
                                    if (taobaoIdObj.IsTheBest)
                                    {
                                        ApiResult apiResult;
                                        apiResult = Global.api.RequestOthers("https://h5api.m.taobao.com/h5/mtop.taobao.detail.getdetail/6.0/?appKey=12574478&t=1583495561716&api=mtop.taobao.detail.getdetail&ttid=2017%40htao_h5_1.0.0&data=%7B%22exParams%22%3A%22%7B%5C%22countryCode%5C%22%3A%5C%22CN%5C%22%7D%22%2C%22itemNumId%22%3A%" + taobaoIdObj.ItemId.ToString() + "%22%7D", Method.GET);
                                        if (apiResult.success)
                                        {
                                            NSTaobaoProduct.TaobaoProduct tbp = JsonConvert.DeserializeObject<NSTaobaoProduct.TaobaoProduct>(apiResult.content);
                                            // JSON decode thêm 1 lần nữa vì nó là JSON bên trong của JSON cha
                                            tbp.Data.Details = JsonConvert.DeserializeObject<NSTaobaoProductDetail.TaobaoProductDetails>(tbp.Data.ApiStack.First().Value);
                                            // Tìm đc thằng rẻ hơn
                                            if (float.Parse(tbp.Data.Details.Price.ExtraPrices.First().PriceText) < minTaobaoPrice)
                                            {
                                                taobaoProductInfo = tbp;
                                                minTaobaoPrice = float.Parse(tbp.Data.Details.Price.ExtraPrices.First().PriceText);
                                            }
                                        }
                                        else
                                        {
                                            Global.AddLog("ERROR: Lỗi không lấy đc thông tin sản phẩm taobao. Chuyển sang job tiếp theo");
                                            continue;
                                        }
                                    }
                                }
                            }


                            #endregion

                            // Tactic = 0 nghia la tim tu taobao ve, tactic = 1 hoac 2 nghia la copy tu shopee
                            if (jobData.Tactic.Value != 0)
                            {
                                #region Lấy data từ Shopee
                                Global.AddLog("Lấy data từ Shopee");
                                NSShopeeProduct.ShopeeProduct shopeeProductInfo = Shopee.GetShopeeProductData(jobData.ShopeeIds.First().ItemId, jobData.ShopeeIds.First().ShopId);
                                if (shopeeProductInfo == null)
                                {
                                    Dictionary<string, string> parameters = new Dictionary<string, string>
                                {
                                    { "route", "product/"+jobData.Id },
                                    { "source", "taobao" },
                                    { "shopee_item_id",  "CANNOT_READ_INFO"},
                                    { "shopee_shop_id",  Global.myShopId},
                                    { "shopee_price",  "0"},
                                    { "shopee_model_list",  ""},
                                    { "action", "list_done" }
                                };
                                    Global.api.RequestMyApi(parameters, Method.PUT);

                                    Global.AddLog("ERROR: Lỗi lấy thông tin sản phẩm shopeeId " + jobData.ShopeeIds.First().ItemId + ", shopId " + jobData.ShopeeIds.First().ShopId);
                                    continue;
                                };
                                Global.AddLog("Lấy data từ Shopee xong");
                                #endregion

                                if (taobaoProductInfo != null)
                                {
                                    Shopee.CopyTaobaoToShopee(taobaoProductInfo, shopeeProductInfo, jobData);
                                }
                                else
                                {
                                    Global.AddLog("ERROR: Thật ra theo logic code của mình thì dòng này sẽ ko bao giờ đc gọi tới, bởi vì đi tới đây thì taobaoProductInfo đã khác null rồi");
                                    continue;
                                }

                            }
                            // Ngược lại thì copy thông tin từ Taobao rồi dịch
                            else
                            {
                                // Gọi hàm Shopee.CopyTaobaoToShopee2 trong hàm này phải dịch content tiếng TQ trước khi post thay vì copy toàn bộ thông tin của đối thủ shopee như hàm trước.
                            }

                        }

                        // Xử lý product chờ được update

                        // Xử lý đơn đặt hàng
                        if(job.jobName == "checkorder")
                        {
                            Global.AddLog("==============\nBẮT ĐẦU KIỂM TRA ORDER MỚI\n=============");
                            Global.AddLog("Lấy order cũ nhất trên server");
                            Shopee.ProcessNewCheckout();
                        }
                        job.jobStatus = "done"; // Lỗi hay gì thì cũng phải done thôi để còn làm việc khác, hehe
                    }
                }
                doneAllJob = true;
                Thread.Sleep(500);
                } catch
                {
                    Thread.Sleep(1000);
                    continue;
                }
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

          
            /*
            ApiResult apiResult;
            apiResult = Global.api.RequestOthers("https://translate.google.com/translate_a/t?v=2.0&hl=vi&ie=UTF-8&client=at&sc=1&oe=UTF-8&text=珍珠白送充电硅胶洁面刷&sl=zh-CN&tl=vi", Method.POST);
            if (apiResult.success)
            {
                Global.AddLog(apiResult.content);
            }
            */

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

        private void txtDebug_TextChanged(object sender, EventArgs e)
        {
            txtDebug.SelectionStart = txtDebug.Text.Length;
            // scroll it automatically
            txtDebug.ScrollToCaret();
        }
    }
}
