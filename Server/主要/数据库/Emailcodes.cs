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
        public static class Emailcodes
        {
            /// <summary>
            ///     存储邮箱和取消令牌的字典
            /// </summary>
            private static readonly Dictionary<string, CancellationTokenSource> Emails = new();

            /// <summary>
            ///     将邮箱和验证码插入数据库，同时启动延迟删除任务
            /// </summary>
            /// <param name="email">邮箱</param>
            /// <param name="emailcode">验证码</param>
            public static async Task Insert(string email, string emailcode)
            {
                var cancelTokenSource = new CancellationTokenSource();
                Emails[email] = cancelTokenSource;
                DelayDelete(email, cancelTokenSource.Token).Coroutine();
                await MailService.SendMail("原神注册", $"[原神]，验证码[{emailcode}]，3分钟内有效", email);
                const string query = "INSERT INTO emailcodes (email, emailcode) VALUES (@email, @emailcode)";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@emailcode", emailcode);
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
            ///     延迟删除任务
            /// </summary>
            private static async Task DelayDelete(string email, CancellationToken cancellationToken)
            {
                await Task.Delay(TimeSpan.FromMinutes(3.0), cancellationToken);
                const string query = "DELETE FROM emailcodes WHERE email = @email";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@email", email);
                    await cmd.ExecuteNonQueryAsync(cancellationToken);
                    Emails.Remove(email);
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
            ///     根据邮箱删除邮箱验证码，并取消延迟删除任务
            /// </summary>
            /// <param name="email">邮箱</param>
            public static async Task Delete(string email)
            {
                if (Emails.TryGetValue(email, out var cancellationTokenSource))
                {
                    cancellationTokenSource.Cancel();
                    Emails.Remove(email);
                }

                const string query = "DELETE FROM emailcodes WHERE email = @email";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@email", email);
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
            ///     检查邮箱是否存在对应的验证码
            /// </summary>
            /// <param name="email">邮箱</param>
            /// <returns>如果存在对应的验证码，返回 true；否则返回 false</returns>
            public static async Task<bool> Check(string email)
            {
                const string query = "SELECT email FROM emailcodes WHERE email = @email";
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
            ///     根据邮箱获取邮箱验证码
            /// </summary>
            /// <param name="email">邮箱</param>
            /// <returns>如果存在对应的验证码，返回验证码；否则返回 null</returns>
            public static async Task<string> GetEmailCode(string email)
            {
                const string query = "SELECT emailcode FROM emailcodes WHERE email = @email";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@email", email);
                    await cmd.ExecuteNonQueryAsync();
                    string emailcode = null;
                    await using var reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        emailcode = reader.GetString("emailcode");
                    }

                    return emailcode;
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