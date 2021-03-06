﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ESLTracker.BusinessLogic.General;
using ESLTracker.Utils;

namespace ESLTracker.BusinessLogic.General
{
    public class HTTPService : IHTTPService
    {
        private ITrackerFactory trackerFactory;

        static HTTPService()
        {
            //enable TSL 1.1 and 1.2
            //https://stackoverflow.com/questions/28286086/default-securityprotocol-in-net-4-5  
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        }

        public HTTPService(ITrackerFactory trackerFactory)
        {
            this.trackerFactory = trackerFactory;
        }

        public string SendGetRequest(string url)
        {
            var request = WebRequest.Create(url);
            if (request is HttpWebRequest)
            {
                var webRequest = request as HttpWebRequest;
                webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:18.0) Gecko/20100101 Firefox/18.0";
                webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                webRequest.Method = "GET";
                webRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }
            var response = request.GetResponse();
            string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return responseString;
        }

        public string SendPostRequest(string url, string postData)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:18.0) Gecko/20100101 Firefox/18.0";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            request.Method = "POST";
            request.ContentLength = postData.Length;

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return responseString;
        }
    }
}
