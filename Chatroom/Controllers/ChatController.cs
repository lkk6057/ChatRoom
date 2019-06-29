using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chatroom.Modules;
using Chatroom;
using Chatroom.Models;
namespace Chatroom.Controllers
{
    public class ChatController : Controller
    {
        // GET: Chat
        public ActionResult Login(string message = "Login or Register.")
        {
            Response response = new Response() { response = message };
            return View(response);
        }
        [HttpPost]
        public ActionResult SendLogin(string username, string password)
        {
            Main main = MvcApplication.main;
            bool UserExists = main.CheckUser(username,password);
            if (UserExists) {
                string token = main.GenToken(username);
                return RedirectToAction("ChatRoom", new { token });
            }
            else
            {
                if (main.GetUsers().Find(x=>x.username==username)==null) {
                    if (username.Length < 25 && !username.Contains(" ") && password.Length < 200) {
                        main.AddUser(username, password);
                        return RedirectToAction("Login", new { message = "Registered Successfully. Please Log In." });
                    }
                    else
                    {
                        return RedirectToAction("Login", new { message = "Failed to Register, Invalid/too many characters." });
                    }
                }
                else
                {
                    return RedirectToAction("Login", new { message = "Failed to Register, Username already taken." });
                }
            }
        }
        [HttpPost]
        public ActionResult Get(string token)
        {
            if (MvcApplication.main.CheckToken(token)) {
                MvcApplication.main.Ping(token);
                return Content(MvcApplication.main.GetMessageString());
            }
            else if (token==MvcApplication.main.admintoken&&MvcApplication.main.adminenabled)
            {
                return Content(MvcApplication.main.GetMessageString());
            }
            else
            {
                return Content("Invalid token, please login again.");
            }
        }
        public ActionResult GetOnline()
        {
            return Content(MvcApplication.main.GetOnline());
        }
        public ActionResult ChatRoom(string token)
        {
            Token tokenmodel = new Token() {token = token};
            return View(tokenmodel);
        }
        [HttpPost]
        public ActionResult SendMessage(string message, string token)
        {
            if (token!=MvcApplication.main.admintoken) {
                if (message.Length < 10000)
                {
                    MvcApplication.main.SendMessage(message, token);
                }
            }
            else
            {
                MvcApplication.main.AdminMessage(message);
            }
            Token tokenmodel = new Token() { token = token };
            return View("ChatRoom",tokenmodel);
        }
    }
}