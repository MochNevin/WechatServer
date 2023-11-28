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
        ///     SmtpClient池
        /// </summary>
        private static readonly Stack<SmtpClient> SmtpClientPool = new();

        /// <summary>
        ///     服务设置
        /// </summary>
        public static MailServiceSetting ServerSetting { get; private set; }

        /// <summary>
        ///     连接
        /// </summary>
        /// <param name="setting">设置</param>
        public static void Connect(MailServiceSetting setting) => ServerSetting = setting;

        /// <summary>
        ///     连接
        /// </summary>
        /// <param name="server">服务器</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="displayName">显示名称</param>
        public static void Connect(string server, string userName, string password, string displayName) => ServerSetting = new MailServiceSetting(server, userName, password, displayName);

        /// <summary>
        ///     发送邮件
        /// </summary>
        /// <param name="message">信息</param>
        public static async Task SendMailAsync(MailMessage message) => await GetSmtpClient().SendMailAsync(message);

        /// <summary>
        ///     发送信息
        /// </summary>
        /// <param name="subject">标题</param>
        /// <param name="body">中文</param>
        /// <param name="email">邮箱</param>
        public static async Task SendMail(string subject, string body, string email)
        {
            if (!MailAddress.TryCreate(email, out var address))
                return;
            using var message = GetMessage(subject, body);
            message.To.Add(address);
            await SendMailAsync(message);
        }

        /// <summary>
        ///     发送信息
        /// </summary>
        /// <param name="subject">标题</param>
        /// <param name="body">中文</param>
        /// <param name="address">邮箱地址</param>
        public static async Task SendMail(string subject, string body, MailAddress address)
        {
            using var message = GetMessage(subject, body);
            message.To.Add(address);
            await SendMailAsync(message);
        }

        /// <summary>
        ///     发送信息
        /// </summary>
        /// <param name="subject">标题</param>
        /// <param name="body">中文</param>
        /// <param name="priority">优先级</param>
        /// <param name="email">邮箱</param>
        public static async Task SendMail(string subject, string body, MailPriority priority, string email)
        {
            if (!MailAddress.TryCreate(email, out var address))
                return;
            using var message = GetMessage(subject, body, priority);
            message.To.Add(address);
            await SendMailAsync(message);
        }

        /// <summary>
        ///     发送信息
        /// </summary>
        /// <param name="subject">标题</param>
        /// <param name="body">中文</param>
        /// <param name="emails">邮箱</param>
        public static async Task SendMails(string subject, string body, params string[] emails)
        {
            if (emails == null || emails.Length == 0)
                return;
            var hashSet = new HashSet<string>();
            foreach (var email in emails)
            {
                if (!string.IsNullOrEmpty(email))
                    hashSet.Add(email);
            }

            if (hashSet.Count == 0)
                return;
            using var message = GetMessage(subject, body);
            foreach (var email in hashSet)
            {
                if (!MailAddress.TryCreate(email, out var address))
                    continue;
                message.To.Add(address);
            }

            await SendMailAsync(message);
        }

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
            {
                if (!string.IsNullOrEmpty(email))
                    hashSet.Add(email);
            }

            if (hashSet.Count == 0)
                return;
            using var message = GetMessage(subject, body, priority);
            foreach (var email in hashSet)
            {
                if (!MailAddress.TryCreate(email, out var address))
                    continue;
                message.To.Add(address);
            }

            await SendMailAsync(message);
        }

        /// <summary>
        ///     清空
        /// </summary>
        public static void Clear()
        {
            foreach (var smtpClient in SmtpClientPool)
                smtpClient.Dispose();
            SmtpClientPool.Clear();
        }

        /// <summary>
        ///     获取SmtpClient
        /// </summary>
        /// <returns>获得的SmtpClient</returns>
        private static SmtpClient GetSmtpClient()
        {
            if (SmtpClientPool.Count > 0)
                return SmtpClientPool.Pop();
            var smtpClient = ServerSetting.GetSmtpClient();
            smtpClient.SendCompleted += (_, _) => SmtpClientPool.Push(smtpClient);
            return smtpClient;
        }

        /// <summary>
        ///     获取邮件信息
        /// </summary>
        /// <param name="subject">标题</param>
        /// <param name="body">正文</param>
        /// <returns>邮件信息</returns>
        private static MailMessage GetMessage(string subject, string body)
        {
            var message = new MailMessage();
            message.From = ServerSetting.From;
            message.Priority = MailPriority.High;
            message.SubjectEncoding = Encoding.UTF8;
            message.BodyEncoding = Encoding.UTF8;
            message.Subject = subject;
            message.Body = body;
            return message;
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
            message.From = ServerSetting.From;
            message.Priority = priority;
            message.SubjectEncoding = Encoding.UTF8;
            message.BodyEncoding = Encoding.UTF8;
            message.Subject = subject;
            message.Body = body;
            return message;
        }
    }
}