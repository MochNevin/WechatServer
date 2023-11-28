using System.Data;
using System.Net.Mail;

#pragma warning disable CS8600
#pragma warning disable CS8603

namespace Erinn
{
    public static partial class WechatMySql
    {
        public static class Emailcodes
        {
            private static readonly Dictionary<MailAddress, CancellationTokenSource> _emails = new();

            public static async Task Insert(MailAddress address, string email, string emailcode)
            {
                var cancelTokenSource = new CancellationTokenSource();
                _emails[address] = cancelTokenSource;
                _ = DelayDelete(address, email, cancelTokenSource.Token);
                await MailService.SendMail("原神注册", $"[原神]，验证码[{emailcode}]，3分钟内有效。", address);
                const string query = "INSERT INTO emailcodes (email, emailcode) VALUES (@email, @emailcode)";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@emailcode", emailcode);
                    await cmd.ExecuteNonQueryAsync();
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            private static async Task DelayDelete(MailAddress address, string email, CancellationToken cancellationToken)
            {
                await Task.Delay(TimeSpan.FromMinutes(3), cancellationToken);
                const string query = "DELETE FROM emailcodes WHERE email = @email";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@email", email);
                    await cmd.ExecuteNonQueryAsync(cancellationToken);
                    _emails.Remove(address);
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            public static async Task Delete(MailAddress address, string email)
            {
                if (_emails.TryGetValue(address, out var cancellationTokenSource))
                {
                    cancellationTokenSource.Cancel();
                    _emails.Remove(address);
                }

                const string query = "DELETE FROM emailcodes WHERE email = @email";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@email", email);
                    await cmd.ExecuteNonQueryAsync();
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

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
                catch
                {
                    return true;
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

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
                catch
                {
                    return null;
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            public static async Task Truncate()
            {
                const string query = "TRUNCATE TABLE emailcodes";
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

            public struct Emailcode
            {
                public string email;
                public string emailcode;

                public Emailcode(string email, string emailcode)
                {
                    this.email = email;
                    this.emailcode = emailcode;
                }
            }
        }
    }
}