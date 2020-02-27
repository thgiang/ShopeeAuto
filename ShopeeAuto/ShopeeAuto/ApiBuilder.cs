using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShopeeAuto
{
    public class ApiBuilder
    {
        public string apiUrl;
        public string accessToken;

        public ApiBuilder()
        {
            apiUrl = ConfigurationSettings.AppSettings["api_url"];
            accessToken = ConfigurationSettings.AppSettings["access_token"];
        }

        public dynamic Request(IDictionary<string, string> parameters)
        {
            // String request path
            if(String.IsNullOrEmpty(parameters["route"]))
            {
                parameters["route"] = "";
            }

            // Init request
            var client = new RestClient(apiUrl + parameters["route"]);
            var request = new RestRequest(Method.GET);
            request.AddHeader("content-type", "application/json; charset=UTF-8");

            // Add parameters to request
            foreach (var key in parameters.Keys)
            {
                if (!string.IsNullOrEmpty(parameters[key]))
                {
                    request.AddParameter(key, parameters[key]);
                }
            }
            request.AddParameter("access_token", accessToken);

            // Send request now
            IRestResponse response = client.Execute(request);
            dynamic results = new ExpandoObject();
            results.status = "unknown";
            results.message = "unknown";
            try
            {
                results = JsonConvert.DeserializeObject<dynamic>(response.Content);
            } catch
            {
                return results;
            }
            
            // Return result
            return results;
        }
    }
}
