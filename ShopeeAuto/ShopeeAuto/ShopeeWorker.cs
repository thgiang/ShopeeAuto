using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Dynamic;
using SeleniumExtras.WaitHelpers;
using RestSharp;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using Newtonsoft.Json.Serialization;
using System.Windows.Forms;
using System.Drawing;
using Keys = OpenQA.Selenium.Keys;
using System.Collections.ObjectModel;

namespace ShopeeAuto
{
    class ShopeeWorker
    {
        List<string> notImportantWords = new List<string> { "建议", "【", "】", "清仓", "元"};
        private Helper helper = new Helper();
        private ReadOnlyCollection<OpenQA.Selenium.Cookie> shopeeCookie;
        private string SPC_CDS = "GI_CUNG_DUOC";
        private int minRevenueInMoney = 30000;
        private int minRevenue = 20;
        private int maxRevenue = 50;
        private string username;
        private string password;
        public bool Login()
        {
            ApiResult result;
            // Lấy thông tin username và pass từ server
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                ["route"] = "client/info"
            };
            result = Global.api.RequestMyApi(parameters);
            // Lấy được user và pass, tiến hành login vào shopee
            if (result.success)
            {
                NSClientInfo.ClientInfo client = JsonConvert.DeserializeObject<NSClientInfo.ClientInfo>(result.content);
                minRevenue = int.Parse(client.Data.ShopeeMinRevenue.ToString());
                maxRevenue = int.Parse(client.Data.ShopeeMaxRevenue.ToString());
                // username = client.Data.ShopeeUsername.ToString();
                // password = client.Data.ShopeePassword.ToString();
                // Global.myAccountId = client.Data.Id.ToString();
                Global.myAccountId = "5e6cc1832a895c6611691942";
                username = "nhutam9045";
                password = "Tapphelu@123627";
            }
            // Lỗi khi gọi lên server lấy username, pass
            else
            {
                Global.AddLog("STOP. Lỗi lấy username, pass từ server.");
                return false;
            }

            bool needToLogin = false;
            Global.AddLog("Kiểm tra Shopee đã đăng nhập chưa");
            Global.driver.Navigate().GoToUrl("https://banhang.shopee.vn/account/signin");

            
            for (int i = 0; i < 3; i++)
            {
                Thread.Sleep(2000);
                try
                {
                    string loggedInUser = Global.driver.FindElement(By.ClassName("user-info")).Text;
                    if (loggedInUser == username)
                    {
                        Global.AddLog("Đăng nhập thành côngggg");
                        shopeeCookie = Global.driver.Manage().Cookies.AllCookies;
                        foreach(var x in shopeeCookie)
                        {
                            if(x.Name == "SPC_CDS")
                            {
                                SPC_CDS = x.Value;
                            }
                        }
                        return true;
                    }
                }
                catch
                {
                    continue;
                }
            }

            // Chờ tối đa 10 xem có thấy form login hay không.
            try
            {
                Global.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("form.signin-form")));
                Global.AddLog("Chưa đăng nhập, đang đăng nhập");
                needToLogin = true;
            } catch
            {
                Global.AddLog("Đã đăng nhập");
                // Lấy cookie trước khi return Login thành công
                shopeeCookie = Global.driver.Manage().Cookies.AllCookies;
                return true;
            }
            
            /*
            // Gọi lên địa chỉ check thông tin của người đang login
            ApiResult apiResult;
            apiResult = Global.api.RequestOthers("https://banhang.shopee.vn/api/v2/login/", Method.GET, shopeeCookie);
            if (apiResult.success)
            {
                dynamic results = JsonConvert.DeserializeObject<dynamic>(apiResult.content);
                if (results.shopid == null)
                {
                    needToLogin = true;
                }
                else
                {
                    Global.myShopId = results.shopid;
                }
            }
            */
            // Nếu có form login thì lấy thông tin username và pass từ server
            if(needToLogin)
            {
                IWebElement loginForm = Global.driver.FindElement(By.CssSelector("form.signin-form"));
                for (int i = 0; i < 30; i++)
                {
                    loginForm.FindElements(By.TagName("input"))[0].SendKeys(Keys.Backspace); // Username
                    Thread.Sleep(20);
                }
                loginForm.FindElements(By.TagName("input"))[0].SendKeys(username); // Username


                for (int i = 0; i < 30; i++)
                {
                    loginForm.FindElements(By.TagName("input"))[1].SendKeys(Keys.Backspace); // Password
                }
        
                loginForm.FindElements(By.TagName("input"))[1].SendKeys(password + "\n"); // Password

                //loginForm.FindElement(By.ClassName("shopee-checkbox__indicator")).Click(); // Remember me
                //loginForm.FindElement(By.ClassName("shopee-button--primary")).Click(); // Login now

                // Check lại xem đăng nhập thành công chưa
                for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(2000);
                    try { 
                        string loggedInUser = Global.driver.FindElement(By.ClassName("user-info")).Text;
                        if (loggedInUser == username)
                        {
                            Global.AddLog("Đăng nhập thành côngggg");
                            shopeeCookie = Global.driver.Manage().Cookies.AllCookies;
                            return true;
                        }
                    } catch
                    {
                        continue;
                    }
                }
                return false;
            }
            // Lấy cookie trước khi return Login thành công
            shopeeCookie = Global.driver.Manage().Cookies.AllCookies;
            return true;
        }

        public string GetShopId()
        {
            // Kiểm tra lại lần nữa xem login thành công chưa và nhân thể set luôn Global.myShopId nếu bên trên chưa có
            if (Global.myShopId == "")
            {
                ApiResult apiResult = Global.api.RequestOthers("https://banhang.shopee.vn/api/v2/login/", Method.GET, shopeeCookie);
                if (apiResult.success)
                {
                    dynamic results = JsonConvert.DeserializeObject<dynamic>(apiResult.content);
                    if (results.shopid != null)
                    { 
                        Global.myShopId = results.shopid;
                    }
                }
            }
            return Global.myShopId;
        }

        // Gọi hàm này trước khi request
        public void FakeshopeeCookie(RestRequest request)
        {
            foreach (OpenQA.Selenium.Cookie cookie in shopeeCookie)
            {
                request.AddCookie(cookie.Name, cookie.Value.TrimEnd('"').TrimStart('"'));
                request.AddCookie("SPC_CDS", SPC_CDS);
            }
        }

        // Lấy thông tin sản phẩm Shopee
        public NSShopeeProduct.ShopeeProduct GetShopeeProductData(string itemId, string shopId)
        {
            ApiResult apiResult;
            apiResult = Global.api.RequestOthers("https://shopee.vn/api/v2/item/get?itemid=" + itemId + "&shopid=" + shopId, Method.GET);
            if(!apiResult.success)
            {
                Global.AddLog("ERROR: Lỗi kết nối api GetShopeeProductData");
                return null;
            }

            NSShopeeProduct.ShopeeProduct results = JsonConvert.DeserializeObject<NSShopeeProduct.ShopeeProduct>(apiResult.content);
            if(results == null || results.Error != null)
            {
                Global.AddLog("ERROR: Lỗi đọc thông tin ShopeeProduct");
                return null;
            }
            return results;
        }

        // Lấy thông tin sản phẩm taobao
        public NSTaobaoProduct.TaobaoProduct GetTaobaoProductData(string taobaoId)
        {
            ApiResult apiResult;
            apiResult = Global.api.RequestOthers(Global.api.laoNetApi + "&api_name=item_get&num_iid=" + taobaoId, Method.GET);
            if (!apiResult.success)
            {
                Global.AddLog("ERROR: Lỗi kết nối api GetTaobaoProductData");
                return null;
            }

            NSTaobaoProduct.TaobaoProduct results = JsonConvert.DeserializeObject<dynamic>(apiResult.content);
            if(results.Data.Item == null)
            {
                Global.AddLog("ERROR: Lỗi kết đọc thông tin TaobaoProductData");
                return null;
            }
            
            return results;
        }

        // Mỗi category có 1 model (form nhập thông tin) khác nhau. Cần lấy model_id sau đó lấy form để biết đc nó yêu cầu nhập những thông tin gì
        // Tạm thời gọi thẳng API để post nên có thể qua mặt đc phần nhập thông tin form, nhưng vẫn cần truyền đúng model_id
        public int GetAttributeModelId(string catId)
        {
            ApiResult apiResult;
            apiResult = Global.api.RequestOthers("https://banhang.shopee.vn/api/v3/category/get_category_attributes?category_ids=" + catId, Method.GET, shopeeCookie);
            if (!apiResult.success)
            {
                return 0;
            }

            dynamic results = JsonConvert.DeserializeObject<dynamic>(apiResult.content);
            return int.Parse(results.data.list[0].attribute_model_id.ToString());
        }

        // Đăng ảnh từ máy mình lên shopee, nhận lại id của ảnh shopee đã lưu
        public string PostImageToShopee(string path)
        {
            ApiResult apiResult;
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>
            {
                ["file"] = path
            };

            apiResult = Global.api.RequestOthers("https://banhang.shopee.vn/api/v3/general/upload_image/?SPC_CDS="+ SPC_CDS + "&SPC_CDS_VER=2", Method.POST, shopeeCookie, parameters);
            if (!apiResult.success)
            {
                return "";
            }
            dynamic results = JsonConvert.DeserializeObject<dynamic>(apiResult.content);
            if(results.message != "failed")
            {
                string resource_id = results.data.resource_id;
                Global.AddLog("Đã đăng được ảnh " + resource_id);
                return resource_id;
            } else
            {
                Global.AddLog("Lỗi khi đăng ảnh "+ path);
                return "";
            }
        }

        public string PostImageToShopeeChat(string path)
        {
            ApiResult apiResult;
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>
            {
                ["file"] = path
            };

            apiResult = Global.api.RequestOthers("https://banhang.shopee.vn/webchat/api/v1/mini/images", Method.POST, shopeeCookie, parameters);
            if (!apiResult.success)
            {
                return "";
            }
            dynamic results = JsonConvert.DeserializeObject<dynamic>(apiResult.content);
            if (results.message != "failed")
            {
                string url = results.url;
                Global.AddLog("Đã đăng được ảnh lên shopeechat " + url);
                return url;
            }
            else
            {
                Global.AddLog("Lỗi khi đăng ảnh lên shopeechat " + path);
                return "";
            }
        }

        public bool SendChatToShopee(string toId, string type, string value)
        {
            
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            /*
            string data = "{\"content\":{\"text\":\""+value+"\"},\"to_id\":"+ toId + ",\"type\":\"text\",\"request_id\":\"4c8ec91f-b34a-4e09-bccb-" + unixTimestamp.ToString() + "\",\"created_timestamp\":" + unixTimestamp.ToString()+ ",\"source\":\"minichat\"}";
            if(type == "image")
            {
                data = "{\"content\":{\"url\":\"" + value + "\"},\"to_id\":" + toId + ",\"type\":\"image\",\"request_id\":\"4c8ec91f-b34a-4e09-bccb-" + unixTimestamp.ToString() + "\",\"created_timestamp\":" + unixTimestamp.ToString() + ",\"source\":\"minichat\"}";

            }
            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
            dynamic dataaa = JsonConvert.DeserializeObject<dynamic>(data);
            parameters.Add("text/plain;charset=UTF-8", JsonConvert.SerializeObject(data));
            ApiResult apiResult;
            apiResult = Global.api.RequestOthers("https://banhang.shopee.vn/webchat/api/v1.1/mini/messages", Method.POST, shopeeCookie, parameters);
            if (apiResult.success)
            {
                return true;
            } else
            {
                return false;
            }
            */

            var client = new RestClient("https://banhang.shopee.vn/webchat/api/v1.1/mini/messages");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            foreach (OpenQA.Selenium.Cookie cookie in shopeeCookie)
            {
                request.AddCookie(cookie.Name, cookie.Value.TrimEnd('"').TrimStart('"'));
            }
            request.AddHeader("accept-language", "en-US,en;q=0.9");
            request.AddHeader("referer", "https://banhang.shopee.vn/");
            request.AddHeader("sec-fetch-mode", "cors");
            request.AddHeader("sec-fetch-site", "same-origin");
            request.AddHeader("accept", "*/*");
            request.AddHeader("content-type", "text/plain;charset=UTF-8");
            request.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.132 Safari/537.36");
            request.AddHeader("sec-fetch-dest", "empty");
            request.AddHeader("origin", "https://banhang.shopee.vn");
            request.AddHeader("authorization", Global.authToken);
            request.AddHeader("connection", "keep-alive");
            if(type == "image")
            {
                request.AddParameter("text/plain;charset=UTF-8", "{\"content\":{\"url\":\""+value+"\"},\"to_id\":" + toId + ",\"type\":\"image\",\"request_id\":\"fed34d00-35b4-4fb4-a2d8-" + unixTimestamp.ToString() + "\",\"created_timestamp\":" + unixTimestamp.ToString() + ",\"source\":\"minichat\"}", ParameterType.RequestBody);
            } else
            {
                request.AddParameter("text/plain;charset=UTF-8", "{\"content\":{\"text\":\""+value+"\"},\"to_id\":" + toId + ",\"type\":\"text\",\"request_id\":\"fed34d00-35b4-4fb4-a2d8-" + unixTimestamp.ToString() + "\",\"created_timestamp\":" + unixTimestamp.ToString() + ",\"source\":\"minichat\"}", ParameterType.RequestBody);

            }
            IRestResponse response = client.Execute(request);
            return true;
        }
        public class PrepareTaobaoData
        {
            public List<string> generalImgs = new List<string>();
            public Dictionary<string, string> SKUImages = new Dictionary<string, string>();
            public Dictionary<string, string> skuNames = new Dictionary<string, string>();
            public Dictionary<string, string> uploadedImages = new Dictionary<string, string>();
        }
        // Copy ảnh từ Taobao sang Shopee, ghép các array của Taobao lại để dễ truy cập hơn khi BuildSKU đúng dạng của shopee
        public PrepareTaobaoData PrepareTBData(NSTaobaoProduct.TaobaoProduct taobaoProductInfo)
        {
            Global.AddLog("Đang up ảnh từ taobao sang shopee nên hơi lâu chút");
            PrepareTaobaoData PrepareTaobaoData = new PrepareTaobaoData();
            string shopeeMd5 = "";
            if (taobaoProductInfo.Data.SkuBase != null && taobaoProductInfo.Data.SkuBase.Props != null && taobaoProductInfo.Data.SkuBase.Props.Count > 0)
            {
                foreach (NSTaobaoProduct.Prop prop in taobaoProductInfo.Data.SkuBase.Props)
                {
                    foreach (NSTaobaoProduct.Value value in prop.Values)
                    {

                        if (value.Image != null)
                        {
                            // Nếu chưa up thì up
                            if (!PrepareTaobaoData.uploadedImages.ContainsKey(value.Image))
                            {
                                string downloadedImage = helper.DownloadImage(value.Image);
                                if(downloadedImage != "CANNOT_DOWNLOAD_FILE")
                                {
                                    shopeeMd5 = PostImageToShopee(downloadedImage);
                                }
                                if (shopeeMd5 != "")
                                {
                                    PrepareTaobaoData.uploadedImages[value.Image] = shopeeMd5;
                                }
                            }
                            // SKU proppath có dạng "20509:28314;1627207:28341" vì vậy ở đây mình ghép Pid và Vid vào thành 1627207:28341 cho dễ gọi
                            if(PrepareTaobaoData.uploadedImages[value.Image] != null)
                            {
                                PrepareTaobaoData.SKUImages[prop.Pid + ":" + value.Vid] = PrepareTaobaoData.uploadedImages[value.Image];
                            }   
                        }

                        if (value.Name != null)
                        {
                            PrepareTaobaoData.skuNames[(prop.Pid + ":" + value.Vid)] = value.Name;
                        }
                    }
                }
            }

            foreach (string item_img in taobaoProductInfo.Data.Item.Images)
            {
                // Lặp nốt mảng item_imgs thì up rồi cho vào List
                if (!PrepareTaobaoData.uploadedImages.ContainsKey(item_img) && PrepareTaobaoData.generalImgs.Count < 8) {
                    string downloadedImage = helper.DownloadImage(item_img);
                    if (downloadedImage != "CANNOT_DOWNLOAD_FILE")
                    {
                        shopeeMd5 = PostImageToShopee(downloadedImage);
                    }

                    if (shopeeMd5 != "")
                    {
                        PrepareTaobaoData.uploadedImages[item_img] = shopeeMd5;
                        PrepareTaobaoData.generalImgs.Add(shopeeMd5);
                    }
                    
                }
            }

            return PrepareTaobaoData;
        }

        // Lấy SKU của sản phẩm taobao, đưa nó về đúng form mà shopee yêu cầu, trả về model_list và tier_variation
        public dynamic BuildShopeeSKUBasedOnTaobao(NSTaobaoProduct.TaobaoProduct taobaoProductInfo, PrepareTaobaoData PrepareTaobaoData, float revenuePercent, int weight)
        {
            // Không thể bán lỗ được vì vậy revenuePercent phải lớn hơn hoặc bằng 1
            revenuePercent = Math.Max(revenuePercent, 1);

            ApiResult apiResult;

            Global.AddLog("Bắt đầu lấy danh sách SKU của sản phẩm");          


            NSShopeeCreateProduct.TierVariation variation = new NSShopeeCreateProduct.TierVariation();
            List<NSShopeeCreateProduct.TierVariation> tier_variations = new List<NSShopeeCreateProduct.TierVariation>();

            NSShopeeCreateProduct.ModelList model = new NSShopeeCreateProduct.ModelList();
            List<NSShopeeCreateProduct.ModelList> model_lists = new List<NSShopeeCreateProduct.ModelList>();

            // Lưu lại tránh trường nhiều variation trùng tên thì phải lưu những thằng đã dùng r
            List<string> skuNames = new List<string>();

            variation.Name = "Mẫu mã";
            variation.Options = new List<string>() { };
            variation.Images = new List<string>() { };
            int index = 0;

            if (taobaoProductInfo.Data.SkuBase != null && taobaoProductInfo.Data.SkuBase.Skus != null && taobaoProductInfo.Data.SkuBase.Skus.Count > 0)
            {
                // Tìm ra SKU có giá cao nhất, để loại bỏ SKU nào có giá nhỏ hơn 1/5 giá cao nhất (Shopee ko cho phép chênh lệch giá quá 5 lần)
                float maxPrice = 1;
                foreach (NSTaobaoProduct.Skus Sku in taobaoProductInfo.Data.SkuBase.Skus)
                {
                    float price = float.Parse(taobaoProductInfo.Data.Details.SkuCore.Sku2Info[Sku.SkuId].Price.PriceText);
                    if (price > maxPrice)
                    {
                        maxPrice = price;
                        
                    }
                }
                Global.AddLog("SKU có giá cao nhất là " + maxPrice + " vì vậy sẽ bỏ qua các SKU có giá nhỏ hơn 1/3 = "+ (maxPrice / 3));

                foreach (NSTaobaoProduct.Skus Sku in taobaoProductInfo.Data.SkuBase.Skus)
                {
                    string[] skuProps = Sku.PropPath.Split(';');

                    string skuName = "";
                    if (taobaoProductInfo.Data.Details.SkuCore.Sku2Info[Sku.SkuId].Quantity > 0)
                    {
                        model = new NSShopeeCreateProduct.ModelList();
                        model.Id = 0;
                        model.Name = "";
                        // Đăng tối đa 79 sản phẩm vì mình thích thế, hihi
                        model.Stock = Math.Min(taobaoProductInfo.Data.Details.SkuCore.Sku2Info[Sku.SkuId].Quantity, 69);

                        // Gọi lên API để tính cước vận chuyển để tính ra giá cuối cùng
                        //float SKUPrice = SKUData.price * revenuePercent; // Dòng này sai, phải gọi lên API tính giá vc (giá gốc) xong rồi mới nhân tỉ lệ để ra giá rao trên shopee
                        float modelPrice = float.Parse(taobaoProductInfo.Data.Details.SkuCore.Sku2Info[Sku.SkuId].Price.PriceText);
                        
                        // Nếu giá của SKU này thấp hơn 1/5 giá của SKU cao nhất thì bỏ qua vì Shopee ko cho phép
                        if(modelPrice / maxPrice <= 0.3f) // 1/3 cho chắc ăn, có thể còn liên quan tới phí vận chuyển
                        {
                            Global.AddLog("SKU cao nhất giá là "+ maxPrice+", SKU này có giá quá thấp "+modelPrice+". Bỏ qua!");
                            continue;
                        } else
                        {
                            Global.AddLog("modelPrice / maxPrice = " + (modelPrice / maxPrice) + ". Tỉ lệ này lớn hơn 0.3f nên cho tiếp tục chạy");
                        }

                        Dictionary<string, string> parameters = new Dictionary<string, string>
                        {
                            ["route"] = "shipping-fee-ns",
                            ["amount"] = modelPrice.ToString(),
                            ["weight"] = weight.ToString()
                        };
                       // Global.AddLog("Tinh gia  cua SKU " + Sku.SkuId + " gia TQ: "+modelPrice.ToString());
                        // Gọi đến chết bao giờ tính đc giá thì thôi, cái này ko thể ko dừng đc
                        bool calcSuccess = false;
                        do
                        {
                            try
                            {
                                apiResult = Global.api.RequestMyApi(parameters);
                                if (apiResult.success)
                                {
                                    // Đề phòng trường hợp final_price tính sai hoặc trả về 0
                                    // 3600 là tỉ giá, bán kiểu gì cũng phải cao hơn giá này.
                                    model.Price = Math.Min((int)(modelPrice * 3600), (int)JsonConvert.DeserializeObject<dynamic>(apiResult.content).final_price).ToString();
                                    calcSuccess = true;
                                }
                                else
                                {
                                    Global.AddLog("Tính giá chưa đc, tao gọi lại đến chết, bao giờ tính đc thì thôi");
                                    Thread.Sleep(1000);
                                }
                            }
                            catch (Exception e)
                            {
                                Global.AddLog("Tính giá chưa đc lại còn bị lỗi " + e.Message.ToString() + ", tao gọi lại đến chết, bao giờ tính đc thì thôi");
                                Thread.Sleep(1000);
                            }
                        } while (!calcSuccess);
                        Global.AddLog("Giá bán ra cua SKU "+ Sku.SkuId + " trước khi nhân tỉ lệ: " + model.Price.ToString() + ". Tỉ lệ nhân " + revenuePercent.ToString());
                        int originalModelPrice = int.Parse(model.Price);
                        // Giá bán ra cuối cùng bằng giá thật nhân với tỉ lệ, nhưng tối thiểu phải lãi minRevenueInMoney
                        model.Price = (Math.Max(originalModelPrice + minRevenueInMoney, originalModelPrice * revenuePercent) / 1000 * 1000).ToString();
                        model.Sku = Sku.SkuId.ToString();
                        model.TierIndex = new List<int>() { index };
                        // Thêm SKU vào model_list
                        model_lists.Add(model);

                        //======= SKU NAME ==========
                        // Dịch SKU name (để sinh ra dạng Xanh - Size XL)
                        foreach (string skuProp in skuProps)
                        {
                            // Mã này là size
                            //if(skuProp.StartsWith("20509")), //1627207 là màu

                            // Xóa một số từ ko quan trọng
                            foreach (string notImportantWord in notImportantWords)
                            {
                                skuName = skuName.Replace(notImportantWord, "");
                            }
                            skuName += Global.Translate(PrepareTaobaoData.skuNames[skuProp]).Replace("[","").Replace("]", "").Replace("Khuyến nghị", "").Replace("khuyến nghị", "").Replace("Kích thước", "").Replace("kích thước", "").Replace("(", "").Replace(")", "").Replace("  "," ");

                         
                            // Dấu ngăn cách giữa các Prop đúng chuẩn là " - " tuy nhiên do shopee giới hạn 20 kí tự nên thôi chỉ viết dấu cách thôi
                            if (!skuProp.Equals(skuProps.Last()))
                            {
                                skuName += " ";
                            }
                            
                        }
                        
                        // SKUName tối đa 20 kí tự
                        skuName = skuName.Substring(0, Math.Min(20, skuName.Length));
                        // Nếu bị trùng với một SKUName nào trước đó thì thêm chữ kiểu x
                        if (skuNames.Contains(skuName))
                        {
                            bool foundGoodName = false;
                            int alt = 2;
                            do
                            {
                                if (!skuNames.Contains(skuName.Substring(0, Math.Min(17, skuName.Length)) + " K" + alt))
                                {
                                    skuName = skuName.Substring(0, Math.Min(17, skuName.Length)) + " K" + alt;
                                    foundGoodName = true;
                                }
                                else
                                {
                                    alt++;
                                }
                            } while (!foundGoodName);
                        }
                        skuNames.Add(skuName);

                        Global.AddLog("Giá bán ra cho SKU " + Sku.SkuId + " -  " + skuName + " là: " + model.Price.ToString());
                        // Thêm SKU vào danh sách variation shopee
                        variation.Options.Add(skuName.ToString());
                        //======= END SKU NAME ==========




                        //======= SKU IMAGES ==========
                        // Tìm ảnh để thêm vào images của  varation
                        if (PrepareTaobaoData.SKUImages.Count > 0)
                        {
                            // Nếu sản phẩm taobao có SKU Images thì chọn ảnh tương ứng để add vào đúng thứ tự của Shopee
                            bool foundImage = false;
                            foreach (string skuProp in skuProps)
                            {
                                if (PrepareTaobaoData.SKUImages.ContainsKey(skuProp))
                                {
                                    variation.Images.Add(PrepareTaobaoData.SKUImages[skuProp]);
                                    foundImage = true;
                                    break;
                                }
                            }
                            // Trong trường hợp variation này ko có ảnh nào khớp thì phải chọn ngẫu nhiên 1 ảnh add vào để đảm bảo
                            // không bị sai thứ tự mảng
                            if (!foundImage)
                            {
                                foreach (var d in PrepareTaobaoData.uploadedImages)
                                {
                                    variation.Images.Add(d.Value);
                                    break;
                                }
                            }
                        }
                        //======= END SKU IMAGE ==========



                        index++;
                        // Shopee chỉ cho tối đa 15 Variation
                        if(index == 16)
                        {
                            break;
                        }
                    }
                }
            }
            

            dynamic responseData = new ExpandoObject();
            if(index > 0)
            {
                tier_variations.Add(variation);
            }
            responseData.tier_variation = tier_variations;
            responseData.model_list = model_lists;
            Global.AddLog("Lấy SKU xong!");
            return responseData;
        }

        // Sinh ra một description đáng yêu ♥
        public class DesAndTitle
        {
            public string Description = "";
            public string Title = "";
        }
        public DesAndTitle BeautifulDescriptionAndTitle(NSShopeeCreateProduct.CreateProduct postData, NSShopeeProduct.ShopeeProduct shopeeProductInfo, NSTaobaoProduct.TaobaoProduct taobaoProductInfo, int ShopeeCategoryId)
        {
            DesAndTitle destitle = new DesAndTitle();

            string categoryTitle = ShopeeCategory.GetCategoryName(ShopeeCategoryId);
            destitle.Title = categoryTitle;

            // Độ dài tối đa 3000 kí tự
            //" + shopeeProductInfo.Item.Description.Substring(0, Math.Min(2000, shopeeProductInfo.Item.Description.Length)).Replace(",", ", ").Replace(".", ". ").Replace("  ", " ").Replace(" ,", ", ").Replace(".", ". ") + @"
            string fullPropsString = "";
            List<string> ignoreKeys = new List<string> { "成色", "大码女装分类", "是否商场同款", "箱包硬度", "价格区间","流行款式名称", "品牌", "甜美", "货号", "套头", "开口深度", "皮质特征", "通勤", "流行元素/工艺", "安全等级", "销售渠道类型"};
            List<string> ignoreValues = new List<string> {"一字领","甜美", "套头", "开口深度", "皮质特征", "其他", "其他/other", "other", "短裙" };
            
            List<string> badWords = new List<string> {"含", "其他", "other", "元", "清仓", "价格区间"};
            // 价格区间 Mức giá
            // Trong key hoặc value mà có từ này là bỏ qua cả cụm luôn
            // 流行款式名称: Nghe tối nghĩa quá, tên phong cách phổ biến
            // 建议 : khuyến nghị, ước lượng, khoảng bao nhiêu kg... nói chung nó làm tên SKU bị dài, xóa
            // 甜美 ngọt ngào là cái qq gì @@ 
            // 安全等级 cấp độ bảo mật, vô nghĩa quá
            // 品牌 thương hiệu
            // 货号 mã số bài viết
            // 套头 ko dịch nổi, tay áo, bảo hiểm rủi do, chả liên quan gì nhau
            // 开口深度 Độ nông sâu của giày nhưng nói chung tối nghĩa
            // 短裙 Kiểu váy: 短裙 (nghĩa là váy), ý là váy bt, nhưng mà bt thì thôi ko cần ghi
            // 流行元素/工艺: Cách may ra cái váy, giá trị thường là: May, có cúc... nhưng mà sida quá loại
            // 一字领 nếu là key thì nghe có nghĩa, nếu là value thì ko có nghĩa

            Dictionary<string, string> propsTranslated = new Dictionary<string, string>();
            if (taobaoProductInfo.Data.Props != null && taobaoProductInfo.Data.Props.GroupProps != null)
            { 
                foreach (NSTaobaoProduct.GroupProp groupProp in taobaoProductInfo.Data.Props.GroupProps)
                {
                    if(groupProp.BasicInfo != null)
                    {
                        foreach (Dictionary<string, string> prop in groupProp.BasicInfo)
                        {
                            foreach (KeyValuePair<string, string> entry in prop)
                            {
                                string k = entry.Key;
                                string v = entry.Value;

                                // Xóa một số từ ko quan trọng
                                foreach(string notImportantWord in notImportantWords)
                                {
                                    k = k.Replace(notImportantWord, "");
                                    v = v.Replace(notImportantWord, "");
                                }

                                // Bỏ qua nếu key, value = một trong các từ trong danh sách
                                if (ignoreKeys.Contains(k.ToLower()))
                                {
                                    continue;
                                }
                                if (ignoreValues.Contains(v.ToLower()))
                                {
                                    continue;
                                }

                                // Bỏ qua nếu key, value chứa badword
                                bool foundBadWord = false;
                                foreach(string badWord in badWords)
                                {
                                    if(k.ToLower().Contains(badWord.ToLower()) || v.ToLower().Contains(badWord.ToLower()))
                                    {
                                        foundBadWord = true;
                                        break;
                                    }
                                }
                                if(foundBadWord)
                                {
                                    continue;
                                }

                                // Nối vào Description
                                string keyTranslated = Global.Translate(k);
                                fullPropsString += keyTranslated;
                                fullPropsString += ": ";

                                string valueTranslated = Global.Translate(v);
                                fullPropsString += valueTranslated + @"
";
                                propsTranslated[keyTranslated] = valueTranslated;
                            }

                        }
                    }
                   
                }
            }

            // Nối vào title nếu gặp một số key quen thuộc
            Dictionary<int, string> nonSortTitle = new Dictionary<int, string>();
            foreach(KeyValuePair<string, string> prop in propsTranslated)
            {
                string keyTranslated = prop.Key;
                string valueTranslated = prop.Value;

                if (keyTranslated == "Hoa văn, họa tiết")
                {
                    nonSortTitle[0] = Helper.LowerFisrtLetter(valueTranslated);
                }

                if (keyTranslated == "Dáng eo")
                {
                    nonSortTitle[50] = Helper.LowerFisrtLetter(valueTranslated);
                }

                if (keyTranslated == "Loại cổ")
                {
                    nonSortTitle[100] = Helper.LowerFisrtLetter(valueTranslated);
                }

                if (keyTranslated == "Loại tay áo" && valueTranslated != "Bình thường")
                {
                    nonSortTitle[150] = Helper.LowerFisrtLetter(valueTranslated);
                }

                if (keyTranslated == "Độ dài tay áo")
                {
                    nonSortTitle[200] = Helper.LowerFisrtLetter(valueTranslated);
                }

                if (keyTranslated == "Dành cho (nam / nữ)")
                {
                    nonSortTitle[250] = Helper.LowerFisrtLetter(valueTranslated);
                }

                if (keyTranslated == "Phong cách")
                {
                    nonSortTitle[300] = Helper.LowerFisrtLetter(valueTranslated);
                }

                if (keyTranslated == "Chất liệu")
                {
                    nonSortTitle[350] = "chất liệu " + Helper.LowerFisrtLetter(valueTranslated);
                }

                if (keyTranslated == "Mùa áp dụng")
                {
                    nonSortTitle[400] = "cho " + Helper.LowerFisrtLetter(valueTranslated);
                }

                if (keyTranslated == "Các màu")
                {
                    nonSortTitle[450] = "nhiều màu";
                }

                if (keyTranslated == "Kích thước")
                {
                    nonSortTitle[500] = "đủ size";
                }

                if (keyTranslated == "Phù hợp với")
                {
                    nonSortTitle[550] = "dành cho " + Helper.LowerFisrtLetter(valueTranslated);
                }

                if (keyTranslated == "Lứa tuổi phù hợp")
                {
                    nonSortTitle[600] = "phù hợp " + Helper.LowerFisrtLetter(valueTranslated);
                }

                if (keyTranslated == "Thời trang")
                {
                    nonSortTitle[650] = "[SIÊU HOT " + Helper.LowerFisrtLetter(valueTranslated).ToUpper()+"]";
                }
            }
            // Sắp xếp lại nonSortTitle rồi nối vào title cuối cùng
            var list = nonSortTitle.Keys.ToList();
            list.Sort();
            // Loop through keys.
            foreach (var key in list)
            {
                string prependString = " " + nonSortTitle[key];
                if(destitle.Title.Length < 120 - prependString.Length)
                {
                    destitle.Title += prependString;
                }
                
            }

            // Nếu tới đây mà vẫn chưa có title thì lấy bừa thôi
            if (destitle.Title == categoryTitle)
            {
                foreach (KeyValuePair<string, string> prop in propsTranslated)
                {
                    if(destitle.Title.Length < 100)
                    {
                        destitle.Title += ", " + Helper.LowerFisrtLetter(prop.Key) + " " + Helper.LowerFisrtLetter(prop.Value);
                    }
                }
            }
            // Nếu tới đây mà ko có gì ở title thì đành gọi Google dịch
            if (destitle.Title == categoryTitle || destitle.Title.Length < 20)
            {
                destitle.Title = Global.Translate(taobaoProductInfo.Data.Item.Title);
            }
            destitle.Title = Global.AntiDangStyle(destitle.Title);

            string desciption = destitle.Title+ @"
★★★ THÔNG TIN SẢN PHẨM
" + fullPropsString + @"
★★★ CAM KẾT VÀ DỊCH VỤ
- Sản phẩm đảm bảo chất lượng, chính xác 100% về thông số, mô tả và hình ảnh.
- Thời gian giao hàng dự kiến: Trong vòng 10 ngày đối với sản phẩm order và 4 ngày đối với sản phẩm có sẵn. Thông tin vận chuyển được gửi tới Quý khách hàng ngày qua tin nhắn Shopee.
- Hình thức thanh toán: COD toàn quốc.
- Khách hàng đặt mua số lượng lớn vui lòng liên hệ trực tiếp để được giảm giá từ 5% đến 20%.
";

            destitle.Description = desciption;
            return destitle;
        }

        public string CopyTaobaoToShopee(string jobName, NSTaobaoProduct.TaobaoProduct taobaoProductInfo, NSShopeeProduct.ShopeeProduct shopeeProductInfo, NSApiProducts.NsApiProduct jobData)
        {
            Global.driver.Navigate().GoToUrl("https://banhang.shopee.vn/portal/product/category");
            Random random = new Random();
            NSShopeeCreateProduct.CreateProduct postData = JsonConvert.DeserializeObject<NSShopeeCreateProduct.CreateProduct>("{\"id\":0,\"name\":\"Boo loo ba la\",\"brand\":\"No brand\",\"images\":[\"809019b6b3727424bdde5bd677bedec9\",\"0bcd30a3c76c3fc56a5539b3db775650\"],\"description\":\"Không được để trống Không được để trống Không được để trống Không được để trống Không được để trống Không được để trống Không được để trống Không được để trống Không được để trống Không được để trống Không được để trống Không được để trống Không được để trống Không được để trống Không được để trống Không được để trống Không được để trống \",\"model_list\":[{\"id\":0,\"name\":\"\",\"stock\":12,\"price\":\"123000\",\"sku\":\"XL_DEN_123\",\"tier_index\":[0]},{\"id\":0,\"name\":\"\",\"stock\":34,\"price\":\"345000\",\"sku\":\"S_TRANG_345\",\"tier_index\":[1]}],\"category_path\":[162,13206,13210],\"attribute_model\":{\"attribute_model_id\":15159,\"attributes\":[{\"attribute_id\":13054,\"prefill\":false,\"status\":0,\"value\":\"No brand\"},{\"attribute_id\":20074,\"prefill\":false,\"status\":0,\"value\":\"1 Tháng\"}]},\"category_recommend\":[],\"stock\":0,\"price\":\"123000\",\"price_before_discount\":\"\",\"parent_sku\":\"SKU chỗ này là cái gì vậy?\",\"wholesale_list\":[],\"installment_tenures\":{},\"weight\":\"200\",\"dimension\":{\"width\":10,\"height\":10,\"length\":20},\"pre_order\":true,\"days_to_ship\":8,\"condition\":1,\"size_chart\":\"\",\"tier_variation\":[{\"name\":\"Mẫu mã\",\"options\":[\"Size XL màu đen\",\"Size S màu trắng\"],\"images\":[\"02add0536f76d882cdb5b9a13effc546\",\"d853ecab31f9488d2a249b1fef6c1e6a\"]}],\"logistics_channels\":[{\"price\":\"0.00\",\"cover_shipping_fee\":false,\"enabled\":true,\"channelid\":50018,\"sizeid\":0},{\"price\":\"8000.00\",\"cover_shipping_fee\":false,\"enabled\":true,\"channelid\":50016,\"sizeid\":0},{\"price\":\"9000.00\",\"cover_shipping_fee\":false,\"enabled\":true,\"channelid\":50011,\"sizeid\":0},{\"price\":\"9000.00\",\"cover_shipping_fee\":false,\"enabled\":true,\"channelid\":50012,\"sizeid\":0},{\"price\":\"8000.00\",\"cover_shipping_fee\":false,\"enabled\":true,\"channelid\":50015,\"sizeid\":0},{\"price\":\"9000.00\",\"cover_shipping_fee\":false,\"enabled\":true,\"channelid\":50010,\"sizeid\":0}],\"unlisted\":false,\"add_on_deal\":[],\"ds_cat_rcmd_id\":\"0\"}"); ;

            
            PrepareTaobaoData PrepareTaobaoData = PrepareTBData(taobaoProductInfo);

            // Tính toán data thật
            // Tính giá TB các SKU Taobao
            Global.AddLog("Tính giá trung bình mỗi SKU Taobao");

            // Giá chung chung toàn bộ sp (lát nữa mỗi SKU có giá riêng)
            string[] tempPrices = taobaoProductInfo.Data.Details.Price.Price.PriceText.Split('-');
            float taobaoPrice = float.Parse(tempPrices[0]);
            if (taobaoProductInfo.Data.Details.Price.ExtraPrices != null && taobaoProductInfo.Data.Details.Price.ExtraPrices.Count > 0)
            {
                tempPrices = taobaoProductInfo.Data.Details.Price.ExtraPrices.First().PriceText.Split('-');
                taobaoPrice = float.Parse(tempPrices[0]);
            }
            if (taobaoProductInfo.Data.Details.Price.NewExtraPrices != null && taobaoProductInfo.Data.Details.Price.NewExtraPrices.Count > 0)
            {
                tempPrices = taobaoProductInfo.Data.Details.Price.NewExtraPrices.First().PriceText.Split('-');
                taobaoPrice = float.Parse(tempPrices[0]);
            }

            if (taobaoProductInfo.Data.Details.SkuCore.Sku2Info.Count > 0)
            {
                float taobaoPriceAvg = 0;
                int c = 0;
                foreach (KeyValuePair<string, NSTaobaoProductDetail.Sku2Info> entry in taobaoProductInfo.Data.Details.SkuCore.Sku2Info)
                {
                    if(entry.Key != "0")
                    {

                        taobaoPriceAvg += float.Parse(entry.Value.Price.PriceText);
                        c++;
                    }
                                      
                }
                if(c > 0)
                {
                    taobaoPriceAvg /= c;
                    taobaoPrice = taobaoPriceAvg;
                }
            }

            // Gọi lên API để tính cước vận chuyển để tính ra giá cuối cùng

            // Lấy thông tin cân nặng của đối thủ ở shopee
            Global.AddLog("Lấy kích thước sản phẩm");
            ApiResult apiResult = Global.api.RequestOthers("https://shopee.vn/api/v0/shop/" + shopeeProductInfo.Item.Shopid + "/item/" + shopeeProductInfo.Item.Itemid + "/shipping_info_to_address/?city=Huy", Method.GET);
            if (!apiResult.success)
            {
                Global.AddLog("STOP: Lỗi khi lấy thông tin kích thước sản phẩm");
                return "error";
            }
            dynamic shopeeShippings = JsonConvert.DeserializeObject<dynamic>(apiResult.content);
            if (shopeeShippings == null || shopeeShippings.error_code != null)
            {
                Global.AddLog("STOP: Lỗi khi lấy thông tin kích thước sản phẩm.");
                return "error";
            }
            int weight = 300, width = 5, height = 2, length = 15; 
            if(shopeeShippings.shipping_infos != null &&  shopeeShippings.shipping_infos[0].debug != null && shopeeShippings.shipping_infos[0].debug.total_weight != null)
            {
                // Chắc ko cái nào nặng hơn 1 cân đâu :D nặng hơn thì mình sửa bằng tay sau.
                weight = Math.Max(Math.Min(1000, (int)(shopeeShippings.shipping_infos[0].debug.total_weight * 1000)), 200);
                width = Math.Max(5, int.Parse(shopeeShippings.shipping_infos[0].debug.sizes_data[0].width.ToString()));
                height = Math.Max(2, int.Parse(shopeeShippings.shipping_infos[0].debug.sizes_data[0].height.ToString()));
                length = Math.Max(5, int.Parse(shopeeShippings.shipping_infos[0].debug.sizes_data[0].length.ToString()));
            }


            // Tính giá TQ bao gồm cả ship về VN
            Global.AddLog("Gọi lên API để tính giá TQ sau vận chuyển");
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                ["route"] = "shipping-fee-ns",
                ["amount"] = taobaoPrice.ToString(),
                ["weight"] = weight.ToString()
            };

            apiResult = Global.api.RequestMyApi(parameters);
            if (apiResult.success)
            {
                taobaoPrice = JsonConvert.DeserializeObject<dynamic>(apiResult.content).final_price;
                Global.AddLog("Giá taobao cuối cùng là " + taobaoPrice.ToString());
            } else
            {
                Global.AddLog("Lỗi tính giá TQ sau vận chuyển");
                return "error";
            }


            // Tính giá TB các SKU Shopee
            Global.AddLog("Tính giá trung bình mỗi SKU Shopee");
            string p; ;
            p = shopeeProductInfo.Item.PriceMax.ToString();
            int shopeePrice = int.Parse(p.Substring(0, p.Length - 5));
            if (shopeeProductInfo.Item.Models.Count > 0)
            {
                shopeePrice = 0;
                foreach (NSShopeeProduct.Model m in shopeeProductInfo.Item.Models)
                {
                    p = m.Price.ToString();
                    shopeePrice += int.Parse(p.Substring(0, p.Length - 5));
                }
                shopeePrice /= shopeeProductInfo.Item.Models.Count;
            }
            Global.AddLog("Giá shopee cuối cùng là "+ shopeePrice.ToString());
            if (shopeePrice == 0)
            {
                Global.AddLog("STOP: ShopeePrice = 0");
                return "error";
            }

            // Giá bán ra
            //float outPrice = Math.Max(taobaoPrice * (100 + minRevenue) / 100, (shopeePrice  * (100 - random.Next(1, 3)) / 100)); // Giá tối thiểu cần có lãi, random rẻ hơn đối thủ 1 đến 3%
            // Công thức bên trên là bám theo giá đối thủ, nhưng mà nhiều lúc giá linh tinh quá. Giờ chỉ bám theo giá taobao thôi. Tối thiểu 50% và random trong khoảng min đến max
            Random rd = new Random();
            float outPrice = Math.Max(taobaoPrice + minRevenueInMoney, rd.Next((int)taobaoPrice * (100 + minRevenue) / 100, (int)taobaoPrice * (100 + maxRevenue) / 100));
            Global.AddLog("Quyết định bán ra giá chung chung là: " + outPrice.ToString());

            float revenuePercent;
            revenuePercent = outPrice / taobaoPrice;

            
            dynamic sku = BuildShopeeSKUBasedOnTaobao(taobaoProductInfo, PrepareTaobaoData, revenuePercent, weight);
            List<int> categoryPath = new List<int>();
            foreach(NSShopeeProduct.Category cat in shopeeProductInfo.Item.Categories)
            {
                categoryPath.Add(int.Parse(cat.Catid.ToString()));
            }

            // Model Id
            int attribute_model_id = GetAttributeModelId(shopeeProductInfo.Item.Categories.Last().Catid.ToString());
            if(attribute_model_id == 0)
            {
                Global.AddLog("ERROR:Lỗi khi lấy modelId");
                return "error";
            }
            NSShopeeCreateProduct.AttributeModel  attributeModel = new NSShopeeCreateProduct.AttributeModel();
            attributeModel.AttributeModelId = attribute_model_id;
            attributeModel.Attributes = new List<NSShopeeCreateProduct.Attribute>();

            // Đẩy data thật vào object
            //string name = Global.AntiDangStyle(shopeeProductInfo.Item.Name.ToString()).Replace("sẵn", "order").Replace("Sẵn", "Order").Replace("SẴN", "ORDER");            
            if(jobName == "update")
            {
                // Nếu update thì cần cái này, còn nếu list mới thì ko cần
                postData.Id = shopeeProductInfo.Item.Itemid;
            }
            postData.Images                             = PrepareTaobaoData.generalImgs;
            postData.CategoryPath                       = categoryPath;
            postData.AttributeModel                     = attributeModel;
            postData.Price                              = ((int)((int)outPrice / 1000) * 1000).ToString();
            postData.TierVariation                      = sku.tier_variation;
            postData.ModelList                          = sku.model_list;
            //postData.ds_cat_rcmd_id                   = random.Next(11111111, 91111111).ToString() + random.Next(11111111, 91111111).ToString(); // Chưa biết cái này là cái gì
            postData.ParentSku                          = random.Next(1111111, 9111111).ToString(); // Chưa biết cái này là cái gì
            postData.Weight                             = weight.ToString();
            postData.Dimension = new NSShopeeCreateProduct.Dimension();
            postData.Dimension.Width = width;
            postData.Dimension.Height = height;
            postData.Dimension.Length = length;

            DesAndTitle desAndTitle = BeautifulDescriptionAndTitle(postData, shopeeProductInfo, taobaoProductInfo, categoryPath[2]);
            postData.Description = desAndTitle.Description;
            postData.Name = desAndTitle.Title.Substring(0, Math.Min(desAndTitle.Title.Length, 120));

            // POST lên shopee
            Global.AddLog("Bắt đầu up sản phẩm");
            RestClient client = new RestClient("https://banhang.shopee.vn/api/v3/product/create_product/?version=3.1.0&SPC_CDS="+ SPC_CDS + "&SPC_CDS_VER=2");
            if (jobName == "update")
            {
                client = new RestClient("https://banhang.shopee.vn/api/v3/product/update_product/?version=3.1.0&SPC_CDS="+ SPC_CDS + "&SPC_CDS_VER=2");
            }
            //client.Timeout = -1;
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json;charset=UTF-8");
            FakeshopeeCookie(request);

            List<NSShopeeCreateProduct.CreateProduct> postDataFinal = new List<NSShopeeCreateProduct.CreateProduct>
            {
                postData
            };

            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };
            string postDataString = JsonConvert.SerializeObject(postDataFinal, new JsonSerializerSettings {ContractResolver = contractResolver});
            //Global.AddLog(postDataString);
            request.AddParameter("application/json;charset=UTF-8", postDataString, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            // In kết quả trả về
            dynamic results = JsonConvert.DeserializeObject<dynamic>(response.Content);
            Global.AddLog("\n\n results: \n\n" + results + "\n\n");
            try
            {
                if (results.message == "success")
                {
                    string SuccessProductID = results.data.result[0].data.product_id.ToString();
                    Global.AddLog("Upload thành công, ID sản phẩm mới ở Shopee là:" + SuccessProductID);
                    Global.AddLog("===============================");

                    Global.driver.Navigate().GoToUrl("https://banhang.shopee.vn/portal/product/list/all");
                    // Báo lên server
                    parameters = new Dictionary<string, string>
                                {
                                    { "route", "product/"+jobData.Id },
                                    { "source", "taobao" },
                                    { "account_id", Global.myAccountId },
                                    { "taobao_item_id", taobaoProductInfo.Data.Item.ItemId },
                                    { "shopee_item_id",  SuccessProductID},
                                    { "shopee_shop_id",  Global.myShopId},
                                    { "shopee_price",  postData.Price},
                                    { "shopee_model_list",  JsonConvert.SerializeObject(sku.model_list)},
                                    { "taobao_skubase",  JsonConvert.SerializeObject(taobaoProductInfo.Data.SkuBase)},
                                    { "action", "done" }
                                };
                    Global.api.RequestMyApi(parameters, Method.PUT);
                    return SuccessProductID;
                }
                else
                {
                    Global.AddLog("Upload thất bại, nội dung trả về là:" + results.data.result[0].message.ToString());
                    Global.AddLog("===============================");
                    // Báo lên server
                    parameters = new Dictionary<string, string>
                                {
                                    { "route", "product/"+jobData.Id },
                                    { "source", "taobao" },
                                    { "account_id", Global.myAccountId },
                                    { "message", results.data.result[0].message.ToString()},
                                    { "action", "error" }
                                };
                    Global.api.RequestMyApi(parameters, Method.PUT);
                    return "error";
                }
            }
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
            {
                return "error";
            }
        }


        //============================ XỬ LÝ ORDER =========================================== 
        public string ProcessNewCheckout()
        {
            Global.driver.Navigate().GoToUrl("https://banhang.shopee.vn/portal/sale/");

            ApiResult apiResult;
            List<NSShopeeOrders.Order> orders = new List<NSShopeeOrders.Order>();
            NSShopeeOrders.ShopeeOrders orderPage;
            int page = -1;
            bool shouldBreak = false;
            do
            {
                Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                page++;
                apiResult = Global.api.RequestOthers("https://banhang.shopee.vn/api/v3/order/get_order_list/?limit=40&offset=" + (page * 40) + "&total=0&flip_direction=ahead&page_sentinel=0,0&order_by_create_date=desc&is_massship=false", Method.GET, shopeeCookie);
                if (apiResult.success)
                {
                    orderPage = JsonConvert.DeserializeObject<NSShopeeOrders.ShopeeOrders>(apiResult.content);
                    if (orderPage.Code != 0)
                    {
                        Global.AddLog("ERROR: Lỗi lấy danh sách đơn hàng");
                        return "error";
                    } else if (orderPage.Data != null && orderPage.Data.Orders != null && orderPage.Data.Orders.Count > 0)
                    {
                        foreach (NSShopeeOrders.Order od in orderPage.Data.Orders)
                        {
                            // Chỉ lấy order trong 7 ngày qua. Còn lại chắc chắn đc xử lý xong r
                            if (od.CreateTime > unixTimestamp - 30 * 24 * 60 * 60)
                            {
                                Global.AddLog("Đã thêm order " + od.CheckoutId + " vào danh sách xử lý");
                                orders.Add(od);
                            }
                            else
                            {
                                shouldBreak = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        shouldBreak = true;
                    }

                } else
                {
                    Global.AddLog("ERROR: Lỗi khi lấy danh sách đơn hàngg");
                    return "error";
                }
            } while (shouldBreak == false);

            // Xử lý 1 vài thông tin rồi gộp cả vào 1 mảng bắn lên server
            List<NSShopeeOrders.Order> ordersSendToServer = new List<NSShopeeOrders.Order>();
            foreach (NSShopeeOrders.Order order in orders)
            {
                order.Market = "SHOPEE";
                /*
                // TESTTTTT Đoạn này để test, lấy lại hóa đơn vận đơn cũ.
                ApiResult apiResulttest;
                apiResulttest = Global.api.RequestOthers("https://banhang.shopee.vn/api/v3/shipment/get_drop_off/?order_id=" + order.OrderId, Method.GET, shopeeCookie);
                if (apiResult.success)
                {
                    dynamic results2 = JsonConvert.DeserializeObject<dynamic>(apiResulttest.content);
                    if (results2.data != null && results2.data.consignment_no != null && results2.data.consignment_no != "")
                    {
                        order.MVD = results2.data.consignment_no;
                        Global.AddLog("OrderID: " + order.OrderId + ". Mã vận đơn: " + order.MVD);

                        // Chụp ảnh hóa đơn
                        Global.driver.Navigate().GoToUrl("https://banhang.shopee.vn/api/v3/logistics/get_waybill_new/?order_ids=" + order.OrderId);
                        try
                        {
                            Global.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("body")));
                            Thread.Sleep(1000);
                            Bitmap bmpImage = Helper.ScreenshotWayBill();
                            string savePath = @"C:\Users\Admin\Desktop\ma_van_don\" + order.OrderId + ".jpg";
                            bmpImage.Save(savePath);
                            Thread.Sleep(200);
                            // Upload hóa đơn lên server
                            order.MVDImage = Global.api.UploadImageToMyServer(savePath);
                            Global.AddLog("Upload anh " + order.MVDImage);
                        }
                        catch (Exception e)
                        {
                            Global.AddLog("ERROR: Co loi xay ra khi upload don hang " + e.Message + e.StackTrace);
                        }
                        Global.AddLog("OrderID: " + order.OrderId + ". Đã lấy xong mã vận đơn và lưu ảnh");

                        // GỬI TIN NHẮN YÊU CẦU KHÁCH XÁC NHẬN ĐƠN
                        Random random = new Random();
                        int randomBetween1000And9999 = random.Next(100000, 999999);
                        string captchaFile = Helper.GenCaptcha(randomBetween1000And9999.ToString());
                        string url = PostImageToShopeeChat(captchaFile);
                        if (url == "")
                        {
                            SendChatToShopee(order.BuyerUser.UserId.ToString(), "text", "Cảm ơn Quý khách đã đặt hàng. Quý khách vui lòng trả lời tin nhắn với nội dung \"" + randomBetween1000And9999 + "\" để xác nhận đơn hàng. Trân trọng cảm ơn!");
                        }
                        else
                        {
                            SendChatToShopee(order.BuyerUser.UserId.ToString(), "image", url);
                            Thread.Sleep(500);
                            SendChatToShopee(order.BuyerUser.UserId.ToString(), "text", "Cảm ơn Quý khách đã đặt hàng. Quý khách vui lòng trả lời tin nhắn với nội dung là số được ghi trong ảnh để xác nhận đơn hàng. Trân trọng cảm ơn!");
                        }
                    }
                }
                ordersSendToServer.Add(order);
                */
                /// HẾT TEST
                /// 

                
                // logistics_channel 50018 là J&T, 50011 Gia hàng tiết kiệm
                switch (order.Status)
                {
                    case 1: // Đơn bt, đang ship
                        
                        // order.LogisticsStatus == 1 là đã add thông tin vận chuyển. Ko cần làm gì nữa
                        // = 9 cần add thoongtin vận chuyển
                        if (order.LogisticsStatus == 9)
                        {
                            Dictionary<string, dynamic> parameters = new Dictionary<string, dynamic>();
                            parameters.Add("json_body", "{\"channel_id\":" + order.LogisticsChannel + ",\"order_id\":" + order.OrderId + ",\"forder_id\":\"" + order.OrderId + "\"}");

                            apiResult = Global.api.RequestOthers("https://banhang.shopee.vn/api/v3/shipment/init_order/", Method.POST, shopeeCookie, parameters);
                            if (apiResult.success)
                            {
                                dynamic results = JsonConvert.DeserializeObject<dynamic>(apiResult.content);
                                if (results.message != null && results.message == "success")
                                {
                                    Global.AddLog("OrderID: " + order.OrderId + ". Submit phương thức vận chuyển thành công");

                                    for(int x = 0; x < 5; x++)
                                    {
                                        Thread.Sleep(2000);
                                        Global.AddLog("OrderID: " + order.OrderId + ". Lấy mã vận đơn lần thứ " + x);
                                        ApiResult apiResult2;
                                        apiResult2 = Global.api.RequestOthers("https://banhang.shopee.vn/api/v3/shipment/get_drop_off/?order_id="+order.OrderId, Method.GET, shopeeCookie);
                                        if (apiResult.success)
                                        {
                                            dynamic results2 = JsonConvert.DeserializeObject<dynamic>(apiResult2.content);
                                            if (results2.data != null && results2.data.consignment_no != null && results2.data.consignment_no != "")
                                            {
                                                order.MVD = results2.data.consignment_no;
                                                Global.AddLog("OrderID: " + order.OrderId + ". Mã vận đơn: "+ order.MVD);

                                                // Chụp ảnh hóa đơn
                                                Global.driver.Navigate().GoToUrl("https://banhang.shopee.vn/api/v3/logistics/get_waybill_new/?order_ids=" + order.OrderId);
                                                try
                                                {
                                                    Global.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("body")));
                                                    Thread.Sleep(1000);
                                                    Bitmap bmpImage = Helper.ScreenshotWayBill();
                                                    string savePath = @"C:\Users\Admin\Desktop\ma_van_don\" + order.OrderId + ".jpg";
                                                    bmpImage.Save(savePath);
                                                    Thread.Sleep(200);
                                                    // Upload hóa đơn lên server
                                                    order.MVDImage = Global.api.UploadImageToMyServer(savePath);
                                                    Global.AddLog("Upload anh " + order.MVDImage);
                                                    
                                                }
                                                catch (Exception e)
                                                {
                                                    Global.AddLog("ERROR: Co loi xay ra khi upload don hang " + e.Message + e.StackTrace);
                                                }
                                                Global.AddLog("OrderID: " + order.OrderId + ". Đã lấy xong mã vận đơn và lưu ảnh ở lần thứ " + x);

                                                // GỬI TIN NHẮN YÊU CẦU KHÁCH XÁC NHẬN ĐƠN
                                                Random random = new Random();
                                                int randomBetween1000And9999 = random.Next(1000, 9999);
                                                string captchaFile = Helper.GenCaptcha(randomBetween1000And9999.ToString());
                                                string url = PostImageToShopeeChat(captchaFile);
                                                if(url != "")
                                                {
                                                    SendChatToShopee(order.BuyerUser.UserId.ToString(), "text", "Cảm ơn Quý khách đã đặt hàng. Quý khách vui lòng trả lời tin nhắn với nội dung \"" + randomBetween1000And9999 + "\" để xác nhận đơn hàng. Trân trọng cảm ơn!");
                                                } else
                                                {
                                                    SendChatToShopee(order.BuyerUser.UserId.ToString(), "image", url);
                                                    Thread.Sleep(500);
                                                    SendChatToShopee(order.BuyerUser.UserId.ToString(), "text", "Cảm ơn Quý khách đã đặt hàng. Quý khách vui lòng trả lời tin nhắn với nội dung là số được ghi trong ảnh để xác nhận đơn hàng. Trân trọng cảm ơn!");
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Global.AddLog("ERROR: OrderID: " + order.OrderId + ". Lỗi khi submit phương thức vận chuyển (tự mang hàng đến bưu cục)");
                                return "error";
                            }
                            break;
                        }

                        // Thêm đơn vào danh sách bắn lên server trước khi break;
                        ordersSendToServer.Add(order);
                        break;
                    default:
                        ordersSendToServer.Add(order);
                        break;
                }
                
            }


            // BẮN ĐƠN LÊN SERVER
            ApiResult apiResult3;
            Dictionary<string, string> parameters3 = new Dictionary<string, string>
            {
                ["route"] = "order",
                ["orders"] = JsonConvert.SerializeObject(ordersSendToServer)
            };

            apiResult3 = Global.api.RequestMyApi(parameters3, Method.POST);
            if (apiResult3.success)
            {
                dynamic bloblah = JsonConvert.DeserializeObject<dynamic>(apiResult3.content);
                Global.AddLog("Đã bắn thông tin order lên server");
                return "success";
            }
            else
            {
                Global.AddLog("ERROR: Lỗi khi bắn order lên server ");
                return "error";
            }
        }
    }
}
