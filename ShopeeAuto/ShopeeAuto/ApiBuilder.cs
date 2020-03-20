using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShopeeAuto
{
    // Class data trả về
    public class ApiResult
    {
        public bool success = false;
        public string content = "";
    }

    // Class gọi API
    public class ApiBuilder
    {
        public string apiUrl;
        public string accessToken;
        public string laoNetApi;

        public ApiBuilder()
        {
            apiUrl = System.Configuration.ConfigurationManager.AppSettings["api_url"];
            accessToken = System.Configuration.ConfigurationManager.AppSettings["access_token"];
            laoNetApi = System.Configuration.ConfigurationManager.AppSettings["laonet_api"];
        }

 
        public string UploadImageToMyServer(string filePath)
        {
            var client = new RestClient(apiUrl+"upload_image");
            var request = new RestRequest(Method.POST);
            request.AddHeader("content-type", "multipart/form-data");
            request.AddParameter("access_token", accessToken);
            request.AddFile("image", File.ReadAllBytes(filePath), Path.GetFileName(filePath));
            IRestResponse response;
            int requestTime = 1;
            do
            {
                try
                {
                    // Send request now
                    response = client.Execute(request);
                    dynamic results = JsonConvert.DeserializeObject<dynamic>(response.Content);
                    if(results.url != null && results.url != "")
                    {
                        Global.AddLog("Upload ảnh " + filePath + " thành công");
                        return results.url;
                    }  
                }
                catch
                {
                    Global.AddLog("Up ảnh bị lỗi lần " + (requestTime - 1).ToString());
                    requestTime++;
                    Thread.Sleep(1000);
                }
            } while (requestTime < 3);

            // Return result
            return "";
        }

        public ApiResult RequestMyApi(IDictionary<string, string> parameters, Method method = Method.GET)
        {
            ApiResult result = new ApiResult();

            // String request path
            if (String.IsNullOrEmpty(parameters["route"]))
            {
                parameters["route"] = "";
            }

            // Init request
            string url = apiUrl + parameters["route"];
            var client = new RestClient(url);
            var request = new RestRequest(method);
            if(method == Method.PUT)
            {
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
            }

            // Add parameters to request
            foreach (var key in parameters.Keys)
            {
                if (!string.IsNullOrEmpty(parameters[key]) && key != "route")
                {
                    request.AddParameter(key, parameters[key]);
                }
            }
            request.AddParameter("access_token", accessToken);

          
            dynamic results = new ExpandoObject();
            results.status = "unknown";
            results.message = "unknown";

            IRestResponse response;
            // RequestMyApi 3 lần mà vẫn lỗi thì thôi
            int requestTime = 1;
            do
            {
                try
                {
                    // Send request now
                    response = client.Execute(request);
                    result.success = true;
                    result.content = response.Content;
                   // results = JsonConvert.DeserializeObject<dynamic>(response.Content);
                    requestTime = 3;
                    Global.AddLog("RequestMyApi tới URL "+url+" thành công");
                }
                catch
                {
                    Global.AddLog("RequestMyApi tới URL " + url + " bị lỗi lần " + (requestTime - 1).ToString());
                    requestTime++;
                    Thread.Sleep(1000);
                }
            } while (requestTime < 3);

            // Return result
            return result;
        }
       
        public ApiResult RequestOthers(string url, RestSharp.Method method, dynamic cookies = null, Dictionary<string, dynamic> parameters = null)
        {
            ApiResult result = new ApiResult();

            var client = new RestClient(url);
            var request = new RestRequest(method);

            request.AddHeader("Authorization", Global.authToken);
            request.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36");
            // Fake cookie nếu có
            if(cookies != null)
            {
                foreach (OpenQA.Selenium.Cookie cookie in cookies)
                {
                    request.AddCookie(cookie.Name, cookie.Value.TrimEnd('"').TrimStart('"'));
                    request.AddCookie("SPC_CDS", "GICUNGDUOC");
                }

            }
            // Add Parameters nếu có
            if(parameters != null)
            {
                foreach (KeyValuePair<string, dynamic> entry in parameters)
                {
                    if(entry.Key == "file")
                    {
                        request.AddFile("file", File.ReadAllBytes(entry.Value), Path.GetFileName(entry.Value));
                    } else
                    {
                        if(entry.Key == "json_body")
                        {
                            request.AddParameter("application/json;charset=UTF-8", entry.Value, ParameterType.RequestBody);
                        } else
                        {
                            request.AddParameter(entry.Key, entry.Value);
                        }
                        
                    }
                }
            }

            IRestResponse response;
            int requestTime = 1; // Đếm số lần thử request
            do
            {
                try
                {
                    response = client.Execute(request);
                    result.success = true;
                    result.content = response.Content;
                    requestTime = 3;
                }
                catch
                {
                    Global.AddLog("RequestMyApi tới URL " + url + " bị lỗi lần " + (requestTime - 1).ToString());
                    requestTime++;
                    Thread.Sleep(1000);
                }
            } while (requestTime < 3);

            return result;
        }
    }
}
