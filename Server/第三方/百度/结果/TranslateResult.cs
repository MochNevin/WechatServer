//------------------------------------------------------------
// 偷我的代码就会被拖进黑暗空间
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

namespace Erinn
{
    public static partial class BaiduService
    {
        /// <summary>
        ///     翻译结果
        /// </summary>
        public struct TranslateResult
        {
            /// <summary>
            ///     操作是否成功
            /// </summary>
            public bool Success;

            /// <summary>
            ///     翻译结果响应数据
            /// </summary>
            public Response ResponseData;

            /// <summary>
            ///     构造函数
            /// </summary>
            /// <param name="success">操作是否成功</param>
            /// <param name="response">翻译结果响应数据</param>
            public TranslateResult(bool success, Response response)
            {
                Success = success;
                ResponseData = response;
            }

            /// <summary>
            ///     响应
            /// </summary>
            public struct Response
            {
                /// <summary>
                ///     响应代码
                /// </summary>
                public int code;

                /// <summary>
                ///     响应消息
                /// </summary>
                public string msg;

                /// <summary>
                ///     数据
                /// </summary>
                public Data data;

                /// <summary>
                ///     数据
                /// </summary>
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
    }
}