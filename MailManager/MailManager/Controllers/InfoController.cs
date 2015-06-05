using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MailManager.Controllers
{
    public class InfoController : Controller
    {
        //
        // GET: /Info/
        private MailManager.Models.MailManagerEntities db = new Models.MailManagerEntities();
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetClass(string keyword)
        {
            List<Models.T_Base_Class> lst = new List<Models.T_Base_Class>();
            if(keyword == "")
                lst = db.T_Base_Class.Where(s=>s.Type == 1).ToList();
            else
                lst = db.T_Base_Class.Where(s=>s.Type == 1 && s.Name.ToUpper().Contains(keyword.ToUpper())).ToList();
            return Json(lst);
        }
        public JsonResult GetNikeName(string keyword)
        {
            List<Models.T_Base_Class> lst = new List<Models.T_Base_Class>();
            if(keyword == "")
                lst = db.T_Base_Class.Where(s => s.Type == 2).ToList();
            else
                lst = db.T_Base_Class.Where(s => s.Type == 2 && s.NickName.ToUpper().Contains(keyword.ToUpper())).ToList();
            return Json(lst);
        }
        [Authorize(Roles = "guest")]
        public ActionResult Create()
        {
            return View();
        }
        public JsonResult Save(int Type,string Name,string NickName,string Phone)
        {
            try
            {
                Models.T_Base_Class info = new Models.T_Base_Class();
                info.Type = Type;
                info.Name = Name;
                info.NickName = NickName;
                info.Phone = Phone;
                db.T_Base_Class.Add(info);
                db.SaveChanges();

                return Json(new Models.Message()
                {
                    statusCode = 200,
                    rel = ("infoList" + Type),
                    callbackType = "closeCurrent",
                    confirmMsg = "",
                    forwardUrl = "",
                    message = "添加成功",
                    navTabId = ("infoList" + Type)
                });
            }
            catch
            {
                return Json(new Models.Message()
                {
                    statusCode = 300,
                    rel = ("infoList" + Type),
                    callbackType = "",
                    confirmMsg = "",
                    forwardUrl = "",
                    message = "添加失败",
                    navTabId = ("infoList" + Type)
                });
            }
        }
        [Authorize(Roles = "guest")]
        public ActionResult Student(int pageNum = 1, int numPerPage = 5, string orderField = "ID", string orderDirection = "asc", string keywords = "")
        {
            IQueryable<Models.T_Base_Class> lst = null;
            if (keywords != "")
                lst = db.T_Base_Class.Where(s => s.Type == 1 && s.Name.ToUpper().Contains(keywords.ToUpper()));
            else
                lst = db.T_Base_Class.Where(s => s.Type == 1).OrderBy(m => m.ID);
            ViewBag.totalCount = lst.Count();
            if (orderDirection == "asc")
            {
                if (orderField == "NickName") lst = lst.OrderBy(m => m.NickName);
                else if (orderField == "Name") lst = lst.OrderBy(m => m.Name);
                else if (orderField == "ID") lst = lst.OrderBy(m => m.ID);
            }
            else
            {
                if (orderField == "NickName") lst = lst.OrderByDescending(m => m.NickName);
                else if (orderField == "Name") lst = lst.OrderByDescending(m => m.Name);
                else if (orderField == "ID") lst = lst.OrderByDescending(m => m.ID);
            }
            List<Models.T_Base_Class> list = lst.ToList();
            List<Models.T_Base_Class> list1 = new List<Models.T_Base_Class>();
            for (int i = (pageNum - 1) * numPerPage; i < pageNum * numPerPage; i++)
            {
                if (i >= list.Count()) break;
                list1.Add(list[i]);
            }

            ViewBag.pageNum = pageNum;
            ViewBag.numPerPage = numPerPage;
            ViewBag.orderField = orderField;
            ViewBag.orderDirection = orderDirection;
            ViewBag.keywords = keywords;
            return View(list1);
        }
        [Authorize(Roles = "guest")]
        public ActionResult Teacher(int pageNum = 1, int numPerPage = 5, string orderField = "ID", string orderDirection = "asc", string keywords = "")
        {
            IQueryable<Models.T_Base_Class> lst = null;
            if (keywords != "")
                lst = db.T_Base_Class.Where(s => s.Type == 2 && s.Name.ToUpper().Contains(keywords.ToUpper()));
            else
                lst = db.T_Base_Class.Where(s => s.Type == 2).OrderBy(m => m.ID);
            ViewBag.totalCount = lst.Count();
            if (orderDirection == "asc")
            {
                if (orderField == "NickName") lst = lst.OrderBy(m => m.NickName);
                else if (orderField == "Name") lst = lst.OrderBy(m => m.Name);
                else if (orderField == "ID") lst = lst.OrderBy(m => m.ID);
            }
            else
            {
                if (orderField == "NickName") lst = lst.OrderByDescending(m => m.NickName);
                else if (orderField == "Name") lst = lst.OrderByDescending(m => m.Name);
                else if (orderField == "ID") lst = lst.OrderByDescending(m => m.ID);
            }
            List<Models.T_Base_Class> list = lst.ToList();
            List<Models.T_Base_Class> list1 = new List<Models.T_Base_Class>();
            for (int i = (pageNum - 1) * numPerPage; i < pageNum * numPerPage; i++)
            {
                if (i >= list.Count()) break;
                list1.Add(list[i]);
            }

            ViewBag.pageNum = pageNum;
            ViewBag.numPerPage = numPerPage;
            ViewBag.orderField = orderField;
            ViewBag.orderDirection = orderDirection;
            ViewBag.keywords = keywords;
            return View(list1);
        }
        [Authorize(Roles = "guest")]
        public ActionResult Edit(int id)
        {
            Models.T_Base_Class info = db.T_Base_Class.Single(m=>m.ID == id);
            return View(info);
        }
        public JsonResult SaveEdit(int ID,int Type, string Name, string NickName, string Phone)
        {
            try
            {
                Models.T_Base_Class info = db.T_Base_Class.Where(m=>m.ID == ID).ToList()[0];
                info.Name = Name;
                info.NickName = NickName;
                info.Phone = Phone;
                db.Entry(info).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new Models.Message()
                {
                    statusCode = 200,
                    rel = ("infoList" + Type),
                    callbackType = "closeCurrent",
                    confirmMsg = "",
                    forwardUrl = "",
                    message = "保存成功",
                    navTabId = ("infoList" + Type)
                });
            }
            catch
            {
                return Json(new Models.Message()
                {
                    statusCode = 300,
                    rel = ("infoList" + Type),
                    callbackType = "",
                    confirmMsg = "",
                    forwardUrl = "",
                    message = "保存失败",
                    navTabId = ("infoList" + Type)
                });
            }
        }
    }
}
