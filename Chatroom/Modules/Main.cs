using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Timers;
namespace Chatroom.Modules
{
    public class Main
    {
        public string admintoken;
        public bool adminenabled;
        BinaryFormatter bf;
        FileStream file;
        string basepath;
        string path;
        string textpath;
        string chatpath;
        string adminpath;
        Data data;
        List<string> messages = new List<string>();
        List<UserStatus> onlineusers = new List<UserStatus>();
        public void Start()
        {

            bf = new BinaryFormatter();
            basepath = Directory.GetCurrentDirectory()+@"\Chatroom";
            path = basepath + @"\data.dat";
            textpath = basepath + @"\users.txt";
            chatpath = basepath + @"\chat.dat";
            adminpath = basepath + @"\admintoken.txt";
            Directory.CreateDirectory(basepath);
            if (File.Exists(adminpath))
            {
                admintoken = File.ReadAllText(adminpath);
                adminenabled = true;
            }
            else
            {
                adminenabled = false;
            }
            if (!File.Exists(path))
            {
                file = File.Create(path);
                data = new Data(new List<User>());
                file.Close();
                Save();
            }
            if (!File.Exists(chatpath))
            {
                file = File.Create(chatpath);
                ChatCache chatcache = new ChatCache(new List<string>());
                bf.Serialize(file,chatcache);
                file.Close();
            }
            Load();
            System.Timers.Timer timer = new System.Timers.Timer(3000);
            timer.AutoReset = true;
            timer.Elapsed += TimerTick;
            timer.Start();
        }
        void TimerTick(object sender, ElapsedEventArgs e)
        {
            List<UserStatus> UsersToRemove = new List<UserStatus>();
            for (int i = 0; i<onlineusers.Count;i++)
            {
                if (onlineusers[i].pinged)
                {
                    onlineusers[i].pinged = false;
                }
                else
                {
                    UsersToRemove.Add(onlineusers[i]);
                }
            }
            foreach (UserStatus status in UsersToRemove)
            {
                onlineusers.Remove(status);
            }
        }
        public List<User> GetUsers()
        {
            return data.users;
        }
        public void Load()
        {
            file = File.Open(path, FileMode.Open);
            if (new FileInfo(path).Length>0) {
                data = (Data)bf.Deserialize(file);
            }
            file.Close();
            file = File.Open(chatpath,FileMode.Open);
            ChatCache chatcache = (ChatCache)bf.Deserialize(file);
            file.Close();
            messages = chatcache.cache;
            File.WriteAllLines(basepath+@"/messagecache.txt",messages);
        }
        public void Save()
        {
            file = File.Create(path);
            bf.Serialize(file,data);
            file.Close();
            string users = "";
            for (int i = 0;i<data.users.Count;i++)
            {
                users += $"Username:{data.users[i].username} Password:{data.users[i].password} Token:{data.users[i].token}{Environment.NewLine}";
            }
            File.WriteAllText(textpath,users);
            
        }
        public void SaveChat()
        {
            try
            {
                file = File.Create(chatpath);
                bf.Serialize(file, new ChatCache(messages));
                file.Close();
            }
            catch(Exception ex)
            {

            }
        }
        public string GenToken(string username)
        {
            string token = RandomString();
            int userindex = data.users.FindIndex(x => x.username == username);
            data.users[userindex].token = token;
            Save();
            return token;
        }
        
        public string RandomString(int length = 25)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public User GetUser(string username,string password)
        {
            if (data.users.Find(x=>x.username==username)!=null&&data.users.Find(x=>x.password==password)!=null)
            {
                return data.users.Find(x => x.username == username);
            }
            else
            {
                return null;
            }
        }
        public bool CheckUser(string username,string password)
        {
            if (GetUser(username,password)==null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public void AddUser(string username, string password)
        {
            data.users.Add(new User(username,password));
            Save();
        }
        public void AddMessage(string message, string token)
        {
            try
            {
                    User user = data.users.Find(x => x.token == token);
                    if (user != null)
                    {
                        string trymessage = $"{DateTime.UtcNow.ToString()} UTC {user.username}: {message}";
                        messages.Add(trymessage);
                        if (messages.Count > 45)
                        {
                            messages.RemoveRange(0, messages.Count - 44);
                        }
                    }
            }
            catch (Exception ex)
            {

            }
        }
        public void AdminMessage(string message)
        {
            string trymessage = $"<font size=5 color=red><i><u><b>God: {message}</b></u></i></font>";
            messages.Add(trymessage);
            if (messages.Count > 45)
            {
                messages.RemoveRange(0, messages.Count - 44);
            }
        }
        public string GetMessageString()
        {
            string messagestring = String.Join("<br>",messages);
            messagestring += "<br><br>";
            return messagestring;
        }
        public void Ping(string token)
        {
            string username = data.users.Find(x => x.token == token).username;
            if (username!=null) {
                if (onlineusers.Find(x => x.username == username) == null)
                {
                    onlineusers.Add(new UserStatus(username, true));
                }
                else
                {
                    onlineusers.Find(x => x.username == username).pinged = true;
                }
            }
        }
        public bool CheckToken(string token)
        {
            if (data.users.Find(x=>x.token==token)!=null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string GetOnline()
        {
            List<string> onlineusernames = new List<string>();
            foreach (UserStatus status in onlineusers)
            {
                onlineusernames.Add(status.username);
            }
            return $"Online Users({onlineusers.Count}):<br>{String.Join("<br>",onlineusernames)}";
        }
        public void SendMessage(string message, string token)
        {
                AddMessage(message, token);
                SaveChat();
        }
    }
}