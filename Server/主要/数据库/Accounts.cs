//------------------------------------------------------------
// 偷我的代码就会被拖进黑暗空间
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

using System.Data;

#pragma warning disable CS8600
#pragma warning disable CS8603

namespace Erinn
{
    public static partial class MySqlite
    {
        public static class Accounts
        {
            /// <summary>
            ///     将新账户插入数据库
            /// </summary>
            /// <param name="email">账户邮箱</param>
            /// <param name="username">账户用户名</param>
            /// <param name="userpassword">账户密码</param>
            public static async Task Insert(string email, string username, string userpassword)
            {
                const string query = "INSERT INTO accounts (email, username, userpassword) VALUES (@email, @username, @userpassword)";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@userpassword", userpassword);
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
            ///     检查邮箱是否存在
            /// </summary>
            /// <param name="email">账户邮箱</param>
            /// <returns>如果邮箱存在，返回 true；否则返回 false</returns>
            public static async Task<bool> CheckEmail(string email)
            {
                const string query = "SELECT email FROM accounts WHERE email = @email";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@email", email);
                    await cmd.ExecuteNonQueryAsync();
                    await using var reader = cmd.ExecuteReader();
                    return reader.HasRows;
                }
                catch (Exception)
                {
                    return true;
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            /// <summary>
            ///     检查用户名是否存在
            /// </summary>
            /// <param name="username">账户用户名</param>
            /// <returns>如果用户名存在，返回 true；否则返回 false</returns>
            public static async Task<bool> CheckUsername(string username)
            {
                const string query = "SELECT username FROM accounts WHERE username = @username";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@username", username);
                    await cmd.ExecuteNonQueryAsync();
                    await using var reader = cmd.ExecuteReader();
                    return reader.HasRows;
                }
                catch (Exception)
                {
                    return true;
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            /// <summary>
            ///     根据邮箱获取用户密码
            /// </summary>
            /// <param name="email">账户邮箱</param>
            /// <returns>如果邮箱存在，返回对应用户密码；否则返回 null</returns>
            public static async Task<string> GetUserpasswordByEmail(string email)
            {
                const string query = "SELECT userpassword FROM accounts WHERE email = @email";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@email", email);
                    await cmd.ExecuteNonQueryAsync();
                    string userpassword = null;
                    await using var reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        userpassword = reader.GetString("userpassword");
                    }

                    return userpassword;
                }
                catch (Exception)
                {
                    return null;
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            /// <summary>
            ///     根据邮箱获取用户名
            /// </summary>
            /// <param name="email">账户邮箱</param>
            /// <returns>如果邮箱存在，返回对应用户名；否则返回 null</returns>
            public static async Task<string> GetUsernameByEmail(string email)
            {
                const string query = "SELECT username FROM accounts WHERE email = @email";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@email", email);
                    await cmd.ExecuteNonQueryAsync();
                    string username = null;
                    await using var reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        username = reader.GetString("username");
                    }

                    return username;
                }
                catch (Exception)
                {
                    return null;
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            /// <summary>
            ///     根据用户名获取邮箱
            /// </summary>
            /// <param name="username">账户用户名</param>
            /// <returns>如果用户名存在，返回对应邮箱；否则返回 null</returns>
            public static async Task<string> GetEmailByUsername(string username)
            {
                const string query = "SELECT email FROM accounts WHERE username = @username";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@username", username);
                    await cmd.ExecuteNonQueryAsync();
                    string email = null;
                    await using var reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        email = reader.GetString("email");
                    }

                    return email;
                }
                catch (Exception)
                {
                    return null;
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