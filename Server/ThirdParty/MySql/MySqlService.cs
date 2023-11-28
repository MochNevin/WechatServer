using MySql.Data.MySqlClient;

namespace Erinn
{
    /// <summary>
    ///     MySql数据库服务
    /// </summary>
    public static class MySqlService
    {
        /// <summary>
        ///     连接池
        /// </summary>
        private static readonly Stack<MySqlConnection> ConnectionPool = new();

        /// <summary>
        ///     连接设置
        /// </summary>
        public static MySqlServiceSetting ConnectSetting { get; private set; }

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
        ///     断开连接
        /// </summary>
        public static void Clear()
        {
            foreach (var connection in ConnectionPool)
                connection.Dispose();
            ConnectionPool.Clear();
        }

        /// <summary>
        ///     获取连接
        /// </summary>
        private static MySqlConnection GetConnection()
        {
            if (ConnectionPool.Count > 0)
                return ConnectionPool.Pop();
            return ConnectSetting.CreateConnection();
        }

        /// <summary>
        ///     连接
        /// </summary>
        public static void Connect(MySqlServiceSetting connectSetting) => ConnectSetting = connectSetting;

        /// <summary>
        ///     连接
        /// </summary>
        public static void Connect(string server, string port, string user, string password, string database) => ConnectSetting = new MySqlServiceSetting(server, port, user, password, database);

        /// <summary>
        ///     发送指令
        /// </summary>
        /// <param name="query">指令</param>
        /// <param name="connection">连接</param>
        /// <returns>MySqlCommand</returns>
        public static MySqlCommand SendQuery(string query, MySqlConnection connection) => new(query, connection);
    }
}