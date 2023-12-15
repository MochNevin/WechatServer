//------------------------------------------------------------
// 偷我的代码就会被拖进黑暗空间
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

using CommandLine;
using Newtonsoft.Json;

#pragma warning disable CS8600
#pragma warning disable CS8602
#pragma warning disable CS8618

namespace Erinn
{
    /// <summary>
    ///     程序入口
    /// </summary>
    public static class Entry
    {
        /// <summary>
        ///     入口方法
        /// </summary>
        public static async Task Main()
        {
            ConnectThirdParty();
            var server = new MasterServer();
            server.Init(NetworkProtocolType.Kcp);
            server.ChangeLog(false);
            _ = new MessageManager(server);
            server.Start(7777);
            await server.Update();
        }

        /// <summary>
        ///     连接第三方
        /// </summary>
        private static void ConnectThirdParty()
        {
            var jsonData = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "CommandLineOptions.txt"));
            var options = JsonConvert.DeserializeObject<CommandLineOptions>(jsonData);
            var mySqlSetting = options.MySqlSetting.Split(',');
            MySqlService.Connect(mySqlSetting[0], mySqlSetting[1], mySqlSetting[2], mySqlSetting[3], mySqlSetting[4]);
            var mailSetting = options.MailSetting.Split(',');
            MailService.Connect(mailSetting[0], mailSetting[1], mailSetting[2], mailSetting[3]);
            var baiduSetting = options.BaiduSetting.Split(',');
            BaiduService.Connect(baiduSetting[0], baiduSetting[1], baiduSetting[2]);
        }

        /// <summary>
        ///     命令行选项
        /// </summary>
        public sealed record CommandLineOptions
        {
            /// <summary>
            ///     数据库
            /// </summary>
            [Option("MySql", Required = true, Default = null)]
            public string MySqlSetting { get; set; }

            /// <summary>
            ///     邮箱
            /// </summary>
            [Option("Mail", Required = true, Default = null)]
            public string MailSetting { get; set; }

            /// <summary>
            ///     百度
            /// </summary>
            [Option("Baidu", Required = true, Default = null)]
            public string BaiduSetting { get; set; }
        }
    }
}