using System.Data;

#pragma warning disable CS8600
#pragma warning disable CS8603

namespace Erinn
{
    public static partial class WechatMySql
    {
        public static class Friends
        {
            public static async Task Insert(string email1, string email2)
            {
                const string query = "INSERT INTO friends (email1, email2) VALUES (@email1, @email2)";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@email1", email1);
                    cmd.Parameters.AddWithValue("@email2", email2);
                    await cmd.ExecuteNonQueryAsync();
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            public static async Task Update(string email1, string email2, bool friendstatus)
            {
                const string query = "UPDATE friends SET friendstatus = @friendstatus WHERE (email1 = @email1 AND email2 = @email2) OR (email1 = @email2 AND email2 = @email1)";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@email1", email1);
                    cmd.Parameters.AddWithValue("@email2", email2);
                    cmd.Parameters.AddWithValue("@friendstatus", friendstatus ? "accepted" : "rejected");
                    await cmd.ExecuteNonQueryAsync();
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            public static async Task Delete(string email1, string email2)
            {
                const string query = "DELETE FROM friends WHERE (email1 = @email1 AND email2 = @email2) OR (email1 = @email2 AND email2 = @email1)";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@email1", email1);
                    cmd.Parameters.AddWithValue("@email2", email2);
                    await cmd.ExecuteNonQueryAsync();
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            public static async Task<List<string>> GetSenders(string email2)
            {
                const string query = "SELECT email1 FROM friends WHERE email2 = @email2 AND friendstatus = 'pending'";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@email2", email2);
                    var pendingemails = new List<string>();
                    await using var reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                        while (await reader.ReadAsync())
                            pendingemails.Add(reader.GetString("email1"));
                    return pendingemails;
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            public static async Task<List<string>> GetReceivers(string email1)
            {
                const string query = "SELECT email2 FROM friends WHERE email1 = @email1 AND friendstatus = 'pending'";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@email1", email1);
                    var pendingemails = new List<string>();
                    await using var reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                        while (await reader.ReadAsync())
                            pendingemails.Add(reader.GetString("email2"));
                    return pendingemails;
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            public static async Task<string> GetFriendstatus(string email1, string email2)
            {
                const string query = "SELECT friendstatus FROM friends WHERE (email1 = @email1 AND email2 = @email2) OR (email1 = @email2 AND email2 = @email1)";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@email1", email1);
                    cmd.Parameters.AddWithValue("@email2", email2);
                    await using var reader = await cmd.ExecuteReaderAsync();
                    string friendstatus = null;
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        friendstatus = reader.GetString("friendstatus");
                    }

                    return friendstatus;
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            public static async Task<bool> Check(string email1, string email2)
            {
                const string query = "SELECT friendstatus FROM friends WHERE (email1 = @email1 AND email2 = @email2) OR (email1 = @email2 AND email2 = @email1)";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@email1", email1);
                    cmd.Parameters.AddWithValue("@email2", email2);
                    await using var reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        var friendstatus = reader.GetString("friendstatus");
                        return friendstatus.Equals("accepted");
                    }

                    return false;
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            public static async Task<List<string>> GetFriends(string email)
            {
                const string query = "SELECT IF(email1 = @email, email2, email1) AS friend FROM friends WHERE (email1 = @email OR email2 = @email) AND friendstatus = 'accepted'";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@email", email);
                    var friends = new List<string>();
                    await using var reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                        while (await reader.ReadAsync())
                            friends.Add(reader.GetString("friend"));
                    return friends;
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            public static async Task Truncate()
            {
                const string query = "TRUNCATE TABLE friends";
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

            public struct Friend
            {
                public int id;
                public string email1;
                public string email2;
                public string friendstatus;

                public Friend(int id, string email1, string email2, string friendstatus)
                {
                    this.id = id;
                    this.email1 = email1;
                    this.email2 = email2;
                    this.friendstatus = friendstatus;
                }
            }
        }
    }
}