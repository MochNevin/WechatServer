using MemoryPack;

namespace Erinn
{
    [MemoryPackable]
    public partial struct LoginRequest : INetworkMessage
    {
        public string Email;
        public string Userpassword;

        public LoginRequest(string email, string userpassword)
        {
            Email = email;
            Userpassword = userpassword;
        }
    }

    [MemoryPackable]
    public partial struct LoginResponse : INetworkMessage
    {
        public bool Success;
        public string Error;

        public LoginResponse(bool success, string error)
        {
            Success = success;
            Error = error;
        }
    }

    [MemoryPackable]
    public partial struct RegisterEmailcodeRequest : INetworkMessage
    {
        public string Email;

        public RegisterEmailcodeRequest(string email) => Email = email;
    }

    [MemoryPackable]
    public partial struct RegisterEmailcodeResponse : INetworkMessage
    {
        public bool Success;
        public string Error;

        public RegisterEmailcodeResponse(bool success, string error)
        {
            Success = success;
            Error = error;
        }
    }

    [MemoryPackable]
    public partial struct RegisterRequest : INetworkMessage
    {
        public string Email;
        public string Username;
        public string Userpassword;
        public string Emailcode;

        public RegisterRequest(string email, string username, string userpassword, string emailcode)
        {
            Email = email;
            Username = username;
            Userpassword = userpassword;
            Emailcode = emailcode;
        }
    }

    [MemoryPackable]
    public partial struct RegisterResponse : INetworkMessage
    {
        public bool Success;
        public string Error;

        public RegisterResponse(bool success, string error)
        {
            Success = success;
            Error = error;
        }
    }
}