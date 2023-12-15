//------------------------------------------------------------
// 偷我的代码就会被拖进黑暗空间
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

namespace Erinn
{
    public static partial class BaiduService
    {
        /// <summary>
        ///     敏感词检测结果
        /// </summary>
        public struct DetectSensitiveWordsResult
        {
            /// <summary>
            ///     操作是否成功
            /// </summary>
            public bool Success;

            /// <summary>
            ///     检测结果响应数据
            /// </summary>
            public Response ResponseData;

            /// <summary>
            ///     构造函数
            /// </summary>
            /// <param name="success">操作是否成功</param>
            /// <param name="response">检测结果响应数据</param>
            public DetectSensitiveWordsResult(bool success, Response response)
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
                ///     是否通过检测
                /// </summary>
                public bool isPass;

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
                    ///     敏感词数组
                    /// </summary>
                    public string[] words;

                    /// <summary>
                    ///     原文
                    /// </summary>
                    public string text;
                }
            }
        }
    }
}