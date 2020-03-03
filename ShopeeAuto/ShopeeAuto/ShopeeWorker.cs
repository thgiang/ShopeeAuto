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
        private static dynamic cookiesShopeeToFake;

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
                GetCookieShopeeToFake();
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
                    GetCookieShopeeToFake();
                    return true;
                }
                // Lỗi khi gọi lên server lấy username, pass
                else
                {
                    Global.AddLog("Lỗi lấy username, pass từ server: " + results.message);
                    return false;
                }
            }

            return true;
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
            Global.AddLog("Mã ngành hàng: " + results.categories[2].catid + "\n");
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





        public static void GetCookieShopeeToFake()
        {
            Thread.Sleep(6000);
            Global.driver.Navigate().GoToUrl("https://banhang.shopee.vn/api/v1/unpublishedProducts/?limit=500&offset=0&subtype=add_attribute&type=unpublished");
            cookiesShopeeToFake = Global.driver.Manage().Cookies.AllCookies;
            Global.AddLog("Get cookie Shopee thành công");
        }

        public static void ClickSaveAllButton()
        {
            Global.wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(".shopee-button.shopee-button--inactive.shopee-button--primary.shopee-button--medium.shopee-button--aa.ember-view"))).Click();
        }

        public static void FakeCookieShopee(RestRequest request)
        {
            foreach (OpenQA.Selenium.Cookie cookie in cookiesShopeeToFake)
            {
                request.AddCookie(cookie.Name, cookie.Value.TrimEnd('"').TrimStart('"'));
                request.AddCookie("SPC_CDS", "GICUNGDUOC");
            }
        }

        public static string GetAttributeModelID(string CatID)
        {
            var client = new RestClient("https://banhang.shopee.vn/api/v2/categories/attributes/?catids=[" + CatID + "]");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            FakeCookieShopee(request);
            IRestResponse response = client.Execute(request);
            dynamic results = JsonConvert.DeserializeObject<dynamic>(response.Content);
            return results.categories[0].meta.modelid;
        }

        public static void AddLogistics()
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

            Thread.Sleep(3000);
            PublishProducts();
        }

        public static void PublishProducts()
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

            Thread.Sleep(3000);
        }

        public static string PostImageToShopee(string path)
        {
            // Đoạn này là request bằng cookie nick Shopee của em
            var client = new RestClient("https://banhang.shopee.vn/api/v3/general/upload_image/?SPC_CDS=8c777714-50b7-4017-82bd-3a5141424b85&SPC_CDS_VER=2");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            FakeCookieShopee(request);

            request.AddFile("file", File.ReadAllBytes(path), Path.GetFileName(path));
            IRestResponse response = client.Execute(request);
            dynamic results = JsonConvert.DeserializeObject<dynamic>(response.Content);
            string resource_id = results.data.resource_id;
            return resource_id;
        }


        public static string DownloadImage(string PictureUrl)
        {
            string Path_In_PC = @"C:\Users\Admin\Desktop\image_to_search.jpg";
            if (PictureUrl.Contains("http") == false)
            {
                PictureUrl = "https:" + PictureUrl;
            }
            MemoryStream ms = new MemoryStream();
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(PictureUrl, Path_In_PC);
                return Path_In_PC;
            }
        }

        public static string CopyImagesProductFromTaobaoToShopee(string ID_Product_Taobao)
        {
            var client = new RestClient("https://laonet.online/index.php?route=api_tester/call&api_name=item_get&lang=vi&num_iid=" + ID_Product_Taobao + "&key=profile.nvt@gmail.com");
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            dynamic results = JsonConvert.DeserializeObject<dynamic>(response.Content);
            dynamic item_imgs = results.item.item_imgs;

            //string[] ImagesListInShopee = new string[999];
            string ImagesListInShopee = "[";
            int i = 0;
            foreach (dynamic Image in item_imgs)
            {
                if (i >= 7)
                {
                    break;
                }
                ImagesListInShopee += "\"" + PostImageToShopee(DownloadImage(Image.url.ToString())) + "\",";
                i++;
                //ImagesListInShopee[i] = ShopeeController.PostImageToShopee(MainController.DownloadImage(Image.url.ToString()));
            }
            ImagesListInShopee = ImagesListInShopee.TrimEnd(',') + "]";
            return ImagesListInShopee;
        }

        public static dynamic GetProductSKUs(string TaobaoProductID)
        {
            Global.AddLog("Bắt đầu lấy danh sách SKU của sản phẩm");

            var client = new RestClient(Global.TaobaoURLAPI + "&api_name=item_get&num_iid=" + TaobaoProductID);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            dynamic results = JsonConvert.DeserializeObject<dynamic>(response.Content).item;
            dynamic ListSKUs = results.skus.sku;

            dynamic tier_variation = new ExpandoObject();
            dynamic model_list = new List<dynamic>() { };

            tier_variation.name = "Mẫu Mã";
            tier_variation.options = new List<string>() { };
            int index = 0;


            foreach (dynamic SKUData in ListSKUs)
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

            dynamic ResponseData = new ExpandoObject();
            ResponseData.tier_variation = JsonConvert.SerializeObject(tier_variation, Formatting.Indented);
            ResponseData.model_list = JsonConvert.SerializeObject(model_list, Formatting.Indented);

            //Global.AddLog("\n\n tier_variation \n\n" + JsonConvert.SerializeObject(tier_variation, Formatting.Indented) + "\n\n");
            //Global.AddLog("\n\n model_list \n\n" + JsonConvert.SerializeObject(model_list, Formatting.Indented) + "\n\n");
            Global.AddLog("Lấy thành công danh sách SKU");
            Global.AddEndProcessLog();
            return ResponseData;
        }


        public string PublishOnlyOneProduct(dynamic ProductDataFromShopee, dynamic ProductDataFromTaobao)
        {
            string ProductName, ProductIDInTaobao, ProductImages, ProductModelList, ProductTiervariation, CategoryPath, Description, AttributeModelID, AttributesData, Stock, Price, Weight, Width, Height, Length, DaysToShip, ProductLogisticData;

            ProductName = Global.FirstLetterToUpper(ProductDataFromShopee.name.ToString());
            ProductIDInTaobao = ProductDataFromTaobao.num_iid;

            Global.AddLog("Bắt đầu upload sản phẩm " + ProductIDInTaobao + " từ Taobao lên Shopee");
            Global.AddLog("Chuẩn bị dữ liệu để up");

            ProductImages = CopyImagesProductFromTaobaoToShopee(ProductIDInTaobao);
            CategoryPath = "[" + ProductDataFromShopee.categories[0].catid + "," + ProductDataFromShopee.categories[1].catid + "," + ProductDataFromShopee.categories[2].catid + "]";
            AttributeModelID = GetAttributeModelID(ProductDataFromShopee.categories[2].catid.ToString());

            dynamic SKUProductData = GetProductSKUs(ProductIDInTaobao);
            ProductModelList = SKUProductData.model_list.ToString();
            ProductTiervariation = SKUProductData.tier_variation.ToString();

            Description = ProductDataFromShopee.description.ToString().Replace("\n", @"\n");
            AttributesData = "";
            Weight = "600";
            Stock = "9999";
            Price = ((float)ProductDataFromTaobao.price * 3400).ToString();
            Width = "6";
            Height = "7";
            Length = "9";
            DaysToShip = "7";
            ProductLogisticData = "{\"price\":\"0.00\",\"cover_shipping_fee\":false,\"enabled\":true,\"channelid\":50018,\"sizeid\":0},{\"price\":\"9000.00\",\"cover_shipping_fee\":false,\"enabled\":true,\"channelid\":50011,\"sizeid\":0},{\"price\":\"8000.00\",\"cover_shipping_fee\":false,\"enabled\":true,\"channelid\":50016,\"sizeid\":0},{\"price\":\"9000.00\",\"cover_shipping_fee\":false,\"enabled\":true,\"channelid\":50012,\"sizeid\":0},{\"price\":\"8000.00\",\"cover_shipping_fee\":false,\"enabled\":true,\"channelid\":50015,\"sizeid\":0},{\"price\":\"9000.00\",\"cover_shipping_fee\":false,\"enabled\":true,\"channelid\":50010,\"sizeid\":0}],\"unlisted\":false,\"add_on_deal\":[]}";

            var client = new RestClient("https://banhang.shopee.vn/api/v3/product/create_product/?version=3.1.0&SPC_CDS=GICUNGDUOC&SPC_CDS_VER=2");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json;charset=UTF-8");
            FakeCookieShopee(request);

            Global.AddLog("Bắt đầu up");

            //Global.AddLog("attribute_model_id: \n" + AttributeModelID + "\n\n";
            //Global.AddLog("attributes: \n" + ProductDataFromShopee.attributes + "\n\n";
            //Global.AddLog("images: \n" + ProductImages + "\n\n";
            //Global.AddLog("name: \n" + ProductName + "\n\n";
            //Global.AddLog("category_path: \n" + CategoryPath + "\n\n";
            //Global.AddLog("description: \n" + Description + "\n\n";

            string DataPost = "[{\"id\":0,\"name\":\"" + ProductName + "\",\"brand\":\"No Brand\",\"images\":" + ProductImages + ",\"description\":\"" + @Description + "\",\"model_list\":" + ProductModelList + ",\"category_path\":" + CategoryPath + ",\"attribute_model\":{\"attribute_model_id\":" + AttributeModelID + ",\"attributes\":[" + AttributesData + "]},\"category_recommend\":[],\"stock\":" + Stock + ",\"price\":\"" + Price + "\",\"price_before_discount\":\"\",\"parent_sku\":\"\",\"wholesale_list\":[],\"installment_tenures\":{},\"weight\":\"" + Weight + "\",\"dimension\":{\"width\":" + Width + ",\"height\":" + Height + ",\"length\":" + Length + "},\"pre_order\":true,\"days_to_ship\":" + DaysToShip + ",\"condition\":1,\"size_chart\":\"\",\"tier_variation\":[" + ProductTiervariation + "],\"logistics_channels\":[" + ProductLogisticData + "]";

            request.AddParameter("application/json;charset=UTF-8", DataPost, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            dynamic results = JsonConvert.DeserializeObject<dynamic>(response.Content);

            Global.AddLog("Datapost: \n\n" + DataPost + "\n\n");
            Global.AddLog("\n\n results: \n\n" + results + "\n\n");
            try
            {
                if (results.message == "success")
                {
                    dynamic SuccessProductID = results.data.result[0].data.product_id;
                    Global.AddLog("Upload thành công, ID sản phẩm mới ở Shopee là:" + SuccessProductID);
                    Global.AddEndProcessLog();
                    //driver.Navigate().GoToUrl("https://banhang.shopee.vn/portal/product/list/all");
                    return SuccessProductID;
                }
                else
                {
                    Global.AddLog("Upload thất bại, nội dung trả về là:" + results.data.result[0].message.ToString());
                    Global.AddEndProcessLog();
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
