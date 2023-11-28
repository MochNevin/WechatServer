namespace Erinn
{
    /// <summary>
    ///     设置
    /// </summary>
    public struct BaiduServiceSetting
    {
        /// <summary>
        ///     密匙
        /// </summary>
        public string ColaKey;

        /// <summary>
        ///     账号
        /// </summary>
        public string Uid;

        /// <summary>
        ///     密码
        /// </summary>
        public string AppKey;

        /// <summary>
        ///     String
        /// </summary>
        public override string ToString() => $"[{ColaKey}] [{Uid}] [{AppKey}]";

        /// <summary>
        ///     连接
        /// </summary>
        /// <param name="colaKey">密匙</param>
        /// <param name="uid">账号</param>
        /// <param name="appKey">密码</param>
        public BaiduServiceSetting(string colaKey, string uid, string appKey)
        {
            ColaKey = colaKey;
            Uid = uid;
            AppKey = appKey;
        }
    }
}