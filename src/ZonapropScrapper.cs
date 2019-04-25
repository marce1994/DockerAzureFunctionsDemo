using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace SouthWorks.src
{
    public class ZonapropScrapper
    {
        private readonly string webUrl;
        public ZonapropScrapper(string url)
        {
            this.webUrl = url;
        }

        public ZonapropScrapper()
        {
            this.webUrl = "https://www.zonaprop.com.ar";
        }

        public async Task<IEnumerable<dynamic>> GetDataAsync(int page)
        {
            var props = new List<dynamic>();
            var url = webUrl + $"/departamento-alquiler-capital-federal-orden-publicado-descendente-pagina-{page}.html";
            try
            {
                return await PullPageProps(url);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return null;
        }

        private async Task<IEnumerable<dynamic>> PullPageProps(string url)
        {
            var propsInformation = new List<dynamic>();
            var web = new HtmlWeb();
            var document = await web.LoadFromWebAsync(url);

            var statusCode = (int)web.StatusCode;
            if (!(statusCode >= 200 && statusCode < 300))
            {
                return null;
            };

            var propInfoUrls = GetPagePropUrls(document);
            List<Task<dynamic>> getPropInformationTasks = new List<Task<dynamic>>();
            foreach (var propInfoUrl in propInfoUrls)
            {
                getPropInformationTasks.Add(GetPropInformation(propInfoUrl));
            }

            Task.WaitAll(getPropInformationTasks.ToArray());
            propsInformation.AddRange(getPropInformationTasks.Select(x => x.Result));
            return propsInformation;
        }

        private IEnumerable<string> GetPagePropUrls(HtmlDocument document)
        {
            var pagePropsUrls = new List<string>();
            foreach (var Node in document.GetElementbyId("avisos-content").DescendantsAndSelf())
            {
                string href = Node.GetAttributeValue("data-href", "");
                if (href != "") pagePropsUrls.Add(webUrl + href);
            }
            return pagePropsUrls;
        }

        private async Task<dynamic> GetPropInformation(string Url)
        {
            var web = new HtmlWeb();
            var document = await web.LoadFromWebAsync(Url);

            var statusCode = (int)web.StatusCode;
            if (!(statusCode >= 200 && statusCode < 300))
            {
                return null;
            };

            var properties = new Dictionary<string, string>();
            try
            {
                var type = document.QuerySelectorAll("div.price-operation").First().InnerHtml;
                properties.Add("tipo", type);

                var price = document.QuerySelectorAll("div.price-items").First().Descendants().First().InnerHtml;
                properties.Add("precio", price);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            var features = document.QuerySelectorAll("li.icon-feature");
            foreach (var feature in features)
            {
                var property = feature.QuerySelectorAll("i.icon-f").First().GetClasses().ElementAt(1).Split('-').Last();
                var value = feature.QuerySelectorAll("b").First().InnerHtml;
                properties.Add(property, value);
            }
            Console.WriteLine(Url);
            return properties;
        }
    }
}