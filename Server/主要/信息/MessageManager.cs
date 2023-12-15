//------------------------------------------------------------
// 偷我的代码就会被拖进黑暗空间
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

// ReSharper disable InconsistentNaming

#pragma warning disable CS0169
#pragma warning disable CS8601
#pragma warning disable CS8604
#pragma warning disable CS8618

namespace Erinn
{
    /// <summary>
    ///     信息管理器
    /// </summary>
    public sealed class MessageManager : ServerMessageManager
    {
        /// <summary>
        ///     邮箱映射
        /// </summary>
        private readonly Map<uint, string> EmailMap = new();

        /// <summary>
        ///     用户名映射
        /// </summary>
        private readonly Map<uint, string> UsernameMap = new();

        /// <summary>
        ///     构造
        /// </summary>
        public MessageManager(MasterServer master) : base(master) => master.OnClientDisconnected += OnClientDisconnected;

        /// <summary>
        ///     客户端断开
        /// </summary>
        /// <param name="id">客户端Id</param>
        private void OnClientDisconnected(uint id)
        {
            EmailMap.RemoveKey(id);
            UsernameMap.RemoveKey(id);
        }

        /// <summary>
        ///     处理用户登录请求的方法。
        /// </summary>
        /// <param name="id">用户ID。</param>
        /// <param name="request">登录请求对象。</param>
        [Rpc]
        private async void OnRequestLogin(uint id, RequestLogin request)
        {
            // 检查邮箱是否为空
            if (string.IsNullOrEmpty(request.email))
            {
                Response(false, "邮箱不能为空");
                return;
            }

            // 检查密码是否为空
            if (string.IsNullOrEmpty(request.userpassword))
            {
                Response(false, "密码不能为空");
                return;
            }

            // 检查邮箱是否存在
            var checkEmail = await MySqlite.Accounts.CheckEmail(request.email);
            if (!checkEmail)
            {
                Response(false, "不存在此邮箱");
                return;
            }

            // 检查密码是否正确
            var userpassword = await MySqlite.Accounts.GetUserpasswordByEmail(request.email);
            if (request.userpassword != userpassword)
            {
                Response(false, "密码错误");
                return;
            }

            // 获取用户名，将用户信息映射到ID
            var username = await MySqlite.Accounts.GetUsernameByEmail(request.email);
            EmailMap[id] = request.email;
            UsernameMap[id] = username;
            Response(true, "登录成功");
            return;

            void Response(bool success, string message) => Send(id, new ResponseLogin
            {
                Success = success,
                Message = message
            });
        }

        /// <summary>
        ///     处理用户注册请求的方法。
        /// </summary>
        /// <param name="id">用户ID。</param>
        /// <param name="request">注册请求对象。</param>
        [Rpc]
        private async void OnRequestRegister(uint id, RequestRegister request)
        {
            // 检查邮箱是否为空
            if (string.IsNullOrEmpty(request.email))
            {
                Response(false, "邮箱不能为空");
                return;
            }

            // 检查用户名是否为空
            if (string.IsNullOrEmpty(request.username))
            {
                Response(false, "用户名不能为空");
                return;
            }

            // 检查密码是否为空
            if (string.IsNullOrEmpty(request.userpassword))
            {
                Response(false, "密码不能为空");
                return;
            }

            // 检查验证码是否为空
            if (string.IsNullOrEmpty(request.emailcode))
            {
                Response(false, "验证码不能为空");
                return;
            }

            // 检查密码长度是否有效
            if (request.userpassword.Length < 8 || request.userpassword.Length > 15)
            {
                Response(false, "密码长度无效");
                return;
            }

            // 检查邮箱是否已存在
            var checkEmail = await MySqlite.Accounts.CheckEmail(request.email);
            if (checkEmail)
            {
                Response(false, "已经存在此邮箱");
                return;
            }

            // 检查用户名是否已存在
            var checkUsername = await MySqlite.Accounts.CheckUsername(request.username);
            if (checkUsername)
            {
                Response(false, "已经存在此用户名");
                return;
            }

            // 检查邮箱是否已验证
            var checkEmailcode = await MySqlite.Emailcodes.Check(request.email);
            if (!checkEmailcode)
            {
                Response(false, "此邮箱未验证");
                return;
            }

            // 检查验证码是否正确
            var emailcode = await MySqlite.Emailcodes.GetEmailCode(request.email);
            if (request.emailcode != emailcode)
            {
                Response(false, "验证码错误");
                return;
            }

            // 删除验证码，插入用户信息
            await MySqlite.Emailcodes.Delete(request.email);
            await MySqlite.Accounts.Insert(request.email, request.username, request.userpassword);
            Response(true, "注册成功");
            return;

            void Response(bool success, string message) => Send(id, new ResponseRegister
            {
                Success = success,
                Message = message
            });
        }

        /// <summary>
        ///     处理发送验证码请求的方法。
        /// </summary>
        /// <param name="id">用户ID。</param>
        /// <param name="request">发送验证码请求对象。</param>
        [Rpc]
        private async void OnRequestSendCode(uint id, RequestSendCode request)
        {
            // 检查是否已经存在此邮箱的验证码
            var checkEmailcode = await MySqlite.Emailcodes.Check(request.email);
            if (checkEmailcode)
                return;

            // 生成随机验证码
            var emailcode = "";
            for (var i = 0; i < 4; ++i)
                emailcode += MathV.Next(0, 10);

            // 将验证码插入数据库
            await MySqlite.Emailcodes.Insert(request.email, emailcode);
        }

        /// <summary>
        ///     处理聊天消息的方法。
        /// </summary>
        /// <param name="id">发送消息的用户ID。</param>
        /// <param name="message">聊天消息对象。</param>
        [Rpc]
        private async void OnMessageChat(uint id, MessageChat message)
        {
            // 如果发送消息的用户ID无法映射到用户名，则忽略此消息
            if (!UsernameMap.TryGetValue(id, out var username))
                return;

            // 如果消息中的用户名无法映射到目标用户ID，则忽略此消息
            if (!UsernameMap.TryGetKey(message.username, out var targetId))
                return;

            // 如果发送消息的用户ID和目标用户ID相同，则忽略此消息
            if (id == targetId)
                return;

            // 如果发送消息的用户或目标用户的邮箱映射不存在，则忽略此消息
            if (!EmailMap.TryGetValue(id, out var email1) || !EmailMap.TryGetValue(targetId, out var email2))
                return;

            // 检查两个用户是否为好友关系
            var isFriend = await MySqlite.Friends.Check(email1, email2);
            if (!isFriend)
                return;

            // 将消息转发给目标用户
            var targetMessage = message with { username = username };
            Send(targetId, targetMessage);
        }

        /// <summary>
        ///     处理获取好友列表请求的方法。
        /// </summary>
        /// <param name="id">用户ID。</param>
        /// <param name="request">获取好友列表请求对象。</param>
        [Rpc]
        private async void OnRequestGetFriends(uint id, RequestGetFriends request)
        {
            // 如果用户ID无法映射到邮箱，则忽略此请求
            if (!EmailMap.TryGetValue(id, out var email))
                return;

            // 获取用户的好友列表
            var friends = await MySqlite.Friends.GetFriends(email);

            // 获取好友的用户名列表
            var usernames = new List<string>();
            foreach (var friend in friends)
            {
                var username = await MySqlite.Accounts.GetUsernameByEmail(friend);
                if (!string.IsNullOrEmpty(username))
                    usernames.Add(username);
            }

            // 发送好友列表响应
            Send(id, new ResponseGetFriends { friends = usernames });
        }

        /// <summary>
        ///     处理添加好友请求的方法。
        /// </summary>
        /// <param name="id">用户ID。</param>
        /// <param name="request">添加好友请求对象。</param>
        [Rpc]
        private async void OnRequestFriend(uint id, RequestFriend request)
        {
            // 如果用户ID无法映射到邮箱，则忽略此请求
            if (!EmailMap.TryGetValue(id, out var email1))
                return;

            // 如果请求中的用户名无法映射到目标用户ID，则忽略此请求
            if (!UsernameMap.TryGetKey(request.username, out var targetId))
                return;

            // 如果发送请求的用户ID和目标用户ID相同，则忽略此请求
            if (id == targetId)
                return;

            // 如果目标用户的邮箱映射不存在，则忽略此请求
            if (!EmailMap.TryGetValue(targetId, out var email2))
                return;

            // 检查两个用户是否已经是好友
            var status = await MySqlite.Friends.GetFriendstatus(email1, email2);
            if (!string.IsNullOrEmpty(status))
                return;

            // 添加好友关系
            await MySqlite.Friends.Insert(email1, email2);
        }

        /// <summary>
        ///     处理获取待处理好友请求的方法。
        /// </summary>
        /// <param name="id">用户ID。</param>
        /// <param name="request">获取待处理好友请求对象。</param>
        [Rpc]
        private async void OnRequestGetPendingFriends(uint id, RequestGetPendingFriends request)
        {
            // 如果用户ID无法映射到邮箱，则忽略此请求
            if (!EmailMap.TryGetValue(id, out var email))
                return;

            // 获取发送请求的用户列表
            var friends = await MySqlite.Friends.GetSenders(email);

            // 获取用户列表的用户名
            var usernames = new List<string>();
            foreach (var friend in friends)
            {
                var username = await MySqlite.Accounts.GetUsernameByEmail(friend);
                if (!string.IsNullOrEmpty(username))
                    usernames.Add(username);
            }

            // 发送待处理好友请求列表响应
            Send(id, new ResponseGetPendingFriends { friends = usernames });
        }

        /// <summary>
        ///     处理更新好友关系请求的方法。
        /// </summary>
        /// <param name="id">用户ID。</param>
        /// <param name="message">更新好友关系消息对象。</param>
        [Rpc]
        private async void OnMessageUpdateFriend(uint id, MessageUpdateFriend message)
        {
            // 如果用户ID无法映射到邮箱，则忽略此消息
            if (!EmailMap.TryGetValue(id, out var email1))
                return;

            // 如果消息中的用户名无法映射到目标用户ID，则忽略此消息
            if (!UsernameMap.TryGetKey(message.username, out var targetId))
                return;

            // 如果发送消息的用户ID和目标用户ID相同，则忽略此消息
            if (id == targetId)
                return;

            // 如果目标用户的邮箱映射不存在，则忽略此消息
            if (!EmailMap.TryGetValue(targetId, out var email2))
                return;

            // 更新好友关系状态
            await MySqlite.Friends.Update(email1, email2, message.accepted);
        }
    }
}