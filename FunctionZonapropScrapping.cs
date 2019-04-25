using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SouthWorks.src;
using Newtonsoft.Json;

namespace SouthWorks
{
    public static class FunctionZonapropScrapping
    {
        [FunctionName("FunctionZonapropScrapping")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            int pageNumber = 0;
            if (!int.TryParse(req.Query["pageNumber"], out pageNumber)) return new BadRequestResult();

            var scrapper = new ZonapropScrapper();
            var result = await scrapper.GetDataAsync(pageNumber);

            return new JsonResult(result)
            {
                SerializerSettings = new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented
                }
            };
        }
    }
}
