using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ChatGPTExample
{
    public class ChatGPT
    {
        private static readonly HttpClient client = new HttpClient();
        private string prompt = "";
        private string conversationId;

        public ChatGPT(string apiKey)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        }

        public async Task<string> SendMessageAsync(string message)
        {
            if (conversationId == null)
            {
                // 创建新会话
                var response = await client.PostAsync("https://api.openai.com/v1/davinci-coding/conversations", null);
                var content = await response.Content.ReadAsStringAsync();
                conversationId = JObject.Parse(content)["id"].ToString();
            }

            // 更新提示符
            prompt += $"用户: {message}\nAI:";

            // 发送请求到OpenAI API
            var jsonContent = new StringContent("{\"prompt\": \"" + prompt + "\", \"max_tokens\": 150, \"temperature\": 0.9}", Encoding.UTF8, "application/json");
            var responseMessage = await client.PostAsync($"https://api.openai.com/v1/davinci-coding/conversations/{conversationId}/completions", jsonContent);
            var responseContent = await responseMessage.Content.ReadAsStringAsync();

            // 解析响应并更新提示符
            var answer = JObject.Parse(responseContent)["choices"]?[0]?["text"]?.ToString().Trim();
            prompt += $"{answer}\n";

            return answer;
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            // 替换为你自己的API密钥
            var chatGPT = new ChatGPT(RootNS.Models.Gval.ChatGpt_apikey);

            Console.WriteLine("开始聊天（输入'exit'退出）：");

            while (true)
            {
                Console.Write("用户: ");
                var input = Console.ReadLine();
                if (input == "exit")
                    break;

                var response = await chatGPT.SendMessageAsync(input);
                Console.WriteLine($"AI: {response}");
            }
        }
    }
}