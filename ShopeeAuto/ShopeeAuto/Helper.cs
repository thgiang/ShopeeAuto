using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
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
            try
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
            catch
            {
                Global.AddLog("ERROR: Không tải đc file " + PictureUrl);
                return "CANNOT_DOWNLOAD_FILE";
            }
            
        }

        public static Bitmap ScreenshotWayBill(ChromeDriver driver)
        {
            int x = 682, y = 75, width = 555, height = 760, appendHeight = 60;

            // Chupj ảnh Chrome
            Screenshot screenshot = driver.TakeScreenshot();
            Image image = ScreenshotToImage(screenshot);

            // Tạo graphic mới
            Bitmap result = new Bitmap(width, height + appendHeight);
            Graphics graphics = Graphics.FromImage(result);

            // Copy ảnh màn hình vào graphics
            graphics.DrawImage(image, new Rectangle(-x, -y, image.Width, image.Height));

            // Vẽ một vùng màu trắng xuống dưới cùng của graphic
            graphics.FillRectangle(Brushes.White, new Rectangle(0, height, width, appendHeight));

            // Vẽ thêm chữ
            var sf = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                //LineAlignment = StringAlignment.Center,
            };
            var font = new Font("Arial", 10, FontStyle.Bold);
            //graphics.DrawRectangle(new Pen(Brushes.Black), new Rectangle(18, height, 518, 55));
            graphics.DrawString("Cảm ơn bạn, chúc bạn một ngày vui vẻ và giúp shop đánh giá 5* nhé!", font, Brushes.Black, new Rectangle(0, height + 5, width, appendHeight), sf);
            List<string> stts = new List<string>();
            stts.Add("Anh có xô hay chậu gì không? Hứng hộ tình cảm của em dành cho anh đi ");
            stts.Add("Anh vô gia cư hay sao cứ ở trong đầu em mãi...");
            stts.Add("Anh có thích Sơn Tùng không? Em không phải Sơn Tùng nhưng em vẫn âm thầm bên anh");
            stts.Add("Chưa quen đừng bảo em kiêu. Quen rồi mới thấy đáng yêu cực kỳ!");
            stts.Add("Nghe nói con gái như em rất là khó gần?\nAnh hỏi cho vui chứ không có cần :'>");
            stts.Add("Không có gì là mãi mãi. Chỉ có từ \"Mãi mãi\" mới là mãi mãi.");
            stts.Add("Có 1 sự thật là… bạn sẽ trẻ mãi… cho tới tận lúc già.");
            stts.Add("Bí quyết để sống lâu là đừng bao giờ ngừng thở.");
            stts.Add("Trứng rán cần mỡ, bắp cần bơ, yêu không cần cớ, cần cậu cơ!");
            stts.Add("Hôm nay anh học toán hình. Tròn, vuông chẳng có; toàn hình bóng em ♥");
            stts.Add("Đôi môi này chỉ ăn cơm với cá. Đã bao giờ biết thơm má ai đâu :*");
            stts.Add("Nghiện ngập còn có thể cai. Yêu em chỉ đầu thai mới hết :\">");
            stts.Add("Hôm qua là monday, hôm nay là tuesday. Vậy hôm nào là bên em đây?");
            stts.Add("Tính em không thích được khen, nhưng em lại thích Nô-en có quà.");
            stts.Add("Người ta vá áo bằng kim, anh cho em hỏi vá tim bằng gì?");
            stts.Add("Vì nàng nói nàng thích màu xanh\nTôi đem lòng tôi yêu cả bầu trời.");
            stts.Add("Trong ngàn vạn cách để hạnh phúc, trực tiếp nhất chính là ngắm nhìn em.");
            stts.Add("Trộm cắp bây giờ nhanh thật, quay đi quay lại mất luôn trái tim.");
            stts.Add("Nhân chi sơ, tính bản thiện.\nThích cậu đến nghiện, thì phải làm sao?");
            stts.Add("Hỏi em đi đứng thế nào. Năm lần, bảy lượt ngã vào tim anh?");
            stts.Add("Em viết hộ anh một phương trình, kết quả chỉ có chúng mình được không?");
            stts.Add("Chẳng cần bánh ngọt với kem. Chỉ cần em nói yêu anh, đủ rồi!");
            stts.Add("Bệnh phổi là do thuốc, bệnh gan là do nhậu\nBệnh tim chắc chắn là do cậu rồi!");
            stts.Add("Em ơi nắng ấm xa rồi. Đông sang, gió lạnh anh cần em thôi!");
            stts.Add("Anh không thích nhạc Only C. Em chỉ thích only em.");
            stts.Add("Nước trong nước chảy quanh chùa.\nAnh xin em đấy bỏ bùa anh đi.");
            stts.Add("Đường khuya thì vắng, nhà anh thì xa.\nNhiều nguy hiểm lắm, ngủ nhà em nha!");
            stts.Add("Đầu tiên hãy nói nhớ anh đi, sau đó hỏi anh đang làm gì?\nCùng vài câu quan tâm sâu sắc. Đơn giản như thế, em làm đi!");
            stts.Add("Anh không muốn làm người xấu, cũng không muốn làm người tốt.\nAnh chỉ muốn làm người yêu em.");
            stts.Add("Muốn mời em một chén trà. Nhưng sợ thành người một nhà với em.");
            stts.Add("Noel anh vẫn một mình. Nếu em cũng thế thì mình yêu thôi.");
            stts.Add("Soái ca là của ngôn tình, còn anh là của một mình em thôi.");
            stts.Add("Ở hiền thì gặp lành. Vậy ở đâu thì gặp anh?");
            stts.Add("Đen Vâu thì muốn trồng rau nuôi cá.\nCòn anh thì đang hỏi má để nuôi thêm em.");
            stts.Add("Xuân kiếm lì xì, Hạ kiếm kem. Thu kiếm hoa sữa, Đông kiếm em.");
            stts.Add("Em như búp bê trên cành, biết ăn biết ngủ, biết kiếm tiền và yêu anh.");
            stts.Add("Anh có thể ship cho em một ly nâu đá. Cùng một vài cái hôn má được không?");
            stts.Add("Nếu em cảm thấy không phiền. Mùa đông đang tới, yêu liền được không?");
            stts.Add("Em sinh ra không phải để vất vả. Mà để sau này được gả cho anh.");
            stts.Add("Trời lạnh ra đường cậu nhớ mang theo tớ nhé!");
            stts.Add("Em thích chiều hoàng hôn buông. Anh thích chiều buồn hôn em.");
            stts.Add("Con cò mà đi ăn đêm, đậu phải cành mềm lộn cổ xuống ao.\nAnh đây không uống ngụm nào, vẫn say ngây ngất ngã vào tình em!");
            stts.Add("Đến giầy dép còn có đôi, cớ sao em lại đơn côi thế này?");
            stts.Add("Em là cô gái mang giày trắng. Ngược đời ngược nắng đi tìm anh.");
            stts.Add("Cuộc sống thì giống cuộc đời, còn em thì giống bạn đời của anh.");
            stts.Add("Ta mua viên thuốc ngừng thương. Người nhầm bán thuốc đơn phương cả đời.");
            stts.Add("Thằng bờm thì thích nắm xôi, còn em thích nắm tay tôi chứ gì?");
            stts.Add("Tim em đã bật đèn xanh. Cớ sao anh mãi đạp phanh thế này?");
            var random = new Random();
            int index = random.Next(stts.Count);
            var font2 = new Font("Arial", 9, FontStyle.Italic);
            graphics.DrawString(stts[index], font2, Brushes.Black, new Rectangle(0, height + 25, width, 40), sf);
            return result;
        }

        // Sinh ra captcha
        public static string GenCaptcha(string text)
        {
            Random random = new Random();
            Bitmap myBitmap = new Bitmap(300, 100);
            Graphics g = Graphics.FromImage(myBitmap);
            char[] chars = text.ToCharArray();
            int i = 0;
            foreach(char c in chars)
            {
                g.DrawString(c.ToString(), new Font("Segoe Script", random.Next(25, 35)), Brushes.Black, new PointF(55 + i * 27, random.Next(10, 20)));
                i++;
            }
           
            string path = Path.GetTempFileName();
            myBitmap.Save(path);
            return path;
        }

        // Chụp ảnh toàn bộ trang web
        public static Image GetEntireScreenshot(ChromeDriver driver)
        {
            // Get the total size of the page
            var totalWidth = (int)(long)((IJavaScriptExecutor)driver).ExecuteScript("return document.body.offsetWidth"); //documentElement.scrollWidth");
            var totalHeight = (int)(long)((IJavaScriptExecutor)driver).ExecuteScript("return  document.body.parentNode.scrollHeight");
            // Get the size of the viewport
            var viewportWidth = (int)(long)((IJavaScriptExecutor)driver).ExecuteScript("return document.body.clientWidth"); //documentElement.scrollWidth");
            var viewportHeight = (int)(long)((IJavaScriptExecutor)driver).ExecuteScript("return window.innerHeight"); //documentElement.scrollWidth");

            // We only care about taking multiple images together if it doesn't already fit
            if (totalWidth <= viewportWidth && totalHeight <= viewportHeight)
            {
                var screenshot = driver.TakeScreenshot();
                return ScreenshotToImage(screenshot);
            }
            // Split the screen in multiple Rectangles
            var rectangles = new List<Rectangle>();
            // Loop until the totalHeight is reached
            for (var y = 0; y < totalHeight; y += viewportHeight)
            {
                var newHeight = viewportHeight;
                // Fix if the height of the element is too big
                if (y + viewportHeight > totalHeight)
                {
                    newHeight = totalHeight - y;
                }
                // Loop until the totalWidth is reached
                for (var x = 0; x < totalWidth; x += viewportWidth)
                {
                    var newWidth = viewportWidth;
                    // Fix if the Width of the Element is too big
                    if (x + viewportWidth > totalWidth)
                    {
                        newWidth = totalWidth - x;
                    }
                    // Create and add the Rectangle
                    var currRect = new Rectangle(x, y, newWidth, newHeight);
                    rectangles.Add(currRect);
                }
            }
            // Build the Image
            var stitchedImage = new Bitmap(totalWidth, totalHeight);
            // Get all Screenshots and stitch them together
            var previous = Rectangle.Empty;
            foreach (var rectangle in rectangles)
            {
                // Calculate the scrolling (if needed)
                if (previous != Rectangle.Empty)
                {
                    var xDiff = rectangle.Right - previous.Right;
                    var yDiff = rectangle.Bottom - previous.Bottom;
                    // Scroll
                    ((IJavaScriptExecutor)driver).ExecuteScript(String.Format("window.scrollBy({0}, {1})", xDiff, yDiff));
                }
                // Take Screenshot
                var screenshot = driver.TakeScreenshot();
                // Build an Image out of the Screenshot
                var screenshotImage = ScreenshotToImage(screenshot);
                // Calculate the source Rectangle
                var sourceRectangle = new Rectangle(viewportWidth - rectangle.Width, viewportHeight - rectangle.Height, rectangle.Width, rectangle.Height);
                // Copy the Image
                using (var graphics = Graphics.FromImage(stitchedImage))
                {
                    graphics.DrawImage(screenshotImage, rectangle, sourceRectangle, GraphicsUnit.Pixel);
                }
                // Set the Previous Rectangle
                previous = rectangle;
            }
            return stitchedImage;
        }

        private static Image ScreenshotToImage(Screenshot screenshot)
        {
            Image screenshotImage;
            using (var memStream = new MemoryStream(screenshot.AsByteArray))
            {
                screenshotImage = Image.FromStream(memStream);
            }
            return screenshotImage;
        }

        public static string LowerFisrtLetter(string s)
        {
            if (s != string.Empty && char.IsUpper(s[0]))
            {
                s = char.ToLower(s[0]) + s.Substring(1);
            }
            return s;
        }
    }
}
