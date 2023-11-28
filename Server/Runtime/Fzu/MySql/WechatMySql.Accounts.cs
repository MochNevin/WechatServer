using System.Data;

#pragma warning disable CS8600
#pragma warning disable CS8603

namespace Erinn
{
    public static partial class WechatMySql
    {
        public static class Accounts
        {
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
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            public static async Task UpdateLoginByEmail(string email, bool login)
            {
                const string query = "UPDATE accounts SET login = @login WHERE email = @email";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@login", login);
                    await cmd.ExecuteNonQueryAsync();
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            public static async Task UpdateUsernameByEmail(string email, string username)
            {
                const string query = "UPDATE accounts SET username = @username WHERE email = @email";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@username", username);
                    await cmd.ExecuteNonQueryAsync();
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            public static async Task UpdateUserpasswordByEmail(string email, string userpassword)
            {
                const string query = "UPDATE accounts SET userpassword = @userpassword WHERE email = @email";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@userpassword", userpassword);
                    await cmd.ExecuteNonQueryAsync();
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            public static async Task DeleteByEmail(string email)
            {
                const string query = "DELETE FROM accounts WHERE email = @email";
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

            public static async Task<string> GetUserpasswordByEmail(string email)
            {
                const string query = "SELECT userpassword FROM accounts WHERE email = @email";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@email", email);
                    await cmd.ExecuteNonQueryAsync();
                    string password = null;
                    await using var reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        password = reader.GetString("userpassword");
                    }

                    return password;
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

            public static async Task<string> GetUsernameByEmail(string email)
            {
                const string query = "SELECT username FROM accounts WHERE email = @email";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@email", email);
                    await cmd.ExecuteNonQueryAsync();
                    string password = null;
                    await using var reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        password = reader.GetString("username");
                    }

                    return password;
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

            public static async Task<string> GetEmailByUsername(string username)
            {
                const string query = "SELECT email FROM accounts WHERE username = @username";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@username", username);
                    await cmd.ExecuteNonQueryAsync();
                    string password = null;
                    await using var reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        password = reader.GetString("email");
                    }

                    return password;
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

            public static async Task<string> GetEmailById(int id)
            {
                const string query = "SELECT email FROM accounts WHERE id = @id";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@id", id);
                    await cmd.ExecuteNonQueryAsync();
                    string password = null;
                    await using var reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        password = reader.GetString("email");
                    }

                    return password;
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

            public static async Task<int> GetIdByEmail(string email)
            {
                const string query = "SELECT email FROM accounts WHERE email = @email";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@email", email);
                    await cmd.ExecuteNonQueryAsync();
                    var id = -1;
                    await using var reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        id = reader.GetInt32("id");
                    }

                    return id;
                }
                catch
                {
                    return -1;
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            public static async Task<Account> GetAccountByEmail(string email)
            {
                const string query = "SELECT * FROM accounts WHERE email = @email";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@email", email);
                    await cmd.ExecuteNonQueryAsync();
                    Account account = default;
                    await using var reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        account = new Account(reader.GetInt32("id"), email, reader.GetString("username"), reader.GetString("userpassword"), reader.GetBoolean("login"));
                    }

                    return account;
                }
                catch
                {
                    return default;
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            public static async Task<Account> GetAccountByUsername(string username)
            {
                const string query = "SELECT * FROM accounts WHERE username = @username";
                var connection = await MySqlService.Pop();
                try
                {
                    await using var cmd = MySqlService.SendQuery(query, connection);
                    cmd.Parameters.AddWithValue("@username", username);
                    await cmd.ExecuteNonQueryAsync();
                    Account account = default;
                    await using var reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        account = new Account(reader.GetInt32("id"), reader.GetString("email"), username, reader.GetString("userpassword"), reader.GetBoolean("login"));
                    }

                    return account;
                }
                catch
                {
                    return default;
                }
                finally
                {
                    await connection.CloseAsync();
                    MySqlService.Push(connection);
                }
            }

            public static async Task Truncate()
            {
                const string query = "TRUNCATE TABLE accounts";
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

            public struct Account
            {
                public int id;
                public string email;
                public string username;
                public string userpassword;
                public bool login;

                public Account(int id, string email, string username, string userpassword, bool login)
                {
                    this.id = id;
                    this.email = email;
                    this.username = username;
                    this.userpassword = userpassword;
                    this.login = login;
                }
            }
        }
    }
}