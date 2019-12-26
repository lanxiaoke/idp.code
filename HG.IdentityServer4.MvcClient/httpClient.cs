using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace HG.IdentityServer4.MvcClient
{
    public class HttpClientX
    {
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
}