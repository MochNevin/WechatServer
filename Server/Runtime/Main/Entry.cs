using CommandLine;

#pragma warning disable CS8600
#pragma warning disable CS8618

namespace Erinn
{
    public class Entry
    {
        public static async Task Main()
        {
            ClearThirdParty();
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
            CommandLineOptions options = null;
            Parser.Default.ParseArguments<CommandLineOptions>(Environment.GetCommandLineArgs()).WithParsed(commandLineOptions => options = commandLineOptions);
            if (options == null)
                return;
            var mySqlSetting = options.MySqlSetting.Split(',');
            MySqlService.Connect(mySqlSetting[0], mySqlSetting[1], mySqlSetting[2], mySqlSetting[3], mySqlSetting[4]);
            var mailSetting = options.MailSetting.Split(',');
            MailService.Connect(mailSetting[0], mailSetting[1], mailSetting[2], mailSetting[3]);
            var baiduSetting = options.BaiduSetting.Split(',');
            BaiduService.Connect(baiduSetting[0], baiduSetting[1], baiduSetting[2]);
        }

        /// <summary>
        ///     清理第三方
        /// </summary>
        private static void ClearThirdParty()
        {
            MySqlService.Clear();
            MailService.Clear();
            BaiduService.Clear();
        }

        /// <summary>
        ///     命令行选项
        /// </summary>
        public sealed class CommandLineOptions
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