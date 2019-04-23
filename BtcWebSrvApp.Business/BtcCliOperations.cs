using BtcWebSrvApp.API;
using BtcWebSrvApp.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BtcWebSrvApp.Business
{
    public class BtcCliOperations
    {
        private readonly string _serviceTag = "btccliwebsrv";
        private readonly string _serviceUrl = "http://192.168.56.101:11111";
        private readonly string _serviceUser = "btcuser";
        private readonly string _servicePassword = "btcpwd";
        private readonly double? _balance;
        private readonly int? _txcount;

        public class Methods
        {
            public static string GetAddressesByLabel = "getaddressesbylabel";
            public static string GetNewAddressForLabel = "getnewaddress";
            public static string GetWalletInfo = "getwalletinfo";
            public static string SendToAddress = "sendtoaddress";
            public static string ListTransactions = "listtransactions";
            public static string Generate101 = "generate";
        }
        private BtcCliOperations()
        {

        }

        public BtcCliOperations(WalletContext wc)
        {
            _serviceTag = wc.Tag;
            _serviceUrl = wc.Url;
            _serviceUser = wc.User;
            _servicePassword = wc.Passsword;
            _balance = wc.Balance;
            _txcount = wc.TxCount;
            
        }
        HttpWebRequest GetBasicRequest1()
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(_serviceUrl);

            webRequest.ContentType = "application/json-rpc";

            webRequest.Method = "POST";

            var encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(_serviceUser + ":" + _servicePassword));
            
            webRequest.Headers.Add("Authorization", "Basic " + encoded);

            webRequest.Proxy = null;

            return webRequest;
        }
        HttpWebRequest GetBasicRequest()
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(_serviceUrl);
            webRequest.Credentials = new NetworkCredential(_serviceUser, _servicePassword);
            webRequest.ContentType = "application/json-rpc";

            webRequest.Method = "POST";

            return webRequest;
        }
        
        public IEnumerable<string> GetAddresses()
        {
            HttpWebRequest webRequest = GetBasicRequest1();

            JObject joe = new JObject();
            joe.Add(new JProperty("jsonrpc", "1.0"));
            joe.Add(new JProperty("id", "1"));
            joe.Add(new JProperty("method", Methods.GetAddressesByLabel));

            joe.Add(new JProperty("params", new JArray() { _serviceTag }));


            string s = JsonConvert.SerializeObject(joe);
            byte[] byteArray = Encoding.UTF8.GetBytes(s);
            webRequest.ContentLength = byteArray.Length;
            Stream dataStream = webRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();


            WebResponse webResponse = webRequest.GetResponse();
            var rs = webResponse.GetResponseStream();
            using (var sr = new StreamReader(rs))
            {
                var r = sr.ReadToEnd();

                var jo = JObject.Parse(r);

                var arr = jo["result"].Select(i => ((JProperty)i).Name).ToArray();

                return arr;
            }
        }
        public IEnumerable<Transaction> ListTransactions()
        {
            var result = new List<Transaction>();

            HttpWebRequest webRequest = GetBasicRequest();

            JObject joe = new JObject();
            joe.Add(new JProperty("jsonrpc", "1.0"));
            joe.Add(new JProperty("id", "1"));
            joe.Add(new JProperty("method", Methods.ListTransactions));
            joe.Add(new JProperty("params", new JArray() { "*",_txcount }));

            string s = JsonConvert.SerializeObject(joe);
            byte[] byteArray = Encoding.UTF8.GetBytes(s);
            webRequest.ContentLength = byteArray.Length;
            Stream dataStream = webRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();


            WebResponse webResponse = webRequest.GetResponse();

            var rs = webResponse.GetResponseStream();

            using (var sr = new StreamReader(rs))
            {
                var r = sr.ReadToEnd();

                var jo = JObject.Parse(r);

                var txarr = jo["result"] as JArray;

                foreach(var tx in txarr)
                {
                    try
                    {
                        var tr = TransactionFactory.From(tx);

                        result.Add(tr);

                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                }

                return result;
            }
        }
        public string GetDefaultWalletInfo()
        {
            HttpWebRequest webRequest = GetBasicRequest();

            JObject joe = new JObject();
            joe.Add(new JProperty("jsonrpc", "1.0"));
            joe.Add(new JProperty("id", "1"));
            joe.Add(new JProperty("method", Methods.GetWalletInfo));

            string s = JsonConvert.SerializeObject(joe);
            byte[] byteArray = Encoding.UTF8.GetBytes(s);
            webRequest.ContentLength = byteArray.Length;

            Stream dataStream = webRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse webResponse = webRequest.GetResponse();

            var rs = webResponse.GetResponseStream();

            using (var sr = new StreamReader(rs))
            {
                var r = sr.ReadToEnd();

                return r;
            }
        }
        public double? GetDefaultWalletBalance()
        {
            HttpWebRequest webRequest = GetBasicRequest();

            JObject joe = new JObject();
            joe.Add(new JProperty("jsonrpc", "1.0"));
            joe.Add(new JProperty("id", "1"));
            joe.Add(new JProperty("method", Methods.GetWalletInfo));

            string s = JsonConvert.SerializeObject(joe);
            byte[] byteArray = Encoding.UTF8.GetBytes(s);
            webRequest.ContentLength = byteArray.Length;

            Stream dataStream = webRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse webResponse = webRequest.GetResponse();

            var rs = webResponse.GetResponseStream();

            using (var sr = new StreamReader(rs))
            {
                var r = sr.ReadToEnd();

                var jo = JObject.Parse(r);

                return jo["result"]["balance"].Value<double?>();
            }
        }
        public int GetDefaultWalletTxCount()
        {
            HttpWebRequest webRequest = GetBasicRequest();

            JObject joe = new JObject();
            joe.Add(new JProperty("jsonrpc", "1.0"));
            joe.Add(new JProperty("id", "1"));
            joe.Add(new JProperty("method", Methods.GetWalletInfo));

            string s = JsonConvert.SerializeObject(joe);
            byte[] byteArray = Encoding.UTF8.GetBytes(s);
            webRequest.ContentLength = byteArray.Length;

            Stream dataStream = webRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse webResponse = webRequest.GetResponse();

            var rs = webResponse.GetResponseStream();

            using (var sr = new StreamReader(rs))
            {
                var r = sr.ReadToEnd();

                var jo = JObject.Parse(r);

                return jo["result"]["txcount"].Value<int>();
            }
        }
        public string GetNewAddressForLabel()
        {
            HttpWebRequest webRequest = GetBasicRequest();

            JObject joe = new JObject();

            joe.Add(new JProperty("jsonrpc", "1.0"));

            joe.Add(new JProperty("id", "1"));

            joe.Add(new JProperty("method", Methods.GetNewAddressForLabel));

            joe.Add(new JProperty("params", new JArray() { _serviceTag }));
            
            string s = JsonConvert.SerializeObject(joe);
            byte[] byteArray = Encoding.UTF8.GetBytes(s);
            webRequest.ContentLength = byteArray.Length;
            Stream dataStream = webRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse webResponse = webRequest.GetResponse();
            var rs = webResponse.GetResponseStream();
            using (var sr = new StreamReader(rs))
            {
                var r = sr.ReadToEnd();

                var jo = JObject.Parse(r);

                return jo["result"].ToString();
            }
        }
        public void GenTestCoins()
        {
            HttpWebRequest webRequest = GetBasicRequest();

            JObject joe = new JObject();

            joe.Add(new JProperty("jsonrpc", "1.0"));

            joe.Add(new JProperty("id", "1"));

            joe.Add(new JProperty("method", Methods.Generate101));

            joe.Add(new JProperty("params", new JArray() { 101 }));

            string s = JsonConvert.SerializeObject(joe);
            byte[] byteArray = Encoding.UTF8.GetBytes(s);
            webRequest.ContentLength = byteArray.Length;
            Stream dataStream = webRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse webResponse = webRequest.GetResponse();
            var rs = webResponse.GetResponseStream();
            using (var sr = new StreamReader(rs))
            {
                var r = sr.ReadToEnd();

                var jo = JObject.Parse(r);

               
            }
        }


        public string SendToAddress(string address, double amount)
        {
            HttpWebRequest webRequest = GetBasicRequest();

            JObject joe = new JObject();

            joe.Add(new JProperty("jsonrpc", "1.0"));

            joe.Add(new JProperty("id", "1"));

            joe.Add(new JProperty("method", Methods.SendToAddress));

            joe.Add(new JProperty("params", new JArray() { address,amount }));

            string s = JsonConvert.SerializeObject(joe);
            byte[] byteArray = Encoding.UTF8.GetBytes(s);
            webRequest.ContentLength = byteArray.Length;
            Stream dataStream = webRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse webResponse = webRequest.GetResponse();
            var rs = webResponse.GetResponseStream();
            using (var sr = new StreamReader(rs))
            {
                var r = sr.ReadToEnd();

                var jo = JObject.Parse(r);

                return jo["result"].ToString();
            }
        }
    }
}
