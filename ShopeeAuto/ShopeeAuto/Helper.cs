using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ShopeeAuto
{
    class Helper
    {
        public string DownloadImage(string PictureUrl)
        {
            string randomPath = Path.GetTempFileName();
            if (PictureUrl.Contains("http") == false)
            {
                PictureUrl = "https:" + PictureUrl;
            }
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(PictureUrl, randomPath);
                return randomPath;
            }
        }
    }
}
