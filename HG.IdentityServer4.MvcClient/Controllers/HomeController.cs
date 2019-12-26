using HG.IdentityServer4.MvcClient.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace HG.IdentityServer4.MvcClient.Controllers
{
    public class HomeController : Controller
    {
        private static string accessTokenValue = string.Empty;
        private static HttpClient httpClient = new HttpClient();

        public HomeController()
        {

        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Access()
        {
            ViewData["Message"] = "授权信息";

            accessTokenValue = HttpContext.GetTokenAsync("access_token").Result;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessTokenValue);

            //var redirect_uri = Request.Query["callBackUrl"];
            //if (!string.IsNullOrEmpty(redirect_uri))
            //{
            //    return Redirect(redirect_uri);
            //}
            //else
            //{
            return View();
            //}
        }


        //{
        //  "header":{
        //      "namespace":"AliGenie.Iot.Device.Discovery",
        //      "name":"DiscoveryDevices",
        //      "messageId":"1bd5d003-31b9-476f-ad03-71d471922820",
        //      "payLoadVersion":1
        //   },
        //   "payload":{
        //       "accessToken":"access token"
        //    ｝
        //  }

        //// 密码模式
        //var parameters = new Dictionary<string, string>();
        //parameters.Add("grant_type", "password");
        //    parameters.Add("UserName", clientId);
        //    parameters.Add("Password", clientSecret);

        //    // 提交
        //    var rst = client.PostAsync(url + "token", new FormUrlEncodedContent(parameters)).Result.Content.ReadAsStringAsync().Result;
        //var entity = GetModelForPostData<Token>(rst);
        //Console.WriteLine(entity.AccessToken);

        public class header
        {
            [JsonProperty("namespace")]
            public string namespaceX { get; set; }

            public string name { get; set; }
            public string messageId { get; set; }
            public string payLoadVersion { get; set; }
        }

        public class payload
        {
            public string accessToken { get; set; }
        }

        public class arg
        {
            public header header { get; set; }
            public payload payload { get; set; }
        }


        public static string Serialize<T>(T obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj);
            }
            catch
            {
                return string.Empty;
            }
        }


        public IActionResult Contact()
        {
            if (string.IsNullOrEmpty(accessTokenValue))
            {
                ViewData["Message"] = "未找到有效令牌,请先获取令牌";
            }
            else
            {
                //ViewData["Message"] = httpClient.GetAsync("http://198.198.198.176:5010/api/values").Result.Content.ReadAsStringAsync().Result;

                var tmp = new arg()
                {
                    header = new header()
                    {
                        namespaceX = "AliGenie.Iot.Device.Discovery",
                        name = "DiscoveryDevices",
                        messageId = "1bd5d003-31b9-476f-ad03-71d471922820",
                        payLoadVersion = "1"
                    },
                    payload = new payload()
                    {
                        accessToken = accessTokenValue
                    }
                };

                //ViewData["Message"] = HttpClientX.PostJson("http://198.198.198.253:8080/Smart/server/dosmart", Serialize(tmp));
                ViewData["Message"] = HttpClientX.PostJson("https://www.tview.cn/Smart/server/dosmart", Serialize(tmp));

            }

            return View();
        }

        public IActionResult UserInfo()
        {
            if (string.IsNullOrEmpty(accessTokenValue))
            {
                ViewData["Message"] = "未找到有效令牌,请先获取令牌";
            }
            else
            {
                ViewData["Message"] = httpClient.GetAsync("http://198.198.198.176:5000/connect/userinfo").Result.Content.ReadAsStringAsync().Result;
            }

            return View();
        }


        public IActionResult GetToken()
        {
            if (string.IsNullOrEmpty(accessTokenValue))
            {
                return Ok("未找到有效令牌,请先获取令牌");
            }
            else
            {
                return Ok(accessTokenValue);
            }
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync("oidc");
        }
    }
}
