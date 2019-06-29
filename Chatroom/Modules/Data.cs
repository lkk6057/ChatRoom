using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Chatroom.Modules
{
    [Serializable]
    public class Data
    {
        public List<User> users;
        public Data(List<User> users)
        {
            this.users = users;
        }
    }
}