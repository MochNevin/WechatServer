namespace Erinn
{
    /// <summary>
    ///     选项
    /// </summary>
    public static class BaiduServiceOptions
    {
        /// <summary>
        ///     严格检测
        /// </summary>
        public static class IsStrict
        {
            public const int Close = 0;
            public const int Open = 1;
        }

        /// <summary>
        ///     语言
        /// </summary>
        public static class Lang
        {
            public const string Chinese = "ZH";
            public const string English = "EN";
        }

        /// <summary>
        ///     长对话
        /// </summary>
        public static class LongChat
        {
            public const int Close = 0;
            public const int Open = 1;
        }
    }
}