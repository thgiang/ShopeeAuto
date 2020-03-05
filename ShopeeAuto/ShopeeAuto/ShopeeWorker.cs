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
        private int minRevenue = 30;
        private int maxRevenue = 70;
        private string username;
        private string password;

        public bool Login()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            dynamic results = new ExpandoObject();
            // Lấy thông tin username và pass từ server
            parameters["route"] = "client/info";
            results = Global.api.Request(parameters);

            // Lấy được user và pass, tiến hành login vào shopee
            if (results.status == "success")
            {
                minRevenue = int.Parse(results.data.shopee_min_revenue.ToString());
                maxRevenue = int.Parse(results.data.shopee_max_revenue.ToString());
                username = results.data.shopee_username.ToString();
                password = results.data.shopee_password.ToString();

            }
            // Lỗi khi gọi lên server lấy username, pass
            else
            {
                Global.AddLog("Lỗi lấy username, pass từ server: " + results.message);
                return false;
            }

           


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
                IWebElement loginForm = Global.driver.FindElement(By.CssSelector("form.signin-form"));
                loginForm.FindElements(By.TagName("input"))[0].SendKeys(username); // Username
                loginForm.FindElements(By.TagName("input"))[1].SendKeys(password); // Password
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

        // Lấy thông tin sản phẩm Shopee
        public dynamic GetShopeeProductData(string itemId, string shopId)
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
            return results;
        }

        // Lấy thông tin sản phẩm taobao
        public dynamic GetTaobaoProductData(string taobaoId)
        {
            var client = new RestClient(Global.api.laoNetApi + "&api_name=item_get&num_iid=" + taobaoId);
            // client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            dynamic results = JsonConvert.DeserializeObject<dynamic>(response.Content);
            return results.item;
        }

        // Mỗi category có 1 model (form nhập thông tin) khác nhau. Cần lấy model_id sau đó lấy form để biết đc nó yêu cầu nhập những thông tin gì
        // Tạm thời gọi thẳng API để post nên có thể qua mặt đc phần nhập thông tin form, nhưng vẫn cần truyền đúng model_id
        public int GetAttributeModelId(string catId)
        {
            var client = new RestClient("https://banhang.shopee.vn/api/v3/category/get_category_attributes?category_ids=" + catId);
            //client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            FakeshopeeCookie(request);
            IRestResponse response = client.Execute(request);
            dynamic results = JsonConvert.DeserializeObject<dynamic>(response.Content);
            return int.Parse(results.data.list[0].attribute_model_id.ToString());
        }

        // Đăng ảnh từ máy mình lên shopee, nhận lại id của ảnh shopee đã lưu
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


       
        public class AllImages
        {
            public List<string> generalImgs;
            public Dictionary<string, string> SKUImages;
            public Dictionary<string, string> uploadedImages;
        }
        // Copy ảnh từ Taobao sang Shopee, trả về một object gồm 2 phần tử: mảng generalImgs và Dictionary SKUIImages
        public AllImages UploadTaobaoImagesToShopee(dynamic taobaoProductInfo)
        {
            AllImages allImages = new AllImages();
            Dictionary<string, string> uploadedImages = new Dictionary<string, string>();
            Dictionary<string, string> SKUImages = new Dictionary<string, string>();
            List<string> generalImgs = new List<string>();

            dynamic item_imgs = taobaoProductInfo.item_imgs;
            dynamic prop_imgs = taobaoProductInfo.prop_imgs.prop_img;

            string shopeeMd5;
            foreach (dynamic prop_img in prop_imgs)
            {
                // Nếu chưa up thì up
                if (!uploadedImages.ContainsKey(prop_img.url.ToString()))
                {
                    shopeeMd5 = PostImageToShopee(helper.DownloadImage(prop_img.url.ToString()));
                    uploadedImages[prop_img.url.ToString()] = shopeeMd5;
                }
                SKUImages[prop_img.properties.ToString()] = uploadedImages[prop_img.url.ToString()];                 
            }

            foreach (dynamic item_img in item_imgs)
            {
                // Nếu chưa up thì up rồi cho vào List
                if (!uploadedImages.ContainsKey(item_img.url.ToString())) {
                    shopeeMd5 = PostImageToShopee(helper.DownloadImage(item_img.url.ToString()));
                    uploadedImages[item_img.url.ToString()] = shopeeMd5;
                    generalImgs.Add(shopeeMd5);
                }
            }

            allImages.generalImgs = generalImgs;
            allImages.SKUImages = SKUImages;
            allImages.uploadedImages = uploadedImages;
            return allImages;

        }

        // Lấy SKU của sản phẩm taobao, đưa nó về đúng form mà shopee yêu cầu
        public dynamic BuildShopeeSKUBasedOnTaobao(dynamic taobaoProductInfo, AllImages allImages, float revenuePercent, int weight)
        {
            Global.AddLog("Bắt đầu lấy danh sách SKU của sản phẩm");          
            dynamic listSKUs = taobaoProductInfo.skus.sku;

            dynamic tier_variation = new ExpandoObject();
            dynamic model_list = new List<dynamic>() { };

            // Lưu lại tránh trường hợp trùng SKUName thì phải thêm số vào đuôi
            List<string> SKUNames = new List<string>();

            tier_variation.name = "Mẫu Mã";
            tier_variation.options = new List<string>() { };
            tier_variation.images = new List<string>() { };
            int index = 0;
            foreach (dynamic SKUData in listSKUs)
            {
                string[] SKUProps = SKUData.properties.ToString().Split(new string[] { ";" }, StringSplitOptions.None);
                string SKUPropName = "";
                dynamic CurrenSKUData = new ExpandoObject();

                if (SKUData.quantity != null)
                {
                    CurrenSKUData.id = 0;
                    CurrenSKUData.name = "";
                    // Đăng tối đa 79 sản phẩm vì mình thích thế, hihi
                    CurrenSKUData.stock = Math.Min(int.Parse(SKUData.quantity.ToString()), 79);

                    // Gọi lên API để tính cước vận chuyển để tính ra giá cuối cùng
                    float SKUPrice = SKUData.price * revenuePercent;
                    var client = new RestClient(Global.api.apiUrl + "shipping-fee-ns?amount=" + SKUPrice.ToString() + "&weight=" + weight.ToString());
                    Global.AddLog("Gọi lên API tính giá " + Global.api.apiUrl + "shipping-fee-ns?amount=" + SKUPrice.ToString() + "&weight=" + weight.ToString());
                    var request = new RestRequest(Method.GET);
                    IRestResponse response = client.Execute(request);
                    CurrenSKUData.price = JsonConvert.DeserializeObject<dynamic>(response.Content).final_price;
                    CurrenSKUData.price = ((int)CurrenSKUData.price / 1000 * 1000).ToString();

                    CurrenSKUData.sku = SKUData.sku_id.ToString();
                    CurrenSKUData.tier_index = new List<int>() { index };

                    // Thêm SKU vào model_list
                    model_list.Add(CurrenSKUData);

                    // Dịch SKU name (để sinh ra dạng Xanh - Size XL)
                    foreach (string SKUProp in SKUProps)
                    {
                        SKUPropName += Global.FirstLetterToUpper(taobaoProductInfo.props_list[@SKUProp].ToString());

                        if (!SKUProp.Equals(SKUProps.Last()))
                        {
                            SKUPropName += " - ";
                        }
                    }
                    SKUPropName = Global.SimpleTranslate(SKUPropName);

                    // Nếu bị trùng với một SKUName nào trước đó thì thêm chữ kiểu x
                    if (SKUNames.Contains(SKUPropName))
                    {
                        int alt = 2;
                        do {
                            SKUPropName += " kiểu " + alt;
                            alt++;
                        } while (SKUNames.Contains(SKUPropName));
                    }
                    SKUNames.Add(SKUPropName);

                    // Thêm SKU vào danh sách variation shopee
                    tier_variation.options.Add(SKUPropName.ToString());

                    // Tìm ảnh để thêm vào
                    if(allImages.SKUImages.Count > 0)
                    {
                        // Nếu sản phẩm taobao có SKU Images thì chọn ảnh tương ứng để add vào đúng thứ tự của Shopee
                        bool foundImage = false;
                        foreach(string SKUPropToImage in SKUProps)
                        {
                            if (allImages.SKUImages.ContainsKey(SKUPropToImage))
                            {
                                tier_variation.images.Add(allImages.SKUImages[SKUPropToImage]);
                                foundImage = true;
                                break;
                            }
                        }
                        // Trong trường hợp variation này ko có ảnh nào khớp thì phải chọn ngẫu nhiên 1 ảnh add vào để đảm bảo
                        // không bị sai thứ tự mảng
                        if (!foundImage)
                        {
                            foreach(var d in allImages.uploadedImages)
                            {
                                tier_variation.images.Add(d.Value);
                                break;
                            }
                        }
                    }
                    
                    Global.AddLog(SKUPropName + " => " + Global.SimpleTranslate(SKUPropName));
                    index++;
                    
                }               
            }
            

            dynamic responseData = new ExpandoObject();
            responseData.tier_variation = new List<dynamic> { tier_variation }.ToArray();
            responseData.model_list = model_list;
            Global.AddLog("Lấy SKU xong!");
            return responseData;
        }

        // Sinh ra một description đáng yêu ♥
        public string BeautifulDescription(dynamic postData, dynamic shopeeProductInfo, dynamic taobaoProductInfo)
        {
            string desciption = postData.name.ToString().ToUpper()+ @"
------------------------------------------------------
★ THÔNG TIN SẢN PHẨM
" + shopeeProductInfo.description.ToString().Replace(" ,", ", ").Replace(" .", ". ") + @"

★ CAM KẾT VÀ DỊCH VỤ
- Sản phẩm đảm bảo chất lượng, chính xác 100 % về thông số, mô tả và hình ảnh.
- Sản phẩm được nhập khẩu trực tiếp từ Trung Quốc.
- Thời gian giao hàng dự kiến: trong vòng 14 ngày làm việc kể từ ngày đặt hàng. Thông tin tracking được gửi tới Quý khách qua tin nhắn Shopee hàng ngày.
- Hình thức thanh toán: COD toàn quốc.
- Khách hàng đặt mua số lượng lớn vui lòng liên hệ trực tiếp để được giảm giá tới 20%.

★ THÔNG TIN LIÊN HỆ
☎ Mobile: 0969.546.294
📞 Zalo: 0969.546.294";

            return desciption;
        }
        public string CopyTaobaoToShopee(string shopeeId, string shopeeCategoryId, string taobaoId)
        {
            Global.driver.Navigate().GoToUrl("https://banhang.shopee.vn/portal/product/category");
            Random random = new Random();
            Global.AddLog("Bắt đầu upload sản phẩm " + taobaoId + " từ Taobao lên Shopee");
            Global.AddLog("Chuẩn bị dữ liệu để up");

            // Lấy data từ Shopee
            Global.AddLog("Lấy data từ Shopee");
            dynamic shopeeProductInfo = GetShopeeProductData(shopeeId, shopeeCategoryId);
            if (shopeeProductInfo == null)
            {
                return "error";
            };
            Global.AddLog("Lấy data từ Shopee xong");

            // Lấy data từ Taobao
            Global.AddLog("Lấy data từ Taobao");
            dynamic taobaoProductInfo = GetTaobaoProductData(taobaoId);
            if (taobaoProductInfo == null)
            {
                return "error";
            };
            Global.AddLog("Lấy data từ Taobao xong");
            // Data mẫu
            string postDataString = "{\"id\":0,\"name\":\"Boo loo ba la\",\"brand\":\"No brand\",\"images\":[],\"description\":\"Không được để trống\",\"model_list\":[],\"category_path\":[],\"attribute_model\":{\"attribute_model_id\":15159,\"attributes\":[{\"attribute_id\":13054,\"prefill\":false,\"status\":0,\"value\":\"No brand\"},{\"attribute_id\":20074,\"prefill\":false,\"status\":0,\"value\":\"1 Tháng\"}]},\"category_recommend\":[],\"stock\":0,\"price\":\"123000\",\"price_before_discount\":\"\",\"parent_sku\":\"SKU chỗ này là cái gì vậy?\",\"wholesale_list\":[],\"installment_tenures\":{},\"weight\":\"200\",\"dimension\":{\"width\":10,\"height\":10,\"length\":20},\"pre_order\":true,\"days_to_ship\":12,\"condition\":1,\"size_chart\":\"\",\"tier_variation\":[],\"logistics_channels\":[{\"price\":\"0.00\",\"cover_shipping_fee\":false,\"enabled\":true,\"channelid\":50018,\"sizeid\":0},{\"price\":\"8000.00\",\"cover_shipping_fee\":false,\"enabled\":true,\"channelid\":50016,\"sizeid\":0},{\"price\":\"9000.00\",\"cover_shipping_fee\":false,\"enabled\":true,\"channelid\":50011,\"sizeid\":0},{\"price\":\"9000.00\",\"cover_shipping_fee\":false,\"enabled\":true,\"channelid\":50012,\"sizeid\":0},{\"price\":\"8000.00\",\"cover_shipping_fee\":false,\"enabled\":true,\"channelid\":50015,\"sizeid\":0},{\"price\":\"9000.00\",\"cover_shipping_fee\":false,\"enabled\":true,\"channelid\":50010,\"sizeid\":0}],\"unlisted\":false,\"add_on_deal\":[],\"ds_cat_rcmd_id\":\"0\"}";
            dynamic postData = JsonConvert.DeserializeObject<ExpandoObject>(postDataString);
            AllImages allImages = UploadTaobaoImagesToShopee(taobaoProductInfo);

            // Tính toán data thật
            // Lấy kích thước, cân nặng của sản phẩm
            Global.AddLog("Lấy kích thước sản phẩm");
            var client = new RestClient("https://shopee.vn/api/v0/shop/"+shopeeCategoryId+"/item/"+shopeeId+"/shipping_info_to_address/?city=Huy");
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            dynamic shopeeShippings = JsonConvert.DeserializeObject<dynamic>(response.Content);
            if (shopeeShippings == null)
            {
                Global.AddLog("Lỗi khi lấy thông tin kích thước sản phẩm");
                return "error";
            }


            // Tính giá TB các SKU Shopee
            Global.AddLog("Tính giá trung bình mỗi SKU Shopee");
            string p; ;
            p = shopeeProductInfo.price_max.ToString();
            int shopeePrice = int.Parse(p.Substring(0, p.Length - 5));
            if (shopeeProductInfo.models.Count > 0) {
                shopeePrice = 0;
                foreach(dynamic m in shopeeProductInfo.models)
                {
                    p = m.price.ToString();
                    shopeePrice += int.Parse(p.Substring(0, p.Length - 5));
                }
                shopeePrice = shopeePrice / shopeeProductInfo.models.Count;
            }
            if(shopeePrice == 0)
            {
                Global.AddLog("STOP: ShopeePrice = 0");
                return "error";
            }

            // Tính giá TB các SKU Taobao
            Global.AddLog("Tính giá trung bình mỗi SKU Taobao");
            int taobaoPrice = (int)Convert.ToDouble(taobaoProductInfo.price.ToString());
            if (taobaoProductInfo.skus.sku.Count > 0)
            {
                taobaoPrice = 0;
                foreach (dynamic s in taobaoProductInfo.skus.sku)
                {
                    Global.AddLog(s.price.ToString());
                    taobaoPrice += (int)Convert.ToDouble(s.price.ToString());                    
                }
                taobaoPrice = taobaoPrice / taobaoProductInfo.skus.sku.Count;
            }

            // Gọi lên API để tính cước vận chuyển để tính ra giá cuối cùng
            Global.AddLog("Gọi lên API để tính tỉ giá TQ");
            int weight = (shopeeShippings.shipping_infos[0].debug.total_weight * 100);
            client = new RestClient(Global.api.apiUrl+ "shipping-fee-ns?amount="+ taobaoPrice.ToString()+ "&weight="+ weight.ToString());
            request = new RestRequest(Method.GET);
            response = client.Execute(request);
            taobaoPrice = JsonConvert.DeserializeObject<dynamic>(response.Content).final_price;

            // Giá bán ra
            int outPrice = Math.Max(taobaoPrice * (100 + minRevenue) / 100, (shopeePrice  - random.Next(1, 5) * 1000));

            float revenuePercent;
            revenuePercent = (float)outPrice / (float)taobaoPrice;

            
            dynamic sku = BuildShopeeSKUBasedOnTaobao(taobaoProductInfo, allImages, revenuePercent, weight);
            List<int> categoryPath = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                categoryPath.Add(int.Parse(shopeeProductInfo.categories[i].catid.ToString()));
            }

            // Đẩy data thật vào object
            postData.name                               = Global.FirstLetterToUpper(shopeeProductInfo.name.ToString());
            postData.images                             = allImages.generalImgs;
            postData.category_path                      = categoryPath;
            postData.attribute_model.attribute_model_id = GetAttributeModelId(shopeeProductInfo.categories[2].catid.ToString());
            postData.attribute_model.attributes         = new List<string>();
            postData.price                              = outPrice.ToString();
            postData.tier_variation                     = sku.tier_variation;
            postData.model_list                         = sku.model_list;
            //postData.ds_cat_rcmd_id                   = random.Next(11111111, 91111111).ToString() + random.Next(11111111, 91111111).ToString(); // Chưa biết cái này là cái gì
            postData.parent_sku                         = random.Next(1111111, 9111111).ToString(); // Chưa biết cái này là cái gì
            postData.weight                             = weight.ToString();
            postData.dimension.with                     = Math.Max(5, int.Parse(shopeeShippings.shipping_infos[0].debug.sizes_data[0].width.ToString()));
            postData.dimension.height                   = Math.Max(2, int.Parse(shopeeShippings.shipping_infos[0].debug.sizes_data[0].height.ToString()));
            postData.dimension.length                   = Math.Max(5, int.Parse(shopeeShippings.shipping_infos[0].debug.sizes_data[0].length.ToString()));
            postData.description                        = BeautifulDescription(postData, shopeeProductInfo, taobaoProductInfo);

            // POST lên shopee
            Global.AddLog("Bắt đầu up sản phẩm");
            client = new RestClient("https://banhang.shopee.vn/api/v3/product/create_product/?version=3.1.0&SPC_CDS=GICUNGDUOC&SPC_CDS_VER=2");
            //client.Timeout = -1;
            request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json;charset=UTF-8");
            FakeshopeeCookie(request);

            List<ExpandoObject> postDataFinal = new List<ExpandoObject>();
            postDataFinal.Add(postData);
            postDataString = JsonConvert.SerializeObject(postDataFinal);
            //Global.AddLog(postDataString);
            request.AddParameter("application/json;charset=UTF-8", postDataString, ParameterType.RequestBody);
            response = client.Execute(request);
            // In kết quả trả về
            dynamic results = JsonConvert.DeserializeObject<dynamic>(response.Content);
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
