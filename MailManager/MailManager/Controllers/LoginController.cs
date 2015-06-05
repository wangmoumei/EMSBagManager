using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MailManager.Controllers
{
    public class LoginController : Controller
    {
        //
        // GET: /Login/
        private MailManager.Models.MailManagerEntities  db =  new Models.MailManagerEntities();
        public ActionResult Index()
        {

            return View();
        }
        public JsonResult Login(string name,string pass)
        {
            List<Models.T_Base_Admin> lst = db.T_Base_Admin.Where(m => m.Name == name).ToList();
            if (lst.Count() == 0)
            {
                return Json(new { success = false, msg = "用户名不存在" });
            }
            else
            {
                if (lst[0].Password == pass)
                {
                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                    1,
                    lst[0].ID.ToString(),
                    DateTime.Now,
                    DateTime.Now.Add(FormsAuthentication.Timeout),
                    false,
                    "guest"
                    );
                    HttpCookie cookie = new HttpCookie(
                        FormsAuthentication.FormsCookieName,
                        FormsAuthentication.Encrypt(ticket)
                    );
                    Response.Cookies.Add(cookie);
                    return Json(new {success = true});
                }
                else
                    return Json(new { success = false,msg = "密码错误" });
            }
            
        }
        [Authorize(Roles = "guest")]
        public ActionResult Change()
        {
            return View();
        }
        [Authorize(Roles = "guest")]
        public JsonResult SaveChange(string password)
        {
            int id = Convert.ToInt16(User.Identity.Name);
            List<Models.T_Base_Admin> lst = db.T_Base_Admin.Where(m => m.ID == id).ToList();
            lst[0].Password = password;
            db.Entry(lst[0]).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new Models.Message()
            {
                statusCode = 200,
                rel = "Change",
                callbackType = "closeCurrent",
                confirmMsg = "",
                forwardUrl = "",
                message = "修改成功",
                navTabId = "Change"
            });
        }
        [Authorize(Roles = "guest")]
        public ActionResult Edit()
        {
            int id = Convert.ToInt16(User.Identity.Name);
            List<Models.T_Base_Admin> lst = db.T_Base_Admin.Where(m => m.ID == id).ToList();
            return View(lst[0]);
        }
        [Authorize(Roles = "guest")]
        public JsonResult SaveEdit(string email,string NickName)
        {
            int id = Convert.ToInt16(User.Identity.Name);
            List<Models.T_Base_Admin> lst = db.T_Base_Admin.Where(m => m.ID == id).ToList();
            lst[0].NickName = NickName;
            lst[0].Email = email;
            db.Entry(lst[0]).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new Models.Message()
            {
                statusCode = 200,
                rel = "Change",
                callbackType = "closeCurrent",
                confirmMsg = "",
                forwardUrl = "",
                message = "保存成功",
                navTabId = "Change"
            });
        }
        [Authorize(Roles = "guest")]
        public ActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "guest")]
        public JsonResult Save(string Name,string password,string email, string NickName)
        {
            Models.T_Base_Admin admin = new Models.T_Base_Admin();
            admin.Name = Name;
            admin.Password = password;
            admin.Email = email;
            admin.NickName = NickName;
            admin.Type = 1;
            db.T_Base_Admin.Add(admin);
            db.SaveChanges();
            return Json(new Models.Message()
            {
                statusCode = 200,
                rel = "Change",
                callbackType = "closeCurrent",
                confirmMsg = "",
                forwardUrl = "",
                message = "添加成功",
                navTabId = "Change"
            });
        }
    }
}
