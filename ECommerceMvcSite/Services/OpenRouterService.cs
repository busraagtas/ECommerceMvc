using System.Net.Http;
using System.Net.Http.Headers;
using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ECommerceMvcSite.Services
{
    public class OpenRouterService
    {
        private readonly string apiKey = "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"; 
        private readonly HttpClient httpClient;

        public OpenRouterService()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://openrouter.ai/api/v1/");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "https://senin-site-adresin.com");
            httpClient.DefaultRequestHeaders.Add("X-Title", "E-Ticaret ChatBot");
        }

        public async Task<string> GetChatbotResponse(string userPrompt)
        {
            var request = new
            {
                model = "openai/gpt-3.5-turbo",  // Claude veya Mixtral de seçebilirsin
                messages = new[]
                {
                new { role = "user", content = userPrompt }
            }
            };

            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync("chat/completions", content);
            var result = await response.Content.ReadAsStringAsync();

            dynamic parsed = JsonConvert.DeserializeObject(result);
            return parsed.choices[0].message.content.ToString();
        }
    }
}