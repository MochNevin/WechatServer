//------------------------------------------------------------
// Wechat
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

using System.Data;

#pragma warning disable CS8600
#pragma warning disable CS8603

namespace Erinn
{
    public static partial class MySqlite
    {
        public static class Friends
        {
            /// <summary>
            ///     将两个邮箱插入好友表
            /// </summary>
            /// <param name="email1">邮箱1</param>
            /// <param name="email2">邮箱2</param>
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
                catch (Exception)
                {
                    //
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            /// <summary>
            ///     更新两个邮箱之间的好友状态
            /// </summary>
            /// <param name="email1">邮箱1</param>
            /// <param name="email2">邮箱2</param>
            /// <param name="friendstatus">好友状态</param>
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
                catch (Exception)
                {
                    //
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            /// <summary>
            ///     删除两个邮箱之间的好友关系
            /// </summary>
            /// <param name="email1">邮箱1</param>
            /// <param name="email2">邮箱2</param>
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
                catch (Exception)
                {
                    //
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            /// <summary>
            ///     获取向指定邮箱发送好友请求的发送方邮箱列表
            /// </summary>
            /// <param name="email2">接收方邮箱</param>
            /// <returns>发送方邮箱列表</returns>
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
                catch (Exception)
                {
                    return default;
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            /// <summary>
            ///     获取向指定邮箱发送好友请求的接收方邮箱列表
            /// </summary>
            /// <param name="email1">发送方邮箱</param>
            /// <returns>接收方邮箱列表</returns>
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
                catch (Exception)
                {
                    return default;
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            /// <summary>
            ///     获取两个邮箱之间的好友状态
            /// </summary>
            /// <param name="email1">邮箱1</param>
            /// <param name="email2">邮箱2</param>
            /// <returns>好友状态</returns>
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
                catch (Exception)
                {
                    return default;
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            /// <summary>
            ///     检查两个邮箱之间是否为好友
            /// </summary>
            /// <param name="email1">邮箱1</param>
            /// <param name="email2">邮箱2</param>
            /// <returns>如果是好友，返回 true；否则返回 false</returns>
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
                catch (Exception)
                {
                    return default;
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            /// <summary>
            ///     获取指定邮箱的好友列表
            /// </summary>
            /// <param name="email">邮箱</param>
            /// <returns>好友邮箱列表</returns>
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
                catch (Exception)
                {
                    return default;
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }
        }
    }
}