using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using System.IO.Compression;
 
using System.Collections.Specialized;

namespace Zhengdi.Framework.Help
{
    public class HttpHelp
    {
        private HttpWebResponse _Response;
        private HttpHelp() { }
        public static HttpHelp GetRequest(string url, string ContentType)
        {
            HttpWebRequest web =  (HttpWebRequest)System.Net.WebRequest.Create(url);
            web.ContentType = ContentType == string.Empty ? "text/html; charset=UTF-8" : ContentType;
            web.Method = "GET";
            web.UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:2.0.1) Gecko/20100101 Firefox/4.0.1";
            var  response = new HttpHelp() { _Response = (HttpWebResponse)web.GetResponse() };
            return response;
        }
        public static HttpHelp PostRequest(string url, string ContentType, string data, Encoding encoding)
        {
            HttpWebRequest web = (HttpWebRequest)System.Net.WebRequest.Create(url);
            web.ContentType = ContentType == string.Empty ? "application/x-www-form-urlencoded" : ContentType;
            web.CookieContainer = new CookieContainer();
            web.Method = "POST";
            web.AllowAutoRedirect = false;
            web.UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:2.0.1) Gecko/20100101 Firefox/4.0.1";
            byte[] byteArray = encoding.GetBytes(data);
            web.ContentLength = byteArray.Length;
            Stream newStream = web.GetRequestStream();
            newStream.Write(byteArray, 0, byteArray.Length);
            newStream.Close();
            HttpHelp response = new HttpHelp() { _Response = (HttpWebResponse)web.GetResponse() };
            return response;
        }
        private static HttpHelp HttpMethod(string url, string ContentType, Encoding encoding, CookieCollection cookies, StringBuilder data)
        {
            try
            {

                HttpWebRequest web = (HttpWebRequest)System.Net.WebRequest.Create(url);
                web.ContentType = ContentType == string.Empty ? "application/x-www-form-urlencoded" : ContentType;
                web.Method = "POST";
                web.CookieContainer = new CookieContainer();
                if (cookies != null)
                {
                    foreach (Cookie c in cookies)
                    {
                        web.CookieContainer.Add(c);
                    }
                } 
                web.AllowAutoRedirect = false;
                web.UserAgent = "Mozilla/5.0 (Windows NT 5.1; rv:2.0.1) Gecko/20100101 Firefox/4.0.1";
                byte[] byteArray = encoding.GetBytes(data.ToString());
                web.ContentLength = byteArray.Length;
                Stream newStream = web.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);
                newStream.Close();
                HttpHelp response = new HttpHelp() { _Response = (HttpWebResponse)web.GetResponse() };
                return response;
            }
            catch (Exception ee)
            {
                throw new Exception(ee.Message, ee);
            }
        }

        public string GetRemoteString()
        {
          return GetRemoteString(Encoding.UTF8);
        }
        public CookieCollection GetCookies()
        {
            return _Response.Cookies;
        }
        public string GetRemoteString(Encoding coding)
        {
            Stream stream = _Response.GetResponseStream();
            if (this._Response.ContentEncoding.ToLower().Contains("gzip"))
               stream =new  GZipStream(stream,  CompressionMode.Decompress);
            StreamReader read = new StreamReader(stream,coding);
            string data = read.ReadToEnd();
            read.Dispose();
            this._Response.Close();
            return data;
        }
        public byte[] GetRemoteBytes()
        {
            Stream stream = _Response.GetResponseStream();
            if (this._Response.ContentEncoding.ToLower().Contains("gzip"))
                stream = new GZipStream(stream, CompressionMode.Decompress);
            byte[] array = new byte[stream.Length];
            stream.Read(array, 0, array.Length);
            stream.Dispose();
            this._Response.Close();
            return array; 

        }
        public void DownloadFile(string savePath)
        {
            try
            {
                WebClient mywebclient = new WebClient();
                mywebclient.DownloadFile(this._Response.ResponseUri, savePath);
                this._Response.Close();
                mywebclient.Dispose();
            }
            catch (Exception ee)
            {
                throw new Exception(ee.Message);
            }

        }

        public byte[] DownloadFile()
        {
            WebClient mywebclient = new WebClient();
            var array = mywebclient.DownloadData(this._Response.ResponseUri);
            this._Response.Close();
            mywebclient.Dispose();
            return array;
        }
        public static  string UploadFile(string  url ,string filePath)
        {
          
            WebClient myWebClient = new WebClient() { Credentials = CredentialCache.DefaultCredentials };
            byte[] responseArray = myWebClient.UploadFile(url, "POST", filePath);
            string result = Encoding.Default.GetString(responseArray, 0, responseArray.Length);
            myWebClient.Dispose();
            return result;
        }
 
    }
}
