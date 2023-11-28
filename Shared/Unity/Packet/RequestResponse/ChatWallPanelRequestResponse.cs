using MemoryPack;

namespace Erinn
{
    [MemoryPackable]
    public partial struct ChatWallRequest : INetworkMessage
    {
        public string Content;

        public ChatWallRequest(string content) => Content = content;
    }

    [MemoryPackable]
    public partial struct ChatWallResponse : INetworkMessage
    {
        public bool Success;
        public string Error;

        public ChatWallResponse(bool success, string error)
        {
            Success = success;
            Error = error;
        }
    }

    [MemoryPackable]
    public partial struct ChatWallMessage : INetworkMessage
    {
        public string Username;
        public string Content;

        public ChatWallMessage(string username, string content)
        {
            Username = username;
            Content = content;
        }
    }
}