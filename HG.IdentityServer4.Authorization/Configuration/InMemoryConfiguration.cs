using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using Newtonsoft.Json;

namespace HG.IdentityServer4.Authorization.Configuration
{
    public class InMemoryConfiguration
    {
        public static IConfiguration Configuration { get; set; }
        /// <summary>
        /// Define which APIs will use this IdentityServer
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new[]
            {
                new ApiResource("clientservice", "CAS Client Service"),
                new ApiResource("productservice", "CAS Product Service"),
                new ApiResource("agentservice", "CAS Agent Service"),
                new ApiResource("api1", "用户信息校验"),
            };
        }

        /// <summary>
        /// Define which Apps will use thie IdentityServer
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                //new Client
                //{
                //    ClientId = "client.api.service",
                //    ClientSecrets = new [] { new Secret("clientsecret".Sha256()) },
                //    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                //    AllowedScopes = new [] { "clientservice" }
                //},
                //new Client
                //{
                //    ClientId = "product.api.service",
                //    ClientSecrets = new [] { new Secret("productsecret".Sha256()) },
                //    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                //    AllowedScopes = new [] { "clientservice", "productservice" }
                //},
                //new Client
                //{
                //    ClientId = "agent.api.service",
                //    ClientSecrets = new [] { new Secret("agentsecret".Sha256()) },
                //    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                //    AllowedScopes = new [] { "agentservice", "clientservice", "productservice" }
                //},
                //new Client
                //{
                //    ClientId = "cas.mvc.client.implicit",
                //    ClientName = "CAS MVC Web App Client",
                //    AllowedGrantTypes = GrantTypes.Implicit,
                //    RedirectUris = { $"http://{Configuration["Clients:MvcClient:IP"]}:{Configuration["Clients:MvcClient:Port"]}/signin-oidc" },
                //    PostLogoutRedirectUris = { $"http://{Configuration["Clients:MvcClient:IP"]}:{Configuration["Clients:MvcClient:Port"]}/signout-callback-oidc" },
                //    AllowedScopes = new [] {
                //        IdentityServerConstants.StandardScopes.OpenId,
                //        IdentityServerConstants.StandardScopes.Profile,
                //        "agentservice", "clientservice", "productservice","userinfo"
                //    },
                //    //AccessTokenLifetime = 3600, // one hour
                //    AllowAccessTokensViaBrowser = true // can return access_token to this client
                //},
                new Client
                {
                    ClientId = "code.015cd5b0-5b1c-406d-8b8e-fca63ad6d7e5",
                    ClientSecrets = {new Secret("secret.ce9028f2-b3d0-4e9f-9043-ec40f945332a".Sha256())},
                    //ClientId = "code123456789",
                    //ClientSecrets = {new Secret("secret123456789".Sha256())},
                    ClientName = "天涯明月App授权许可证",
                    AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,

                    // 2天过期
                    AccessTokenLifetime = 3600 * 24 *2,

                    //// where to redirect to after login
                    //RedirectUris = { $"http://{Configuration["Clients:MvcClient:IP"]}:{Configuration["Clients:MvcClient:Port"]}/signin-oidc" },
                    //// where to redirect to after logout
                    //FrontChannelLogoutUri = $"http://{Configuration["Clients:MvcClient:IP"]}:{Configuration["Clients:MvcClient:Port"]}/signout-oidc",
                    //PostLogoutRedirectUris = { $"http://{Configuration["Clients:MvcClient:IP"]}:{Configuration["Clients:MvcClient:Port"]}/signout-callback-oidc" },

                     //RedirectUris = {"https://open.bot.tmall.com/oauth/callback" },
                    RedirectUris = { $"{Configuration["Clients:MvcClient:Address"]}" },

                    AbsoluteRefreshTokenLifetime = 2592000,//RefreshToken的最长生命周期,默认30天
                    RefreshTokenExpiration = TokenExpiration.Sliding,//刷新令牌时，将刷新RefreshToken的生命周期。RefreshToken的总生命周期不会超过AbsoluteRefreshTokenLifetime。
                    SlidingRefreshTokenLifetime = 3600,//以秒为单位滑动刷新令牌的生命周期。
                    
                    // 按照现有的设置，如果3600内没有使用RefreshToken，那么RefreshToken将失效。即便是在3600内一直有使用RefreshToken，RefreshToken的总生命周期不会超过30天。所有的时间都可以按实际需求调整。
                    AllowOfflineAccess = true,//如果要获取refresh_tokens ,必须把AllowOfflineAccess设置为true

                    AllowedScopes = new List<string>
                    {   "api1",
                        IdentityServerConstants.StandardScopes.OfflineAccess, //如果要获取refresh_tokens ,必须在scopes中加上OfflineAccess
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                }
            };
        }

        /// <summary>
        /// Define which uses will use this IdentityServer
        /// http://198.198.198.253:8080/Smart/manage/usersList
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<TestUser> GetUsers()
        {
            var jsonStr = HttpClient.Get("https://www.tview.cn/Smart/manage/usersList");

            var res = Deserialize<Res>(jsonStr);

            return res.result.Select(w => new TestUser
            {
                SubjectId = w.userid.ToString(),
                Username = w.loginname,
                Password = w.password
            });

            //return new[]
            //{
            //    new TestUser
            //    {
            //        SubjectId = "10001",
            //        Username = "pyj",
            //        Password = "123456"
            //    }
            //};

            //return new[]
            //{
            //    new TestUser
            //    {
            //        SubjectId = "10001",
            //        Username = "edison@hotmail.com",
            //        Password = "edisonpassword"
            //    },
            //    new TestUser
            //    {
            //        SubjectId = "10002",
            //        Username = "andy@hotmail.com",
            //        Password = "andypassword"
            //    },
            //    new TestUser
            //    {
            //        SubjectId = "10003",
            //        Username = "leo@hotmail.com",
            //        Password = "leopassword"
            //    },
            //    new TestUser
            //    {
            //        SubjectId = "10004",
            //        Username = "pyj",
            //        Password = "123456"
            //    }
            //};
        }

        /// <summary>
        /// Define which IdentityResources will use this IdentityServer
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            //var customProfile = new IdentityResource(
            //        name: "userinfo",
            //        displayName: "用户名称", // optional
            //        claimTypes: new[] { "用户名称" });//包含的用户claims

            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                //new IdentityResources.Email(),
                new IdentityResources.Profile(),
                new IdentityResources.Phone(),
                ////new IdentityResources.Address()
                //new IdentityResource(
                //    name: "userinfo",
                //    displayName: "用户名称", // optional
                //    claimTypes: new[] { "用户名称" })
            };
        }

        private static T Deserialize<T>(string json)
        {
            try
            {
                return (T)JsonConvert.DeserializeObject(json, typeof(T));
            }
            catch
            {
                return default(T);
            }
        }

    }

    public class HttpClient
    {
        public static string Get(string url)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "Get";
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            // 获取内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
        public static string Post(string url)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();
            //获取内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
        public static string Post(string url, Dictionary<string, string> dic)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            //req.ContentType = "application/x-www-form-urlencoded";
            req.ContentType = "application/json";

            #region 添加Post 参数
            StringBuilder builder = new StringBuilder();

            //int i = 0;
            //foreach (var item in dic)
            //{
            //    if (i > 0)
            //        builder.Append("&");
            //    builder.AppendFormat("{0}={1}", item.Key, item.Value);
            //    i++;
            //}

            foreach (var item in dic)
            {
                builder.AppendFormat("{0}", item.Value);
            }

            byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
            req.ContentLength = data.Length;

            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion

            var res = req.GetResponse();

            HttpWebResponse resp = (HttpWebResponse)res;
            Stream stream = resp.GetResponseStream();
            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }

        public static string PostJson(string url, string jsonContent)
        {
            var data = Encoding.UTF8.GetBytes(jsonContent);
            var req = (HttpWebRequest)WebRequest.Create(url);

            req.Method = "POST";
            req.ContentType = "application/json";
            req.ContentLength = data.Length;
            //这个在Post的时候，一定要加上，如果服务器返回错误，他还会继续再去请求，不会使用之前的错误数据，做返回数据
            req.ServicePoint.Expect100Continue = false;

            using (var reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }

            using (var resp = (HttpWebResponse)req.GetResponse())
            {
                var stream = resp.GetResponseStream();
                var reader = new StreamReader(stream, Encoding.UTF8);
                return reader.ReadToEnd();
            }
        }
    }

    //    {
    //	"error_code": 200,
    //	"result": [{
    //		"createtime": "2018-02-01 14:17:40.0",
    //		"device_count": 0,
    //		"email": "atmzxl@163.com",
    //		"email_status": 0,
    //		"homeadress": "深圳",
    //		"homename": "我的家",
    //		"homepicture": "1",
    //		"index_style": 0,
    //		"lastlogintime": "2019-12-11 16:11:49.0",
    //		"loginname": "test01",
    //		"logintime": "2019-12-11 16:22:15.0",
    //		"message_mask": 3,
    //		"password": "e10adc3949ba59abbe56e057f20f883e",
    //		"realname": "1232351",
    //		"sex": 1,
    //		"sh_area_id": 39,
    //		"sh_area_name": "",
    //		"state": 1,
    //		"telephone": "18684561390",
    //		"unreadedMessage": 0,
    //		"userid": 52,
    //		"users_has_roles": [],
    //		"users_img": "upload/img/7b91406bba097929b1d312e9f88f1ad5.jpeg",
    //		"weather": null
    //	}, {
    //		"createtime": "2019-07-07 15:38:23.0",
    //		"device_count": 0,
    //		"email": "",
    //		"email_status": 0,
    //		"homeadress": "",
    //		"homename": "13701140565的家",
    //		"homepicture": "1",
    //		"index_style": 0,
    //		"lastlogintime": "2019-07-07 15:38:23.0",
    //		"loginname": "13701140565",
    //		"logintime": "2019-07-07 15:38:48.0",
    //		"message_mask": 15,
    //		"password": "9ab5a27cbe642b3472adc4162e37390f",
    //		"realname": "13701140565",
    //		"sex": 0,
    //		"sh_area_id": 3,
    //		"sh_area_name": "",
    //		"state": 1,
    //		"telephone": "13701140565",
    //		"unreadedMessage": 0,
    //		"userid": 263,
    //		"users_has_roles": [],
    //		"users_img": "",
    //		"weather": null
    //	}],
    //	"total": 176
    //}

    public class Res
    {
        public string error_code { get; set; }
        public List<ResUser> result { get; set; }
        public string total { get; set; }
    }

    public class ResUser
    {
        public int userid { get; set; }
        public string loginname { get; set; }
        public string password { get; set; }
    }
}
