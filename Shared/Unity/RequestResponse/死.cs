//------------------------------------------------------------
// 偷我的代码就会被拖进黑暗空间
// Copyright © 2023 Molth Nevin. All rights reserved.
//------------------------------------------------------------

using System.Collections.Generic;
using MemoryPack;

namespace Erinn
{
    [MemoryPackable]
    public partial struct RequestLogin : INetworkMessage
    {
        public string email;
        public string userpassword;
    }

    [MemoryPackable]
    public partial struct ResponseLogin : INetworkMessage
    {
        public bool Success;
        public string Message;
    }

    [MemoryPackable]
    public partial struct RequestRegister : INetworkMessage
    {
        public string email;
        public string username;
        public string userpassword;
        public string emailcode;
    }

    [MemoryPackable]
    public partial struct ResponseRegister : INetworkMessage
    {
        public bool Success;
        public string Message;
    }

    [MemoryPackable]
    public partial struct RequestSendCode : INetworkMessage
    {
        public string email;
    }

    [MemoryPackable]
    public partial struct MessageChat : INetworkMessage
    {
        public string username;
        public string content;
    }

    [MemoryPackable]
    public partial struct RequestGetFriends : INetworkMessage
    {
    }

    [MemoryPackable]
    public partial struct ResponseGetFriends : INetworkMessage
    {
        public List<string> friends;
    }

    [MemoryPackable]
    public partial struct RequestFriend : INetworkMessage
    {
        public string username;
    }

    [MemoryPackable]
    public partial struct RequestGetPendingFriends : INetworkMessage
    {
    }

    [MemoryPackable]
    public partial struct ResponseGetPendingFriends : INetworkMessage
    {
        public List<string> friends;
    }

    [MemoryPackable]
    public partial struct MessageUpdateFriend : INetworkMessage
    {
        public string username;
        public bool accepted;
    }
}