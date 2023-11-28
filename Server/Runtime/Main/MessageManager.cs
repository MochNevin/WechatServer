using System.Net.Mail;

namespace Erinn
{
    public sealed class MessageManager : ServerMessageManager
    {
        private readonly HashSet<uint> ClientIds = new();

        private readonly Map<uint, string> ClientMap = new();

        public MessageManager(MasterServer master) : base(master)
        {
            Master.OnClientConnected += id => { ClientIds.Add(id); };
            Master.OnClientDisconnected += id =>
            {
                ClientIds.Remove(id);
                ClientMap.RemoveKey(id);
            };
        }

        [Rpc]
        private async void OnLoginRequest(uint id, LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                SendLoginResponse(false, "邮箱不能为空");
                return;
            }

            if (string.IsNullOrEmpty(request.Userpassword))
            {
                SendLoginResponse(false, "密码不能为空");
                return;
            }

            var existsEmail = await WechatMySql.Accounts.CheckEmail(request.Email);
            if (!existsEmail)
            {
                SendLoginResponse(false, "邮箱不存在");
                return;
            }

            var userpassword = await WechatMySql.Accounts.GetUserpasswordByEmail(request.Email);
            if (request.Userpassword != userpassword)
            {
                SendLoginResponse(false, "密码错误");
                return;
            }

            if (ClientMap.TryGetKey(request.Email, out var clientId))
                Master.Disconnect(clientId);
            ClientMap.Add(id, request.Email);
            await WechatMySql.Accounts.UpdateLoginByEmail(request.Email, true);
            SendLoginResponse(true, "登录成功");
            return;
            void SendLoginResponse(bool success, string error) => Send(id, new LoginResponse(success, error));
        }

        [Rpc]
        private async void OnRegisterEmailcodeRequest(uint id, RegisterEmailcodeRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                SendRegisterEmailcodeResponse(false, "邮箱不能为空");
                return;
            }

            if (!MailAddress.TryCreate(request.Email, out var address))
            {
                SendRegisterEmailcodeResponse(false, "邮箱格式错误");
                return;
            }

            var exists = await WechatMySql.Emailcodes.Check(request.Email);
            if (exists)
            {
                SendRegisterEmailcodeResponse(false, "禁止频繁请求验证码");
                return;
            }

            var emailcode = "";
            for (var i = 0; i < 4; ++i)
                emailcode += MathV.Next(0, 10);
            await WechatMySql.Emailcodes.Insert(address, request.Email, emailcode);
            SendRegisterEmailcodeResponse(true, "成功请求验证码");
            return;
            void SendRegisterEmailcodeResponse(bool success, string error) => Send(id, new RegisterEmailcodeResponse(success, error));
        }

        [Rpc]
        private async void OnRegisterRequest(uint id, RegisterRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                SendRegisterResponse(false, "邮箱不能为空");
                return;
            }

            if (string.IsNullOrEmpty(request.Username))
            {
                SendRegisterResponse(false, "用户名不能为空");
                return;
            }

            if (string.IsNullOrEmpty(request.Userpassword))
            {
                SendRegisterResponse(false, "密码不能为空");
                return;
            }

            if (string.IsNullOrEmpty(request.Emailcode))
            {
                SendRegisterResponse(false, "验证码不能为空");
                return;
            }

            if (!MailAddress.TryCreate(request.Email, out var address))
            {
                SendRegisterResponse(false, "邮箱格式错误");
                return;
            }

            var existsEmail = await WechatMySql.Accounts.CheckEmail(request.Email);
            if (existsEmail)
            {
                SendRegisterResponse(false, "邮箱已经存在");
                return;
            }

            var existsUsername = await WechatMySql.Accounts.CheckUsername(request.Username);
            if (existsUsername)
            {
                SendRegisterResponse(false, "用户名已经存在");
                return;
            }

            if (request.Userpassword.Length is < 8 or > 15)
            {
                SendRegisterResponse(false, "密码长度应为8到15位");
                return;
            }

            var emailCode = await WechatMySql.Emailcodes.GetEmailCode(request.Email);
            if (request.Emailcode != emailCode)
            {
                SendRegisterResponse(false, "验证码错误");
                return;
            }

            await WechatMySql.Emailcodes.Delete(address, request.Email);
            await WechatMySql.Accounts.Insert(request.Email, request.Username, request.Userpassword);
            SendRegisterResponse(true, "注册成功");
            return;
            void SendRegisterResponse(bool success, string error) => Send(id, new RegisterResponse(success, error));
        }

        [Rpc]
        private async void OnChatWallRequest(uint id, ChatWallRequest request)
        {
            var data = await BaiduService.DetectSensitiveWords(request.Content, 1);
            if (!data.ResponseData.isPass)
            {
                var str = "";
                for (var i = 0; i < data.ResponseData.data.words.Length; ++i)
                {
                    var word = data.ResponseData.data.words[i];
                    if (i != data.ResponseData.data.words.Length - 1)
                        str += word + ", ";
                    else
                        str += word;
                }

                Send(id, new ChatWallResponse(false, $"含有敏感词: {str}"));
            }
            else
            {
                if (ClientMap.TryGetValue(id, out var email))
                {
                    var username = await WechatMySql.Accounts.GetUsernameByEmail(email);
                    if (string.IsNullOrEmpty(username))
                        return;
                    foreach (var clientId in ClientIds)
                        Send(clientId, new ChatWallMessage(username, request.Content));
                }
            }
        }
    }
}