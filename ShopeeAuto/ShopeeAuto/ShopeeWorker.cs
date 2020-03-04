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

namespace ShopeeAuto
{
    class ShopeeWorker
    {
        private Helper helper = new Helper();
        private dynamic shopeeCookie;

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
                // Lấy cookie trước khi return Login thành công
                shopeeCookie = Global.driver.Manage().Cookies.AllCookies;
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
                    loginForm.FindElements(By.TagName("input"))[0].SendKeys(results.data.shopee_username.ToString()); // Username
                    loginForm.FindElements(By.TagName("input"))[1].SendKeys(results.data.shopee_password.ToString()); // Password
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
                    // Lấy cookie trước khi return Login thành công
                    shopeeCookie = Global.driver.Manage().Cookies.AllCookies;
                    return true;
                }
                // Lỗi khi gọi lên server lấy username, pass
                else
                {
                    Global.AddLog("Lỗi lấy username, pass từ server: " + results.message);
                    return false;
                }
            }
            // Lấy cookie trước khi return Login thành công
            shopeeCookie = Global.driver.Manage().Cookies.AllCookies;
            return true;
        }

        // Gọi hàm này trước khi request
        public void FakeshopeeCookie(RestRequest request)
        {
            foreach (OpenQA.Selenium.Cookie cookie in shopeeCookie)
            {
                request.AddCookie(cookie.Name, cookie.Value.TrimEnd('"').TrimStart('"'));
                request.AddCookie("SPC_CDS", "GICUNGDUOC");
            }
        }


        public dynamic GetProductData(string itemId, string shopId)
        {
            var client = new RestClient("https://shopee.vn/api/v2/item/get?itemid=" + itemId + "&shopid=" + shopId);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            dynamic results = JsonConvert.DeserializeObject<dynamic>(response.Content).item;
            if(results == null)
            {
                return null;
            }
            /*
            Global.AddLog("Lấy data sản phẩm thành công \n\n");
            Global.AddLog("Mã ngành hàng: " + results.categories[2].catId + "\n");
            Global.AddLog("Tên sản phẩm: " + results.name + "\n");
            Global.AddLog("Mô tả sản phẩm: " + results.description + "\n");
            Global.AddLog("Giá bán hiện tại: " + results.price + "\n");
            Global.AddLog("Tổng số đã bán: " + results.historical_sold + "\n");
            Global.AddLog("Thuộc tính sản phẩm: " + results.attributes + "\n");
            Global.AddLog("Phân loại sản phẩm: " + results.models + "\n");
            */
            //Global.AddLog(results + "\n";
            return results;
        }

        public void ClickSaveAllButton()
        {
            Global.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(".shopee-button.shopee-button--inactive.shopee-button--primary.shopee-button--medium.shopee-button--aa.ember-view"))).Click();
        }


        public string GetattributeModeId(string catId)
        {
            var client = new RestClient("https://banhang.shopee.vn/api/v2/categories/attributes/?catIds=[" + catId + "]");
            //client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            FakeshopeeCookie(request);
            IRestResponse response = client.Execute(request);
            dynamic results = JsonConvert.DeserializeObject<dynamic>(response.Content);
            return results.categories[0].meta.modelid;
        }

        public void AddLogistics()
        {
            try
            {
                Global.wait.Until(ExpectedConditions.UrlToBe("https://banhang.shopee.vn/portal/product/list/oldunpublished?subtype=add_logistics"));
                ClickSaveAllButton();
            }
            catch (WebDriverTimeoutException)
            {
                Global.driver.Navigate().GoToUrl("https://banhang.shopee.vn/portal/product/list/oldunpublished?subtype=add_logistics");
                ClickSaveAllButton();
            }

            PublishProducts();
        }

        public void PublishProducts()
        {
            try
            {
                Global.wait.Until(ExpectedConditions.UrlToBe("https://banhang.shopee.vn/portal/product/list/oldunpublished?subtype=ready_publish"));
                ClickSaveAllButton();
            }
            catch (WebDriverTimeoutException)
            {
                Global.driver.Navigate().GoToUrl("https://banhang.shopee.vn/portal/product/list/oldunpublished?subtype=ready_publish");
                ClickSaveAllButton();
            }
        }

        public string PostImageToShopee(string path)
        {
            // Đoạn này là request bằng cookie nick Shopee của em
            var client = new RestClient("https://banhang.shopee.vn/api/v3/general/upload_image/?SPC_CDS=8c777714-50b7-4017-82bd-3a5141424b85&SPC_CDS_VER=2");
            //client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            FakeshopeeCookie(request);

            request.AddFile("file", File.ReadAllBytes(path), Path.GetFileName(path));
            IRestResponse response = client.Execute(request);
            dynamic results = JsonConvert.DeserializeObject<dynamic>(response.Content);
            string resource_id = results.data.resource_id;
            return resource_id;
        }


       
        // Copy ảnh từ Taobao sang Shopee, trả về List<string> md5 của ảnh đã đăng lên shopee
        public string[] TaobaoProductImagesToShopee(string taobaoId)
        {
            
            var client = new RestClient(Global.api.laoNetApi + "&api_name=item_get&lang=vi&num_iid=" + taobaoId);
            //client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            dynamic results = JsonConvert.DeserializeObject<dynamic>(response.Content);
            dynamic item_imgs = results.item.item_imgs;

            List<string> shopeeImages = new List<string>();
            // Lấy tối đa 7 ảnh của Taobao đăng sang shopee
            for (int i = 0; i < Math.Min(7, item_imgs.Count); i++)
            {
                shopeeImages.Add(PostImageToShopee(helper.DownloadImage(item_imgs[i].url.ToString())));
            }
            return shopeeImages.ToArray();
        }

        // Lấy SKU của sản phẩm taobao
        public dynamic GetTaobaoProductSKUs(string taobaoId)
        {
            Global.AddLog("Bắt đầu lấy danh sách SKU của sản phẩm");

            var client = new RestClient(Global.api.laoNetApi + "&api_name=item_get&num_iid=" + taobaoId);
            // client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            dynamic results = JsonConvert.DeserializeObject<dynamic>(response.Content).item;
            dynamic listSKUs = results.skus.sku;

            dynamic tier_variation = new ExpandoObject();
            dynamic model_list = new List<dynamic>() { };

            tier_variation.name = "Mẫu Mã";
            tier_variation.options = new List<string>() { };
            int index = 0;


            foreach (dynamic SKUData in listSKUs)
            {
                string[] SKUProps = SKUData.properties.ToString().Split(new string[] { ";" }, StringSplitOptions.None);
                string SKUPropName = "";
                dynamic CurrenSKUData = new ExpandoObject();

                foreach (string SKUProp in SKUProps)
                {
                    SKUPropName += Global.FirstLetterToUpper(results.props_list[@SKUProp].ToString());

                    //Global.AddLog(SKUProp + " có nghĩa là " + SKUPropName);

                    if (!SKUProp.Equals(SKUProps.Last()))
                    {
                        SKUPropName += " - ";
                    }
                }

                if (SKUData.quantity != null)
                {
                    CurrenSKUData.id = 0;
                    CurrenSKUData.name = "";
                    CurrenSKUData.stock = Int32.Parse(SKUData.quantity.ToString());
                    CurrenSKUData.price = (Convert.ToDouble(SKUData.price) * 1000).ToString();
                    CurrenSKUData.sku = SKUData.sku_id.ToString();
                    //CurrenSKUData.sku = SKUData.sku_id;
                    CurrenSKUData.tier_index = new List<int>() { index };

                    model_list.Add(CurrenSKUData);
                }

                Global.AddLog(SKUPropName + " => " + Global.SimpleTranslate(SKUPropName));
                SKUPropName = Global.SimpleTranslate(SKUPropName);
                tier_variation.options.Add(SKUPropName.ToString());
                index++;
                //Global.AddLog("SKU ID: " + SKUData.sku_id + ", Thuộc tính: " + SKUPropName + ", Trong kho còn " + SKUData.quantity + " sản phẩm ");
            }
            tier_variation.images = new List<string>() { };

            dynamic responseData = new ExpandoObject();
            responseData.tier_variation = JsonConvert.SerializeObject(tier_variation, Formatting.Indented);
            responseData.model_list = JsonConvert.SerializeObject(model_list, Formatting.Indented);
            Global.AddLog("Lấy SKU xong!");
            return responseData;
        }


        public string PublishOnlyOneProduct(dynamic shopeeProductInfo, dynamic taobaoProductInfo)
        {
            Random random = new Random();
            string taobaoId = taobaoProductInfo.num_iid;

            Global.AddLog("Bắt đầu upload sản phẩm " + taobaoId + " từ Taobao lên Shopee");
            Global.AddLog("Chuẩn bị dữ liệu để up");

            // Data mẫu
            string postDataString = "{\"id\":0,\"name\":\"productName\",\"brand\":\"No Brand\",\"description\":\"Description\",\"model_list\":\"\",\"category_path\":\"\",\"attribute_model\":{\"attribute_model_id\":\"\",\"attributes\":[]},\"category_recommend\":[],\"stock\": 30,\"price\":300000,\"price_before_discount\":\"\",\"parent_sku\":\"\",\"wholesale_list\":[],\"installment_tenures\":{},\"weight\":600,\"dimension\":{\"width\":6,\"height\":7,\"length\":9},\"pre_order\":true,\"days_to_ship\":7,\"condition\":1,\"size_chart\":\"\",\"tier_variation\":[],\"logistics_channels\":[{\"price\":\"0.00\",\"cover_shipping_fee\":false,\"enabled\":true,\"channelid\":50018,\"sizeid\":0},{\"price\":\"9000.00\",\"cover_shipping_fee\":false,\"enabled\":true,\"channelid\":50011,\"sizeid\":0},{\"price\":\"8000.00\",\"cover_shipping_fee\":false,\"enabled\":true,\"channelid\":50016,\"sizeid\":0},{\"price\":\"9000.00\",\"cover_shipping_fee\":false,\"enabled\":true,\"channelid\":50012,\"sizeid\":0},{\"price\":\"8000.00\",\"cover_shipping_fee\":false,\"enabled\":true,\"channelid\":50015,\"sizeid\":0},{\"price\":\"9000.00\",\"cover_shipping_fee\":false,\"enabled\":true,\"channelid\":50010,\"sizeid\":0}],\"unlisted\":false,\"add_on_deal\":[]}";
            dynamic postData = JsonConvert.DeserializeObject<ExpandoObject>(postDataString);
            string[] images = TaobaoProductImagesToShopee(taobaoId);

            // Đẩy data thật vào
            postData.name                               = Global.FirstLetterToUpper(shopeeProductInfo.name.ToString());
            postData.images                             = images;
            postData.description                        = shopeeProductInfo.description.ToString().Replace("\n", @"\n");
            postData.category_path                      = "[" + shopeeProductInfo.categories[0].catId + "," + shopeeProductInfo.categories[1].catId + "," + shopeeProductInfo.categories[2].catId + "]";
            postData.attribute_model.attribute_model_id = GetattributeModeId(shopeeProductInfo.categories[2].catid.ToString());
            postData.attribute_model.attributes         = "";
            postData.model_list                         = GetTaobaoProductSKUs(taobaoId).model_list.ToString();
            postData.price                              = shopeeProductInfo.price - random.Next(1, 5) * 1000;


            var client = new RestClient("https://banhang.shopee.vn/api/v3/product/create_product/?version=3.1.0&SPC_CDS=GICUNGDUOC&SPC_CDS_VER=2");
            //client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json;charset=UTF-8");
            FakeshopeeCookie(request);

            Global.AddLog("Bắt đầu up sản phẩm");
            postDataString = JsonConvert.SerializeObject(postData);
            request.AddParameter("application/json;charset=UTF-8", postDataString, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            dynamic results = JsonConvert.DeserializeObject<dynamic>(response.Content);

            //Global.AddLog("postDataString: \n\n" + postDataString + "\n\n");
            Global.AddLog("\n\n results: \n\n" + results + "\n\n");
            try
            {
                if (results.message == "success")
                {
                    dynamic SuccessProductID = results.data.result[0].data.product_id;
                    Global.AddLog("Upload thành công, ID sản phẩm mới ở Shopee là:" + SuccessProductID);
                    Global.AddLog("===============================");
                    //driver.Navigate().GoToUrl("https://banhang.shopee.vn/portal/product/list/all");
                    return SuccessProductID;
                }
                else
                {
                    Global.AddLog("Upload thất bại, nội dung trả về là:" + results.data.result[0].message.ToString());
                    Global.AddLog("===============================");
                    return "error";
                }
            }
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
            {
                return "error";
            }
        }
    }
}
