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
                password = client.Data.ShopeePassword.ToString();
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
                loginForm.FindElements(By.TagName("input"))[0].SendKeys(username); // Username
                loginForm.FindElements(By.TagName("input"))[1].SendKeys(password + "\n"); // Password

                //loginForm.FindElement(By.ClassName("shopee-checkbox__indicator")).Click(); // Remember me
                //loginForm.FindElement(By.ClassName("shopee-button--primary")).Click(); // Login now

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
                if (!PrepareTaobaoData.uploadedImages.ContainsKey(item_img)) {
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

                        Dictionary<string, string> parameters = new Dictionary<string, string>
                        {
                            ["route"] = "shipping-fee-ns",
                            ["amount"] = modelPrice.ToString(),
                            ["weight"] = weight.ToString()
                        };
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
                        Global.AddLog("Giá bán ra trước khi nhân tỉ lệ: " + model.Price.ToString() + ". Tỉ lệ nhân " + revenuePercent.ToString());
                        model.Price = ((int)(int.Parse(model.Price) * revenuePercent / 1000) * 1000).ToString();
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
                            skuName += Global.Translate(PrepareTaobaoData.skuNames[skuProp]);

                         
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

                        Global.AddLog("Giá bán ra cho SKU " + skuName + " là: " + model.Price.ToString());
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
        public string BeautifulDescription(NSShopeeCreateProduct.CreateProduct postData, NSShopeeProduct.ShopeeProduct shopeeProductInfo, NSTaobaoProduct.TaobaoProduct taobaoProductInfo)
        {
            // Độ dài tối đa 3000 kí tự
            //" + shopeeProductInfo.Item.Description.Substring(0, Math.Min(2000, shopeeProductInfo.Item.Description.Length)).Replace(",", ", ").Replace(".", ". ").Replace("  ", " ").Replace(" ,", ", ").Replace(".", ". ") + @"
            string fullPropsString = "";
            List<string> ignoreKeys = new List<string> {"", "甜美", "货号", "货号", "套头", "开口深度", "皮质特征", "通勤", "流行元素/工艺", "安全等级", "销售渠道类型"};
            List<string> ignoreValues = new List<string> {"", "甜美", "货号", "货号", "套头", "开口深度", "皮质特征", "其他", "其他/other", "other", "短裙" };
            List<string> notImportantWords = new List<string> { "建议" };
            // 甜美 ngọt ngào là cái qq gì @@ 
            // 安全等级 cấp độ bảo mật, vô nghĩa quá
            // 品牌 thương hiệu
            // 货号 mã số bài viết
            // 套头 ko dịch nổi, tay áo, bảo hiểm rủi do, chả liên quan gì nhau
            // 开口深度 Độ nông sâu của giày nhưng nói chung tối nghĩa
            // 短裙 Kiểu váy: 短裙 (nghĩa là váy), ý là váy bt, nhưng mà bt thì thôi ko cần ghi
            // 流行元素/工艺: Cách may ra cái váy, giá trị thường là: May, có cúc... nhưng mà sida quá loại

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

                                if (ignoreKeys.Contains(k.ToLower()))
                                {
                                    continue;
                                }

                                if (ignoreValues.Contains(v.ToLower()))
                                {
                                    continue;
                                }

                                fullPropsString += Global.Translate(k);
                                fullPropsString += ": ";                                
                                fullPropsString += Global.Translate(v) + @"
";

                            }

                        }
                    }
                   
                }
            }
            
            string desciption = postData.Name.ToString().ToUpper() + @"
------------------------------------------------------
★★★ THÔNG TIN SẢN PHẨM
"+ fullPropsString + @"

★★★ CAM KẾT VÀ DỊCH VỤ
- Sản phẩm đảm bảo chất lượng, chính xác 100% về thông số, mô tả và hình ảnh.
- Sản phẩm được nhập khẩu trực tiếp từ Trung Quốc.
- Thời gian giao hàng dự kiến: Trong vòng 12 ngày kể từ ngày đặt hàng (bao gồm 8 ngày từ TQ về VN và 3 ngày từ Hà Nội tới địa chỉ bất kì trên toàn quốc). Thông tin tracking được gửi tới Quý khách qua tin nhắn Shopee hàng ngày.
- Hình thức thanh toán: COD toàn quốc.
- Khách hàng đặt mua số lượng lớn vui lòng liên hệ trực tiếp để được giảm giá tới 20%.
";
            desciption = Global.AntiDangStyle(desciption);
            return desciption;
        }

        public string CopyTaobaoToShopee(NSTaobaoProduct.TaobaoProduct taobaoProductInfo, NSShopeeProduct.ShopeeProduct shopeeProductInfo, NSApiProduct.ProductList jobData)
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
                weight = shopeeShippings.shipping_infos[0].debug.total_weight * 1000;
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
            float outPrice = Math.Max(taobaoPrice * (100 + minRevenue) / 100, (shopeePrice  * (100 - random.Next(1, 3)) / 100)); // Giá tối thiểu cần có lãi, random rẻ hơn đối thủ 1 đến 3%
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
            string name = Global.AntiDangStyle(shopeeProductInfo.Item.Name.ToString()).Replace("sẵn", "order").Replace("Sẵn", "Order").Replace("SẴN", "ORDER");
            postData.Name = name.Substring(0, Math.Min(name.Length, 120));
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
            postData.Description                        = BeautifulDescription(postData, shopeeProductInfo, taobaoProductInfo);

            // POST lên shopee
            Global.AddLog("Bắt đầu up sản phẩm");
            RestClient client = new RestClient("https://banhang.shopee.vn/api/v3/product/create_product/?version=3.1.0&SPC_CDS=GICUNGDUOC&SPC_CDS_VER=2");
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
                                    { "shopee_item_id",  SuccessProductID},
                                    { "shopee_shop_id",  Global.myShopId},
                                    { "shopee_price",  postData.Price},
                                    { "shopee_model_list",  JsonConvert.SerializeObject(sku.model_list)},
                                    { "action", "list_done" }
                                };
                    Global.api.RequestMyApi(parameters, Method.PUT);
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


        //============================ XỬ LÝ ORDER =========================================== 
        public string ProcessNewCheckout()
        {
            /*
            string lastCheckoutId = "";
            ApiResult apiResult;
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                ["route"] = "last-checkout",
            };

            apiResult = Global.api.RequestMyApi(parameters);
            if (apiResult.success)
            {
                dynamic lastCheckoutObject = JsonConvert.DeserializeObject<dynamic>(apiResult.content);
                if (lastCheckoutObject.status == "success")
                {
                    lastCheckoutId = lastCheckoutObject.checkout_id;
                }
            }
            else
            {
                Global.AddLog("ERROR: Không lấy được checkout mới nhất");
                return "error";
            }

            // Nếu lastCheckoutId = "" thì phải xử lý toàn bộ đơn của shop này
            List<NSShopeeOrders.Order> orders = new List<NSShopeeOrders.Order>();
            NSShopeeOrders.ShopeeOrders orderPage;
            int page = -1;
            bool shouldBreak = false;
            do
            {
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
                            if (od.CheckoutId != lastCheckoutId)
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

    */
            // Lấy địa chỉ các bưu cục
            //https://banhang.shopee.vn/api/v3/logistics/get_channel_branches/?channel_id=50018 kèm cookie, J&T

            // POST địa chỉ và phương thức gửi hàng
            //https://banhang.shopee.vn/api/v3/shipment/init_order/?SPC_CDS=ff48cffa-68c7-41fb-b279-fbc1fb5a7c26&SPC_CDS_VER=2 
            // {channel_id: 50018, order_id: 37733260616710, forder_id: "37733260616710"}

            // Sau đó liên tục GET tới khi nào lấy đc mã vận đơn thì thôi
            //https://banhang.shopee.vn/api/v3/shipment/batch_get_forder_shipment_status/?SPC_CDS=ff48cffa-68c7-41fb-b279-fbc1fb5a7c26&SPC_CDS_VER=2&order_list=[%7B%22order_id%22:37733260616710,%22forder_id%22:%2237733260616710%22,%22channel_id%22:50018%7D]
            //order_list: [{"order_id":37733260616710,"forder_id":"37733260616710","channel_id":50018}]
            // MÃ vận đơn nằm ở data.orders_status.third_party_tn, can_print_waybill = true

            string orderId = "37733260616710";

            // Chụp ảnh hóa đơn
            Global.driver.Navigate().GoToUrl("https://banhang.shopee.vn/api/v3/logistics/get_waybill_new/?order_ids="+orderId);
            try
            {
                Global.wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("body")));
                Thread.Sleep(500);
               
                Bitmap bmpImage = Helper.ScreenshotWayBill();
                bmpImage.Save(@"C:\Users\Admin\Desktop\ma_van_don\ketqua_" + orderId + ".jpg");  
            } catch
            {

            }
                
               
            // Lưu mã vận đơn
            return "success";
        }
    }
}
