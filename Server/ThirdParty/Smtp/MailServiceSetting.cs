using System.Net;
using System.Net.Mail;
using System.Text;

namespace Erinn
{
    /// <summary>
    ///     邮件服务设置
    /// </summary>
    public readonly struct MailServiceSetting
    {
        /// <summary>
        ///     服务器
        /// </summary>
        public readonly string Server;

        /// <summary>
        ///     地址
        /// </summary>
        public readonly string Address;

        /// <summary>
        ///     密码
        /// </summary>
        public readonly string Password;

        /// <summary>
        ///     发送者
        /// </summary>
        public readonly MailAddress From;

        /// <summary>
        ///     String
        /// </summary>
        public override string ToString() => $"[{Server}] [{Address}] [{Password}] [{From.DisplayName}]";

        /// <summary>
        ///     构造
        /// </summary>
        /// <param name="server">服务器</param>
        /// <param name="address">地址</param>
        /// <param name="password">密码</param>
        /// <param name="displayName">显示名称</param>
        public MailServiceSetting(string server, string address, string password, string displayName)
        {
            Server = server;
            Address = address;
            Password = password;
            From = new MailAddress(address, displayName, Encoding.UTF8);
        }

        /// <summary>
        ///     获取SmtpClient
        /// </summary>
        /// <returns>获得的SmtpClient</returns>
        public SmtpClient GetSmtpClient() => new(Server)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(Address, Password)
        };
    }
}