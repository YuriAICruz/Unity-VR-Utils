using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace VrSafety
{
    public class Response<T>
    {
        public bool success;
        public int code;
        public string json;
        public T data;
        public string error;

        public Response()
        {
        }

        public Response(bool success, int code, string json, T data, string error)
        {
            this.success = success;
            this.code = code;
            this.json = json;
            this.data = data;
            this.error = error;
        }

        public override string ToString()
        {
            return $"success: {success}, code {code}, json {json}, data {data}, error: {error}";
        }
    }

    public class RawResponse<T>
    {
        public bool success;
        public int code;
        public string body;
    }

    public static class HttpComm
    {
        public static int Timeout = 5000; // 20 seconds

        public static void Post<T>(string path, string endpoint, string body, Action<Response<T>> callback = null)
        {
            HttpWebRequest request;
            try
            {
                request = CreateWebRequest(path + endpoint, "POST", body);
            }
            catch (Exception e)
            {
                callback?.Invoke(new Response<T>(false, 400, "", default(T), e.ToString()));
                return;
            }

            GetResponse(request, callback);
        }

        public static void Get<T>(string path, string endpoint, Action<Response<T>> callback = null)
        {
            HttpWebRequest request;
            try
            {
                request = CreateWebRequest(path + endpoint, "GET");
            }
            catch (Exception e)
            {
                callback?.Invoke(new Response<T>(false, 400, "", default(T), e.ToString()));
                return;
            }

            GetResponse(request, callback);
        }

        public static void Put<T>(string path, string endpoint, string body, Action<Response<T>> callback = null)
        {
            HttpWebRequest request;
            try
            {
                request = CreateWebRequest(path + endpoint, "PUT", body);
            }
            catch (Exception e)
            {
                callback?.Invoke(new Response<T>(false, 400, "", default(T), e.ToString()));
                return;
            }

            GetResponse(request, callback);
        }

        static void GetRequestStream(HttpWebRequest request, string body)
        {
            //var encoding = new ASCIIEncoding ();
            var encoding = new UTF8Encoding();
            var bytes = encoding.GetBytes(body);
            request.ContentLength = bytes.Length;

            var newStream = request.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();
            
            return;

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(body);
                streamWriter.Flush();
                streamWriter.Close();
            }
        }

        static void GetResponse<T>(HttpWebRequest request, Action<Response<T>> callback = null)
        {
            var trd = new Thread(() =>
            {
                try
                {
                    var httpResponse = (HttpWebResponse) request.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var json = streamReader.ReadToEnd();
                        var res = default(T);
                        var error = "";

                        try
                        {
                            res = JsonConvert.DeserializeObject<T>(json);
                        }
                        catch (Exception e)
                        {
                            error = e.ToString();
                        }

                        var code = (int) httpResponse.StatusCode;
                        callback?.Invoke(new Response<T>(code >= 200 && code < 300, code, json, res, error));
                    }
                }
                catch (WebException e)
                {
                    if(e.Response == null)
                        callback?.Invoke(new Response<T>(false, 400, "", default(T), e.ToString()));
                    
                    var code = (int) ((HttpWebResponse) e.Response).StatusCode;
                    
                    callback?.Invoke(new Response<T>(false, code, "", default(T), e.ToString()));
                }
                catch (Exception e)
                {
                    callback?.Invoke(new Response<T>(false, 400, "", default(T), e.ToString()));
                }
            });
            trd.Start();
        }

        static HttpWebRequest CreateWebRequest(string url, string method, string body = "")
        {
            if (string.IsNullOrEmpty(url))
                throw new NullReferenceException();

            if (string.IsNullOrEmpty(method))
                throw new NullReferenceException();

            var req = WebRequest.CreateHttp(new Uri(url));
            req.Method = method;

            req.Timeout = Timeout;
            req.ReadWriteTimeout = Timeout * 2;

            //req.KeepAlive = false;

            req.Headers = new WebHeaderCollection();
            
            req.ContentType = "application/json";

            //req.Headers.Add("Content-Type", "application/json");

            if (!string.IsNullOrEmpty(body))
                GetRequestStream(req, body);

            return req;
        }
    }
}