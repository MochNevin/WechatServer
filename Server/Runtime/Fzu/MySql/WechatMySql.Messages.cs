using System.Data;

namespace Erinn
{
    public static partial class WechatMySql
    {
        public static class Messages
        {
            public static async Task Insert(string email1, string email2, string content)
            {
                const string query = "INSERT INTO messages (email1, email2, content) VALUES (@email1, @email2, @content)";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@email1", email1);
                    cmd.Parameters.AddWithValue("@email2", email2);
                    cmd.Parameters.AddWithValue("@content", content);
                    await cmd.ExecuteNonQueryAsync();
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            public static async Task<List<Message>> GetRecentMessages(string email1, string email2)
            {
                const string query = "SELECT * FROM messages WHERE (email1 = @email1 AND email2 = @email2) OR (email1 = @email2 AND email2 = @email1) ORDER BY id DESC LIMIT 30";
                var connection = await MySqlService.Pop();
                try
                {
                    var messages = new List<Message>();
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@email1", email1);
                    cmd.Parameters.AddWithValue("@email2", email2);
                    await using var reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            var message = new Message(reader.GetInt32("id"), reader.GetString("email1"), reader.GetString("content"), reader.GetDateTime("sendtimestamp"));
                            messages.Add(message);
                        }
                    }

                    return messages;
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            public static async Task Truncate()
            {
                const string query = "TRUNCATE TABLE messages";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    await cmd.ExecuteNonQueryAsync();
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            public struct Message
            {
                public int id;
                public string email1;
                public string content;
                public DateTime sendtimestamp;

                public Message(int id, string email1, string content, DateTime sendtimestamp)
                {
                    this.id = id;
                    this.email1 = email1;
                    this.content = content;
                    this.sendtimestamp = sendtimestamp;
                }
            }
        }
    }
}