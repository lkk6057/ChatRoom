using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chatroom.Modules
{
    [Serializable]
    public class ChatCache
    {
        public List<string> cache = new List<string>();
        public ChatCache(List<string> cache)
        {
            this.cache = cache;
        }
    }
}