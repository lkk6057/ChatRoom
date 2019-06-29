using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chatroom.Modules
{
    [Serializable]
    public class User
    {
        public string username;
        public string password;
        public string token;
        public User(string username, string password,string token = "")
        {
            this.username = username;
            this.password = password;
            this.token = token;
        }
    }
}