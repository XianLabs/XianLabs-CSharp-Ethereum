using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;

namespace NethTest
{
    class WebReader
    {
        private WebClient wc { get; set; }
        private byte[] rawBytes { get; set; }
        private string webData { get; set; }

        public bool IsStringOnWebpage(string uri, string token)
        {
            wc = new WebClient();
            rawBytes = wc.DownloadData(uri);
            webData = System.Text.Encoding.UTF8.GetString(rawBytes);

            if(webData.Contains(token))
            {
                Console.WriteLine("Found line on page: " + token);
                return true;
            }

            return false;
        }

        public string GetReplacedTxID(string uri)
        {
            string NewtxID = "";

            wc = new WebClient();
            rawBytes = wc.DownloadData(uri);
            webData = System.Text.Encoding.UTF8.GetString(rawBytes);

            if (webData.Contains("href='/tx/"))
            {
                NewtxID = webData.Substring(webData.IndexOf("href='/tx/") + 10, 66); //32 bytes for tx hash, 32 * 2 + 2 (0x)
                Console.WriteLine("Found replaced tx: " + NewtxID);
                return NewtxID;
            }

            return null;
        }
    }
}
