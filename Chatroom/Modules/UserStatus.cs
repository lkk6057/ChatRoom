using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chatroom.Modules
{
    public class UserStatus
    {
        public string username;
        public bool pinged;
        public UserStatus(string username, bool pinged)
        {
            this.username = username;
            this.pinged = pinged;
        }
    }
}