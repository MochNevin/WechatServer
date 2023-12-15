//------------------------------------------------------------
// 偷我的代码就会被拖进黑暗空间
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

using MySql.Data.MySqlClient;

namespace Erinn
{
    /// <summary>
    ///     MySql数据库服务
    /// </summary>
    public static class MySqlService
    {
        /// <summary>
        ///     连接设置
        /// </summary>
        private static MySqlServiceSetting _connectSetting;

        /// <summary>
        ///     连接池
        /// </summary>
        private static readonly Stack<MySqlConnection> ConnectionPool = new();

        /// <summary>
        ///     获取连接
        /// </summary>
        private static MySqlConnection GetConnection()
        {
            lock (ConnectionPool)
            {
                if (ConnectionPool.Count > 0)
                    return ConnectionPool.Pop();
                return _connectSetting.CreateConnection();
            }
        }

        /// <summary>
        ///     连接
        /// </summary>
        public static void Connect(string server, string port, string user, string password, string database) => _connectSetting = new MySqlServiceSetting(server, port, user, password, database);

        /// <summary>
        ///     获取MySqlConnection
        /// </summary>
        /// <returns>MySqlConnection</returns>
        public static async Task<MySqlConnection> Pop()
        {
            var connection = GetConnection();
            await connection.OpenAsync();
            return connection;
        }

        /// <summary>
        ///     推入池
        /// </summary>
        /// <param name="connection">MySqlConnection</param>
        public static void Push(MySqlConnection connection) => ConnectionPool.Push(connection);

        /// <summary>
        ///     发送指令
        /// </summary>
        /// <param name="query">指令</param>
        /// <param name="connection">连接</param>
        /// <returns>MySqlCommand</returns>
        public static MySqlCommand SendQuery(string query, MySqlConnection connection) => new(query, connection);

        /// <summary>
        ///     MySql连接设置
        /// </summary>
        private readonly struct MySqlServiceSetting
        {
            /// <summary>
            ///     服务器
            /// </summary>
            private readonly string _server;

            /// <summary>
            ///     端口
            /// </summary>
            private readonly string _port;

            /// <summary>
            ///     用户名
            /// </summary>
            private readonly string _userId;

            /// <summary>
            ///     密码
            /// </summary>
            private readonly string _password;

            /// <summary>
            ///     数据库
            /// </summary>
            private readonly string _database;

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
                _server = server;
                _port = port;
                _userId = user;
                _password = password;
                _database = database;
            }

            /// <summary>
            ///     获取连接字符串
            /// </summary>
            /// <returns>连接字符串</returns>
            private string GetConnectionString() => $"Server={_server};Port={_port};UserID={_userId};Password={_password};Database={_database};";

            /// <summary>
            ///     建立连接
            /// </summary>
            /// <returns>连接</returns>
            public MySqlConnection CreateConnection() => new(GetConnectionString());
        }
    }
}