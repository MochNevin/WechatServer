namespace Erinn
{
    /// <summary>
    ///     敏感词检测结果
    /// </summary>
    public struct DetectSensitiveWordsResult
    {
        public bool Success;
        public Response ResponseData;

        public DetectSensitiveWordsResult(bool success, Response response)
        {
            Success = success;
            ResponseData = response;
        }

        public struct Response
        {
            public int code;
            public string msg;

            /// <summary>
            ///     是否通过检测
            /// </summary>
            public bool isPass;

            public Data data;

            public struct Data
            {
                /// <summary>
                ///     敏感词
                /// </summary>
                public string[] words;

                /// <summary>
                ///     原文
                /// </summary>
                public string text;
            }
        }
    }

    /// <summary>
    ///     翻译结果
    /// </summary>
    public struct TranslateResult
    {
        public bool Success;
        public Response ResponseData;

        public TranslateResult(bool success, Response response)
        {
            Success = success;
            ResponseData = response;
        }

        public struct Response
        {
            public int code;
            public string msg;
            public Data data;

            public struct Data
            {
                /// <summary>
                ///     原文
                /// </summary>
                public string text;

                /// <summary>
                ///     译文
                /// </summary>
                public string dst;
            }
        }
    }

    /// <summary>
    ///     聊天结果
    /// </summary>
    public struct ChatResult
    {
        public bool Success;
        public Response ResponseData;

        public ChatResult(bool success, Response response)
        {
            Success = success;
            ResponseData = response;
        }

        public struct Response
        {
            public int code;
            public string msg;
            public Data data;

            public struct Data
            {
                /// <summary>
                ///     结果
                /// </summary>
                public string result;

                /// <summary>
                ///     原文
                /// </summary>
                public string countMsg;

                /// <summary>
                ///     是否是上下文对话模式
                /// </summary>
                public int longChat;
            }
        }
    }
}