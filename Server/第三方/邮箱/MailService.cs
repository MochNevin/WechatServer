//------------------------------------------------------------
// 偷我的代码就会被拖进黑暗空间
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

using System.Net;
using System.Net.Mail;
using System.Text;

namespace Erinn
{
    /// <summary>
    ///     邮件服务
    /// </summary>
    public static class MailService
    {
        /// <summary>
        ///     服务器设置
        /// </summary>
        private static MailServiceSetting _serverSetting;

        /// <summary>
        ///     SmtpClient池
        /// </summary>
        private static readonly Stack<SmtpClient> SmtpClientPool = new();

        /// <summary>
        ///     获取SmtpClient
        /// </summary>
        /// <returns>获得的SmtpClient</returns>
        private static SmtpClient GetSmtpClient()
        {
            if (SmtpClientPool.Count > 0)
                return SmtpClientPool.Pop();
            var smtpClient = _serverSetting.GetNewSmtpClient();
            smtpClient.SendCompleted += (_, _) => SmtpClientPool.Push(smtpClient);
            return smtpClient;
        }

        /// <summary>
        ///     获取邮件信息
        /// </summary>
        /// <param name="subject">标题</param>
        /// <param name="body">正文</param>
        /// <param name="priority">优先级</param>
        /// <returns>邮件信息</returns>
        private static MailMessage GetMessage(string subject, string body, MailPriority priority)
        {
            var message = new MailMessage();
            message.From = _serverSetting.From;
            message.Priority = priority;
            message.SubjectEncoding = Encoding.UTF8;
            message.BodyEncoding = Encoding.UTF8;
            message.Subject = subject;
            message.Body = body;
            return message;
        }

        /// <summary>
        ///     连接
        /// </summary>
        /// <param name="server">服务器</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="displayName">显示名称</param>
        public static void Connect(string server, string userName, string password, string displayName) => _serverSetting = new MailServiceSetting(server, userName, password, displayName);

        /// <summary>
        ///     发送邮件
        /// </summary>
        /// <param name="message">信息</param>
        public static Task SendMail(MailMessage message) => GetSmtpClient().SendMailAsync(message);

        /// <summary>
        ///     发送信息
        /// </summary>
        /// <param name="subject">标题</param>
        /// <param name="body">中文</param>
        /// <param name="email">邮箱</param>
        public static async Task SendMail(string subject, string body, string email) => await SendMail(subject, body, MailPriority.Normal, email);

        /// <summary>
        ///     发送信息
        /// </summary>
        /// <param name="subject">标题</param>
        /// <param name="body">中文</param>
        /// <param name="priority">优先级</param>
        /// <param name="email">邮箱</param>
        public static async Task SendMail(string subject, string body, MailPriority priority, string email)
        {
            if (string.IsNullOrEmpty(email))
                return;
            try
            {
                if (MailAddress.TryCreate(email, out var mailAddress))
                    await SendMail(subject, body, priority, mailAddress);
            }
            catch (Exception e)
            {
                Log.Info(e);
            }
        }

        /// <summary>
        ///     发送信息
        /// </summary>
        /// <param name="subject">标题</param>
        /// <param name="body">中文</param>
        /// <param name="mailAddress">邮箱</param>
        public static async Task SendMail(string subject, string body, MailAddress mailAddress) => await SendMail(subject, body, MailPriority.Normal, mailAddress);

        /// <summary>
        ///     发送信息
        /// </summary>
        /// <param name="subject">标题</param>
        /// <param name="body">中文</param>
        /// <param name="priority">优先级</param>
        /// <param name="mailAddress">邮箱</param>
        public static async Task SendMail(string subject, string body, MailPriority priority, MailAddress mailAddress)
        {
            try
            {
                using var message = GetMessage(subject, body, priority);
                message.To.Add(mailAddress);
                await SendMail(message);
            }
            catch (Exception e)
            {
                Log.Info(e);
            }
        }

        /// <summary>
        ///     发送信息
        /// </summary>
        /// <param name="subject">标题</param>
        /// <param name="body">中文</param>
        /// <param name="emails">邮箱</param>
        public static async Task SendMails(string subject, string body, params string[] emails) => await SendMails(subject, body, MailPriority.Normal, emails);

        /// <summary>
        ///     发送信息
        /// </summary>
        /// <param name="subject">标题</param>
        /// <param name="body">中文</param>
        /// <param name="priority">优先级</param>
        /// <param name="emails">邮箱</param>
        public static async Task SendMails(string subject, string body, MailPriority priority, params string[] emails)
        {
            if (emails == null || emails.Length == 0)
                return;
            var hashSet = new HashSet<string>();
            foreach (var email in emails)
                if (!string.IsNullOrEmpty(email))
                    hashSet.Add(email);
            if (hashSet.Count == 0)
                return;
            var mailAddresses = new List<MailAddress>();
            foreach (var email in hashSet)
            {
                try
                {
                    if (MailAddress.TryCreate(email, out var mailAddress))
                        mailAddresses.Add(mailAddress);
                }
                catch (Exception e)
                {
                    Log.Info(e);
                }
            }

            if (mailAddresses.Count == 0)
                return;
            await SendMails(subject, body, priority, mailAddresses.ToArray());
        }

        /// <summary>
        ///     发送信息
        /// </summary>
        /// <param name="subject">标题</param>
        /// <param name="body">中文</param>
        /// <param name="mailAddresses">邮箱</param>
        public static async Task SendMails(string subject, string body, params MailAddress[] mailAddresses) => await SendMails(subject, body, MailPriority.Normal, mailAddresses);

        /// <summary>
        ///     发送信息
        /// </summary>
        /// <param name="subject">标题</param>
        /// <param name="body">中文</param>
        /// <param name="priority">优先级</param>
        /// <param name="mailAddresses">邮箱</param>
        public static async Task SendMails(string subject, string body, MailPriority priority, params MailAddress[] mailAddresses)
        {
            if (mailAddresses == null || mailAddresses.Length == 0)
                return;
            var hashSet = new HashSet<MailAddress>();
            foreach (var mailAddress in mailAddresses)
                if (mailAddress != null)
                    hashSet.Add(mailAddress);
            if (hashSet.Count == 0)
                return;
            try
            {
                using var message = GetMessage(subject, body, priority);
                foreach (var mailAddress in hashSet)
                    message.To.Add(mailAddress);
                await SendMail(message);
            }
            catch (Exception e)
            {
                Log.Info(e);
            }
        }

        /// <summary>
        ///     邮件服务设置
        /// </summary>
        private readonly struct MailServiceSetting
        {
            /// <summary>
            ///     服务器
            /// </summary>
            private readonly string _server;

            /// <summary>
            ///     地址
            /// </summary>
            private readonly string _address;

            /// <summary>
            ///     密码
            /// </summary>
            private readonly string _password;

            /// <summary>
            ///     发送者
            /// </summary>
            public readonly MailAddress From;

            /// <summary>
            ///     构造
            /// </summary>
            /// <param name="server">服务器</param>
            /// <param name="address">地址</param>
            /// <param name="password">密码</param>
            /// <param name="displayName">显示名称</param>
            public MailServiceSetting(string server, string address, string password, string displayName)
            {
                _server = server;
                _address = address;
                _password = password;
                From = new MailAddress(address, displayName, Encoding.UTF8);
            }

            /// <summary>
            ///     获取SmtpClient
            /// </summary>
            /// <returns>获得的SmtpClient</returns>
            public SmtpClient GetNewSmtpClient() => new(_server)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_address, _password),
                Port = 587
            };
        }
    }
}