using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace LondonBicycles.Data.HttpRequester
{
    public class HttpRequest
    {
        public async static Task<HttpResponseMessage> Post(string url, object data)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url);

            var jsonData = JsonConvert.SerializeObject(data);
            var content = new Dictionary<string, string>();
            content.Add("data", jsonData);
            request.Content = new FormUrlEncodedContent(content);
            var client = new HttpClient();
            return await client.SendAsync(request);
        }

        public async static Task<T> Get<T>(string url)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);        
            var client = new HttpClient();
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<T>(content);
            return responseData;
        }
    }
}
