using MySql.Data.MySqlClient;

namespace Erinn
{
    /// <summary>
    ///     MySql连接设置
    /// </summary>
    public readonly struct MySqlServiceSetting
    {
        /// <summary>
        ///     服务器
        /// </summary>
        public readonly string Server;

        /// <summary>
        ///     端口
        /// </summary>
        public readonly string Port;

        /// <summary>
        ///     用户名
        /// </summary>
        public readonly string UserID;

        /// <summary>
        ///     密码
        /// </summary>
        public readonly string Password;

        /// <summary>
        ///     数据库
        /// </summary>
        public readonly string Database;

        /// <summary>
        ///     String
        /// </summary>
        public override string ToString() => $"[{Server}] [{Port}] [{UserID}] [{Password}] [{Database}]";

        /// <summary>
        ///     构造
        /// </summary>
        /// <param name="server">服务器</param>
        /// <param name="port">端口</param>
        /// <param name="user">summary</param>
        /// <param name="password">密码</param>
        /// <param name="database">数据库</param>
        public MySqlServiceSetting(string server, string port, string user, string password, string database)
        {
            Server = server;
            Port = port;
            UserID = user;
            Password = password;
            Database = database;
        }

        /// <summary>
        ///     获取连接字符串
        /// </summary>
        /// <returns>连接字符串</returns>
        public string GetConnectionString() => $"Server={Server};Port={Port};UserID={UserID};Password={Password};Database={Database};";

        /// <summary>
        ///     建立连接
        /// </summary>
        /// <returns>连接</returns>
        public MySqlConnection CreateConnection() => new(GetConnectionString());
    }
}