using System;
using HtmlAgilityPack;

namespace PenileNET.Modules {
    public class WebUtility {
        public void GetHtml(string url) {
            var web = new HtmlWeb();
            var doc = web.Load(url);

            Console.WriteLine(doc.ParsedText);
        }
    }
}