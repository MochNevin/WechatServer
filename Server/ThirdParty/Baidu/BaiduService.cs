using Newtonsoft.Json;

namespace Erinn
{
    /// <summary>
    ///     百度服务
    /// </summary>
    public static class BaiduService
    {
        /// <summary>
        ///     HttpClient池
        /// </summary>
        private static readonly Stack<HttpClient> HttpClientPool = new();

        /// <summary>
        ///     设置
        /// </summary>
        public static BaiduServiceSetting ColaSetting { get; private set; }

        /// <summary>
        ///     连接
        /// </summary>
        public static void Connect(BaiduServiceSetting setting) => ColaSetting = setting;

        /// <summary>
        ///     连接
        /// </summary>
        /// <param name="colaKey">密匙</param>
        /// <param name="uid">账号</param>
        /// <param name="appKey">密码</param>
        public static void Connect(string colaKey, string uid, string appKey) => ColaSetting = new BaiduServiceSetting(colaKey, uid, appKey);

        /// <summary>
        ///     获取HttpClient
        /// </summary>
        /// <returns>获得的HttpClient</returns>
        private static HttpClient Pop()
        {
            if (HttpClientPool.Count > 0)
                return HttpClientPool.Pop();
            return new HttpClient();
        }

        /// <summary>
        ///     推入HttpClient
        /// </summary>
        /// <param name="httpClient">HttpClient</param>
        private static void Push(HttpClient httpClient) => HttpClientPool.Push(httpClient);

        /// <summary>
        ///     清空
        /// </summary>
        public static void Clear()
        {
            foreach (var httpClient in HttpClientPool)
                httpClient.Dispose();
            HttpClientPool.Clear();
        }

        /// <summary>
        ///     检测敏感词
        /// </summary>
        /// <param name="wordStr">文本</param>
        /// <param name="isStrict">是否严格</param>
        /// <returns>(是否发送成功, 检测响应)</returns>
        public static async Task<DetectSensitiveWordsResult> DetectSensitiveWords(string wordStr, int isStrict)
        {
            const string apiUrl = "https://luckycola.com.cn/tools/sensiWords";
            var httpClient = Pop();
            using var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("ColaKey", ColaSetting.ColaKey),
                new KeyValuePair<string, string>("wordStr", wordStr),
                new KeyValuePair<string, string>("isStrict", isStrict.ToString())
            });
            using var response = await httpClient.PostAsync(apiUrl, requestContent);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<DetectSensitiveWordsResult.Response>(responseBody);
                Push(httpClient);
                return new DetectSensitiveWordsResult(true, data);
            }

            Push(httpClient);
            return new DetectSensitiveWordsResult(false, default);
        }

        /// <summary>
        ///     翻译
        /// </summary>
        /// <param name="text">文本</param>
        /// <param name="fromLang">原文语言</param>
        /// <param name="toLang">译文语言</param>
        /// <returns>(是否发送成功, 翻译响应)</returns>
        public static async Task<TranslateResult> Translate(string text, string fromLang, string toLang)
        {
            const string apiUrl = "https://luckycola.com.cn/tools/fanyi";
            var httpClient = Pop();
            using var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("text", text),
                new KeyValuePair<string, string>("ColaKey", ColaSetting.ColaKey),
                new KeyValuePair<string, string>("fromlang", fromLang),
                new KeyValuePair<string, string>("tolang", toLang)
            });
            using var response = await httpClient.PostAsync(apiUrl, requestContent);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<TranslateResult.Response>(responseBody);
                Push(httpClient);
                return new TranslateResult(true, data);
            }

            Push(httpClient);
            return new TranslateResult(false, default);
        }

        /// <summary>
        ///     聊天
        /// </summary>
        /// <param name="ques">文本</param>
        /// <param name="isLongChat">是否上下文对话</param>
        /// <returns>(是否发送成功, 聊天响应)</returns>
        public static async Task<ChatResult> Chat(string ques, int isLongChat)
        {
            const string apiUrl = "https://luckycola.com.cn/ai/openwxyy";
            var httpClient = Pop();
            using var requestContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("ques", ques),
                new KeyValuePair<string, string>("appKey", ColaSetting.AppKey),
                new KeyValuePair<string, string>("uid", ColaSetting.Uid),
                new KeyValuePair<string, string>("isLongChat", isLongChat.ToString())
            });
            using var response = await httpClient.PostAsync(apiUrl, requestContent);
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<ChatResult.Response>(responseBody);
                Push(httpClient);
                return new ChatResult(true, data);
            }

            Push(httpClient);
            return new ChatResult(false, default);
        }
    }
}