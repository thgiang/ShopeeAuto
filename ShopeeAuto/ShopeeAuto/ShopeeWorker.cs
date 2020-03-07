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
                username = client.Data.ShopeeUsername.ToString();
                password = client.Data.ShopeeUsername.ToString();
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

            apiResult = Global.api.RequestOthers("https://banhang.shopee.vn/api/v3/general/upload_image/?SPC_CDS=8c777714-50b7-4017-82bd-3a5141424b85&SPC_CDS_VER=2", Method.POST, shopeeCookie, parameters);
            if (!apiResult.success)
            {
                return "";
            }
            dynamic results = JsonConvert.DeserializeObject<dynamic>(apiResult.content);
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
            // Không thể bán lỗ được vì vậy revenuePercent phải lớn hơn hoặc bằng 1
            revenuePercent = Math.Max(revenuePercent, 1);

            ApiResult apiResult;

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
                    //float SKUPrice = SKUData.price * revenuePercent; // Dòng này sai, phải gọi lên API tính giá vc (giá gốc) xong rồi mới nhân tỉ lệ để ra giá rao trên shopee
                    float SKUPrice = SKUData.price;

                    Dictionary<string, string> parameters = new Dictionary<string, string>
                    {
                        ["route"] = "shipping-fee-ns",
                        ["amount"] = SKUPrice.ToString(),
                        ["weight"] = weight.ToString()
                    };
                    // Gọi đến chết bao giờ tính đc giá thì thôi, cái này ko thể ko dừng đc
                    bool calcSuccess = false;
                    do
                    {
                        try
                        {
                            apiResult = Global.api.RequestMyApi(parameters);
                            if(apiResult.success)
                            {
                                // Đề phòng trường hợp final_price tính sai hoặc trả về 0
                                // 3600 là tỉ giá, bán kiểu gì cũng phải cao hơn giá này.
                                CurrenSKUData.price = Math.Min((int)(SKUPrice * 3600), (int)JsonConvert.DeserializeObject<dynamic>(apiResult.content).final_price);
                                calcSuccess = true;
                            } else
                            {
                                Global.AddLog("Tính giá chưa đc, tao gọi lại đến chết, bao giờ tính đc thì thôi");
                                Thread.Sleep(1000);
                            }
                           
                        }
                        catch(Exception e)
                        {
                            Global.AddLog("Tính giá chưa đc lại còn bị lỗi "+e.Message.ToString()+", tao gọi lại đến chết, bao giờ tính đc thì thôi");
                            Thread.Sleep(1000);
                        }
                    } while (!calcSuccess);
                    Global.AddLog("Giá bán ra trước khi nhân tỉ lệ: " + CurrenSKUData.price + ". Tỉ lệ nhân "+ revenuePercent.ToString());
                    CurrenSKUData.price = ((int)CurrenSKUData.price * revenuePercent / 1000 * 1000).ToString();
                    

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
                    Global.AddLog("Giá bán ra cho SKU " + SKUPropName+ " là: "  + CurrenSKUData.price.ToString());

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
            ApiResult apiResult;
            Global.driver.Navigate().GoToUrl("https://banhang.shopee.vn/portal/product/category");
            Random random = new Random();
            Global.AddLog("Bắt đầu upload sản phẩm " + taobaoId + " từ Taobao lên Shopee");
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
            // Tính giá TB các SKU Taobao
            Global.AddLog("Tính giá trung bình mỗi SKU Taobao");
            float taobaoPrice = float.Parse(taobaoProductInfo.price.ToString());
            if (taobaoProductInfo.skus.sku.Count > 0)
            {
                taobaoPrice = 0;
                foreach (dynamic s in taobaoProductInfo.skus.sku)
                {
                    Global.AddLog(s.price.ToString());
                    taobaoPrice += float.Parse(s.price.ToString());                    
                }
                taobaoPrice /= taobaoProductInfo.skus.sku.Count;
            }

            // Gọi lên API để tính cước vận chuyển để tính ra giá cuối cùng

            // Lấy thông tin cân nặng của đối thủ ở shopee
            Global.AddLog("Lấy kích thước sản phẩm");
            apiResult = Global.api.RequestOthers("https://shopee.vn/api/v0/shop/" + shopeeCategoryId + "/item/" + shopeeId + "/shipping_info_to_address/?city=Huy", Method.GET);
            if (!apiResult.success)
            {
                Global.AddLog("Lỗi khi lấy thông tin kích thước sản phẩm");
                return "error";
            }
            dynamic shopeeShippings = JsonConvert.DeserializeObject<dynamic>(apiResult.content);
            if (shopeeShippings == null)
            {
                Global.AddLog("Lỗi khi lấy thông tin kích thước sản phẩm");
                return "error";
            }
            int weight = (shopeeShippings.shipping_infos[0].debug.total_weight * 100);


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
            p = shopeeProductInfo.price_max.ToString();
            int shopeePrice = int.Parse(p.Substring(0, p.Length - 5));
            if (shopeeProductInfo.models.Count > 0)
            {
                shopeePrice = 0;
                foreach (dynamic m in shopeeProductInfo.models)
                {
                    p = m.price.ToString();
                    shopeePrice += int.Parse(p.Substring(0, p.Length - 5));
                }
                shopeePrice /= shopeeProductInfo.models.Count;
            }
            Global.AddLog("Giá shopee cuối cùng là "+ shopeePrice.ToString());
            if (shopeePrice == 0)
            {
                Global.AddLog("STOP: ShopeePrice = 0");
                return "error";
            }

            // Giá bán ra
            float outPrice = Math.Max(taobaoPrice * (100 + minRevenue) / 100, (shopeePrice  * (100 - random.Next(1, 3)) / 100)); // Giá tối thiểu cần có lãi, random rẻ hơn đối thủ 1 đến 3%
            Global.AddLog("Quyết định bán ra giá chung chung là: " + outPrice.ToString());

            float revenuePercent;
            revenuePercent = outPrice / taobaoPrice;

            
            dynamic sku = BuildShopeeSKUBasedOnTaobao(taobaoProductInfo, allImages, revenuePercent, weight);
            List<int> categoryPath = new List<int>();
            for (int i = 0; i < 3; i++)
            {
                categoryPath.Add(int.Parse(shopeeProductInfo.categories[i].catid.ToString()));
            }

            // Model Id
            int modelId = GetAttributeModelId(shopeeProductInfo.categories[2].catid.ToString()); ;
            if(modelId == 0)
            {
                Global.AddLog("ERROR:Lỗi khi lấy modelId");
                return "error";
            }
            // Đẩy data thật vào object
            postData.name                               = Global.FirstLetterToUpper(shopeeProductInfo.name.ToString());
            postData.images                             = allImages.generalImgs;
            postData.category_path                      = categoryPath;
            postData.attribute_model.attribute_model_id = modelId;
            postData.attribute_model.attributes         = new List<string>();
            postData.price                              = ((int)outPrice / 1000 * 1000).ToString();
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
            RestClient client = new RestClient("https://banhang.shopee.vn/api/v3/product/create_product/?version=3.1.0&SPC_CDS=GICUNGDUOC&SPC_CDS_VER=2");
            //client.Timeout = -1;
            RestRequest request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "application/json;charset=UTF-8");
            FakeshopeeCookie(request);

            List<ExpandoObject> postDataFinal = new List<ExpandoObject>
            {
                postData
            };
            postDataString = JsonConvert.SerializeObject(postDataFinal);
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
