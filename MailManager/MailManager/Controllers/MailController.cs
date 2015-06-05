using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MailManager.Controllers
{
    public class MailController : Controller
    {
        //
        // GET: /Mail/
        private MailManager.Models.MailManagerEntities db = new Models.MailManagerEntities();
        public ActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "guest")]
        public ActionResult Mailed(int pageNum = 1, int numPerPage = 20, string orderField = "ID", string orderDirection = "asc", string keywords = "")
        {
            IQueryable<Models.T_Base_Mail> lst = null;
            if (keywords != "")
                lst = db.T_Base_Mail.Where(s => s.Type == 2 && s.Number.ToUpper().Contains(keywords.ToUpper()));
            else
                lst = db.T_Base_Mail.Where(s => s.Type == 2).OrderBy(m => m.ID);
            ViewBag.totalCount = lst.Count();
            if (orderDirection == "asc")
            {
                if (orderField == "ID") lst = lst.OrderBy(m => m.ID);
                else if (orderField == "CreateTime") lst = lst.OrderBy(m => m.CreateTime);
                else if (orderField == "Type") lst = lst.OrderBy(m => m.Type);
            }
            else
            {
                if (orderField == "ID") lst = lst.OrderByDescending(m => m.ID);
                else if (orderField == "CreateTime") lst = lst.OrderByDescending(m => m.CreateTime);
                else if (orderField == "Type") lst = lst.OrderByDescending(m => m.Type);
            }
            List<Models.T_Base_Mail> list = lst.ToList();
            List<Models.T_Base_Mail> list1 = new List<Models.T_Base_Mail>();
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
        public ActionResult Mail(int pageNum = 1, int numPerPage = 20, string orderField = "ID", string orderDirection = "asc", string keywords = "")
        {
            IQueryable<Models.T_Base_Mail> lst = null;
            if (keywords != "")
                lst = db.T_Base_Mail.Where(s => (s.Type == 1 || s.Type == 3) && s.Number.ToUpper().Contains(keywords.ToUpper()));
            else
                lst = db.T_Base_Mail.Where(s => (s.Type == 1 || s.Type == 3)).OrderBy(m => m.ID);
            ViewBag.totalCount = lst.Count();
            if (orderDirection == "asc")
            {
                if (orderField == "ID") lst = lst.OrderBy(m => m.ID);
                else if (orderField == "CreateTime") lst = lst.OrderBy(m => m.CreateTime);
                else if (orderField == "Type") lst = lst.OrderBy(m => m.Type);
            }
            else
            {
                if (orderField == "ID") lst = lst.OrderByDescending(m => m.ID);
                else if (orderField == "CreateTime") lst = lst.OrderByDescending(m => m.CreateTime);
                else if (orderField == "Type") lst = lst.OrderByDescending(m => m.Type);
            }
            List<Models.T_Base_Mail> list = lst.ToList();
            List<Models.T_Base_Mail> list1 = new List<Models.T_Base_Mail>();
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
        public ActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "guest")]
        public JsonResult Save()
        {
            int id = Convert.ToInt16(HttpContext.Request["org1.ID"]);
            List<Models.T_Base_Class> lst = db.T_Base_Class.Where(s=>s.ID == id).ToList();
            if (lst.Count() < 0)
                return Json(new Models.Message()
                {
                    statusCode = 300,
                    rel = "mailList",
                    callbackType = "",
                    confirmMsg = "",
                    forwardUrl = "",
                    message = "未找到该学生/教职工",
                    navTabId = "mailList"
                });
            else
            {
                Models.T_Base_Mail mail = new Models.T_Base_Mail();
                mail.ClassID = id;
                mail.CreateTime = System.DateTime.Now;
                if (lst[0].Type == 1)
                    mail.Name = HttpContext.Request["NickName"].ToString();
                else
                    mail.Name = lst[0].Name;
                mail.Number = (System.DateTime.Now.ToFileTime() - 130778000000000000).ToString();
                mail.Type = 1;
                
                db.T_Base_Mail.Add(mail);
                db.SaveChanges();
                return Json(new Models.Message()
                {
                    statusCode = 200,
                    rel = "mailList",
                    callbackType = "closeCurrent",
                    confirmMsg = "",
                    forwardUrl = "",
                    message = "添加成功",
                    navTabId = "mailList"
                });
            }
        }
        [Authorize(Roles = "guest")]
        public JsonResult Delete(int id)
        {
            List<Models.T_Base_Mail> lst = db.T_Base_Mail.Where(m => m.ID == id).ToList();
            if (lst.Count() <= 0)
                return Json(new Models.Message()
                {
                    statusCode = 300,
                    rel = "mailList",
                    callbackType = "",
                    confirmMsg = "",
                    forwardUrl = "",
                    message = "未找到记录",
                    navTabId = "mailList"
                });
            else
            {
                lst[0].Type = 2;
                lst[0].DeleteTime = System.DateTime.Now;
                db.Entry(lst[0]).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new Models.Message()
                {
                    statusCode = 200,
                    rel = "mailList",
                    callbackType = "",
                    confirmMsg = "",
                    forwardUrl = "",
                    message = "取件成功",
                    navTabId = "mailList"
                });
            }
        }
        [Authorize(Roles = "guest")]
        public JsonResult Deletes(string ids)
        {
            List<Models.T_Base_Mail> lst = null;
            string[] id = ids.Split(',');
            foreach (string i in id)
            {
                int n = Convert.ToInt32(i);
                lst = db.T_Base_Mail.Where(m => m.ID == n).ToList();
                if (lst.Count() <= 0)
                    return Json(new Models.Message()
                    {
                        statusCode = 300,
                        rel = "mailList",
                        callbackType = "",
                        confirmMsg = "",
                        forwardUrl = "",
                        message = "未找到记录",
                        navTabId = "mailList"
                    });
                else
                {
                    lst[0].Type = 2;
                    lst[0].DeleteTime = System.DateTime.Now;
                    db.Entry(lst[0]).State = EntityState.Modified;
                }
            }
            
            db.SaveChanges();
            return Json(new Models.Message()
            {
                statusCode = 200,
                rel = "mailList",
                callbackType = "",
                confirmMsg = "",
                forwardUrl = "",
                message = "批量取件成功,共取掉" + id.Count() + "件",
                navTabId = "mailList"
            });
        }
        [Authorize(Roles = "guest")]
        public ActionResult Message(int pageNum = 1, int numPerPage = 5, string orderField = "ID", string orderDirection = "asc", string keywords = "")
        {
            IQueryable<Models.T_Base_Mail> lst = null;
            if (keywords != "")
                lst = db.T_Base_Mail.Where(s => s.Type == 1 && s.Number.ToUpper().Contains(keywords.ToUpper()));
            else
                lst = db.T_Base_Mail.Where(s => s.Type == 1).OrderBy(m => m.ID);
            ViewBag.totalCount = lst.Count();
            if (orderDirection == "asc")
            {
                if (orderField == "ID") lst = lst.OrderBy(m => m.ID);
                else if (orderField == "CreateTime") lst = lst.OrderBy(m => m.CreateTime);
                else if (orderField == "Type") lst = lst.OrderBy(m => m.Type);
            }
            else
            {
                if (orderField == "ID") lst = lst.OrderByDescending(m => m.ID);
                else if (orderField == "CreateTime") lst = lst.OrderByDescending(m => m.CreateTime);
                else if (orderField == "Type") lst = lst.OrderByDescending(m => m.Type);
            }
            List<Models.T_Base_Mail> list = lst.ToList();
            List<Models.T_Base_Mail> list1 = new List<Models.T_Base_Mail>();
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
        public ActionResult Messaged(int pageNum = 1, int numPerPage = 5, string orderField = "ID", string orderDirection = "asc", string keywords = "")
        {
            IQueryable<Models.T_Base_Mail> lst = null;
            if (keywords != "")
                lst = db.T_Base_Mail.Where(s => s.Type == 3 && s.Number.ToUpper().Contains(keywords.ToUpper()));
            else
                lst = db.T_Base_Mail.Where(s => s.Type ==3).OrderBy(m => m.ID);
            ViewBag.totalCount = lst.Count();
            if (orderDirection == "asc")
            {
                if (orderField == "ID") lst = lst.OrderBy(m => m.ID);
                else if (orderField == "CreateTime") lst = lst.OrderBy(m => m.CreateTime);
                else if (orderField == "Type") lst = lst.OrderBy(m => m.Type);
            }
            else
            {
                if (orderField == "ID") lst = lst.OrderByDescending(m => m.ID);
                else if (orderField == "CreateTime") lst = lst.OrderByDescending(m => m.CreateTime);
                else if (orderField == "Type") lst = lst.OrderByDescending(m => m.Type);
            }
            List<Models.T_Base_Mail> list = lst.ToList();
            List<Models.T_Base_Mail> list1 = new List<Models.T_Base_Mail>();
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
        public JsonResult Send(int id)
        {
            List<Models.T_Base_Mail> lst = db.T_Base_Mail.Where(m => m.ID == id).ToList();
            string str = "";
            if (lst.Count() <= 0)
                return Json(new Models.Message()
                {
                    statusCode = 300,
                    rel = "message",
                    callbackType = "",
                    confirmMsg = "",
                    forwardUrl = "",
                    message = "未找到记录",
                    navTabId = "message"
                });
            else
            {
                lst[0].Type = 3;
                db.Entry(lst[0]).State = EntityState.Modified;
                db.SaveChanges();
                int ClassID = (int)lst[0].ClassID;
                Models.T_Base_Class info = db.T_Base_Class.Where(m => m.ID == ClassID).ToList()[0];
                str = "已发送：号码"+ info.Phone + ",内容 ";
                if(info.Type == 1){
                    str +=(info.NickName + "同学，请通知你们班的" + lst[0].Name +"同学持编号"+lst[0].Number+",到收发室领取信件");
                }else {
                    str +=(info.Name + "老师，请到收发室领取信件，编号" + lst[0].Number);
                }
                return Json(new Models.Message()
                {
                    statusCode = 200,
                    rel = "message",
                    callbackType = "",
                    confirmMsg = "",
                    forwardUrl = "",
                    message = str,
                    navTabId = "message"
                });
            }
        }
        [Authorize(Roles = "guest")]
        public JsonResult Sends(string ids)
        {
            List<Models.T_Base_Mail> lst = null;
            string[] id = ids.Split(',');
            foreach (string i in id)
            {
                int n = Convert.ToInt32(i);
                lst = db.T_Base_Mail.Where(m => m.ID == n).ToList();
                if (lst.Count() <= 0)
                    return Json(new Models.Message()
                    {
                        statusCode = 300,
                        rel = "mailList",
                        callbackType = "",
                        confirmMsg = "",
                        forwardUrl = "",
                        message = "未找到记录",
                        navTabId = "mailList"
                    });
                else
                {
                    lst[0].Type = 3;
                    db.Entry(lst[0]).State = EntityState.Modified;
                }
            }

            db.SaveChanges();
            return Json(new Models.Message()
            {
                statusCode = 200,
                rel = "mailList",
                callbackType = "",
                confirmMsg = "",
                forwardUrl = "",
                message = "批量发送成功,共发送" + id.Count() + "次",
                navTabId = "mailList"
            });
        }
        [Authorize(Roles = "guest")]
        public ActionResult OutTime(int pageNum = 1, int numPerPage = 5, string orderField = "ID", string orderDirection = "asc", string keywords = "")
        {
            IQueryable<Models.T_Base_Mail> lst = null;
            if (keywords != "")
                lst = db.T_Base_Mail.Where(s => (s.Type == 1 || s.Type == 3) && s.Number.ToUpper().Contains(keywords.ToUpper()));
            else
                lst = db.T_Base_Mail.Where(s => (s.Type == 1 || s.Type == 3)).OrderBy(m => m.ID);
            
            if (orderDirection == "asc")
            {
                if (orderField == "ID") lst = lst.OrderBy(m => m.ID);
                else if (orderField == "CreateTime") lst = lst.OrderBy(m => m.CreateTime);
                else if (orderField == "Type") lst = lst.OrderBy(m => m.Type);
            }
            else
            {
                if (orderField == "ID") lst = lst.OrderByDescending(m => m.ID);
                else if (orderField == "CreateTime") lst = lst.OrderByDescending(m => m.CreateTime);
                else if (orderField == "Type") lst = lst.OrderByDescending(m => m.Type);
            }
            List<Models.T_Base_Mail> list = lst.ToList();
            List<Models.T_Base_Mail> list2 = new List<Models.T_Base_Mail>();
            for (int i = 0; i < list.Count(); i++)
            {
                if (list[i].CreateTime < (System.DateTime.Now.AddDays(-30d)))
                    list2.Add(list[i]);
            }
            ViewBag.totalCount = list2.Count();
            List<Models.T_Base_Mail> list1 = new List<Models.T_Base_Mail>();
            for (int i = (pageNum - 1) * numPerPage; i < pageNum * numPerPage; i++)
            {
                if (i >= list2.Count()) break;
                list1.Add(list2[i]);
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
            Models.T_Base_Mail mail = db.T_Base_Mail.Single(m => m.ID == id);
            Models.T_Base_Class info = db.T_Base_Class.Where(m => m.ID == ((int)mail.ClassID)).ToList()[0];
            ViewBag.type = info.Type;
            ViewBag.Name = info.Name;
            return View(mail);
        }
        [Authorize(Roles = "guest")]
        public JsonResult SaveEdit()
        {
            int mailID =  Convert.ToInt16(HttpContext.Request["ID"]);
            int id = Convert.ToInt16(HttpContext.Request["org1.ID"]);
            int type = Convert.ToInt16(HttpContext.Request["type"]);
            try
            {
                Models.T_Base_Mail mail = db.T_Base_Mail.Where(m => m.ID == mailID).ToList()[0];
                if(type == 1)
                    mail.Name = HttpContext.Request["NickName"].ToString();
                else
                    mail.Name = HttpContext.Request["org1.Name"].ToString();
                if (mail.Type == 2)
                {
                    mail.Pname = HttpContext.Request["Pname"].ToString();
                    mail.Phone = HttpContext.Request["Phone"].ToString();
                }else if(mail.Type>3)
                    mail.Tip = HttpContext.Request["tip"].ToString();
                db.Entry(mail).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new Models.Message()
                {
                    statusCode = 200,
                    rel = "mailList",
                    callbackType = "closeCurrent",
                    confirmMsg = "",
                    forwardUrl = "",
                    message = "修改成功",
                    navTabId = "mailList"
                });
            }
            catch
            {
                return Json(new Models.Message()
                {
                    statusCode = 200,
                    rel = "mailList",
                    callbackType = "",
                    confirmMsg = "",
                    forwardUrl = "",
                    message = "添加成功",
                    navTabId = "mailList"
                });
            }
        }
        [Authorize(Roles = "guest")]
        public ActionResult Other(int id)
        {
            Models.T_Base_Mail mail = db.T_Base_Mail.Single(m => m.ID == id);
            return View(mail);
        }
        [Authorize(Roles = "guest")]
        public JsonResult OtherDelete(int ID,string Phone,string Pname)
        {
            List<Models.T_Base_Mail> lst = db.T_Base_Mail.Where(m => m.ID == ID).ToList();
            if (lst.Count() <= 0)
                return Json(new Models.Message()
                {
                    statusCode = 300,
                    rel = "mailList",
                    callbackType = "",
                    confirmMsg = "",
                    forwardUrl = "",
                    message = "未找到记录",
                    navTabId = "mailList"
                });
            else
            {
                lst[0].Type = 2;
                lst[0].Pname = Pname;
                lst[0].Phone = Phone;
                lst[0].DeleteTime = System.DateTime.Now;
                db.Entry(lst[0]).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new Models.Message()
                {
                    statusCode = 200,
                    rel = "mailList",
                    callbackType = "closeCurrent",
                    confirmMsg = "",
                    forwardUrl = "",
                    message = "登记成功",
                    navTabId = "mailList"
                });
            }
        }
        [Authorize(Roles = "guest")]
        public ActionResult SendBack(int id)
        {
            Models.T_Base_Mail mail = db.T_Base_Mail.Single(m => m.ID == id);
            return View(mail);
        }
        [Authorize(Roles = "guest")]
        public JsonResult SendBackSave(int ID, int type,string tip)
        {
            List<Models.T_Base_Mail> lst = db.T_Base_Mail.Where(m => m.ID == ID).ToList();
            if (lst.Count() <= 0)
                return Json(new Models.Message()
                {
                    statusCode = 300,
                    callbackType = "",
                    confirmMsg = "",
                    forwardUrl = "",
                    message = "未找到记录",
                    navTabId = "mailList"
                });
            else
            {
                lst[0].Type = type;
                lst[0].Tip = tip;
                lst[0].DeleteTime = System.DateTime.Now;
                db.Entry(lst[0]).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new Models.Message()
                {
                    statusCode = 200,
                    callbackType = "closeCurrent",
                    confirmMsg = "",
                    forwardUrl = "",
                    message = "登记成功",
                    navTabId = "mailList"
                });
            }
        }
        [Authorize(Roles = "guest")]
        public JsonResult OutTimeSave(int id)
        {
            List<Models.T_Base_Mail> lst = db.T_Base_Mail.Where(m => m.ID == id).ToList();
            if (lst.Count() <= 0)
                return Json(new Models.Message()
                {
                    statusCode = 300,
                    rel = "outtime",
                    callbackType = "",
                    confirmMsg = "",
                    forwardUrl = "",
                    message = "未找到记录",
                    navTabId = "outtime"
                });
            else
            {
                lst[0].Type = 4;
                lst[0].Tip = "过期，退回";
                lst[0].DeleteTime = System.DateTime.Now;
                db.Entry(lst[0]).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new Models.Message()
                {
                    statusCode = 200,
                    rel = "outtime",
                    callbackType = "",
                    confirmMsg = "",
                    forwardUrl = "",
                    message = "登记成功",
                    navTabId = "outtime"
                });
            }
        }
        [Authorize(Roles = "guest")]
        public JsonResult OutTimeSaves(string ids)
        {
            List<Models.T_Base_Mail> lst = null;
            string[] id = ids.Split(',');
            foreach (string i in id)
            {
                int n = Convert.ToInt32(i);
                lst = db.T_Base_Mail.Where(m => m.ID == n).ToList();
                if (lst.Count() <= 0)
                    return Json(new Models.Message()
                    {
                        statusCode = 300,
                        rel = "mailList",
                        callbackType = "",
                        confirmMsg = "",
                        forwardUrl = "",
                        message = "未找到记录",
                        navTabId = "outtime"
                    });
                else
                {
                    lst[0].Type = 4;
                    lst[0].Tip = "过期，退回";
                    lst[0].DeleteTime = System.DateTime.Now;
                    db.Entry(lst[0]).State = EntityState.Modified;
                }
            }

            db.SaveChanges();
            return Json(new Models.Message()
            {
                statusCode = 200,
                rel = "mailList",
                callbackType = "",
                confirmMsg = "",
                forwardUrl = "",
                message = "批量登记成功,共取掉" + id.Count() + "件",
                navTabId = "outtime"
            });
        }
        [Authorize(Roles = "guest")]
        public ActionResult Mailing(int pageNum = 1, int numPerPage = 20, string orderField = "ID", string orderDirection = "asc", string keywords = "")
        {
            IQueryable<Models.T_Base_Mail> lst = null;
            if (keywords != "")
                lst = db.T_Base_Mail.Where(s => s.Type >3  && s.Number.ToUpper().Contains(keywords.ToUpper()));
            else
                lst = db.T_Base_Mail.Where(s => s.Type >3).OrderBy(m => m.ID);
            ViewBag.totalCount = lst.Count();
            if (orderDirection == "asc")
            {
                if (orderField == "ID") lst = lst.OrderBy(m => m.ID);
                else if (orderField == "CreateTime") lst = lst.OrderBy(m => m.CreateTime);
                else if (orderField == "Type") lst = lst.OrderBy(m => m.Type);
            }
            else
            {
                if (orderField == "ID") lst = lst.OrderByDescending(m => m.ID);
                else if (orderField == "CreateTime") lst = lst.OrderByDescending(m => m.CreateTime);
                else if (orderField == "Type") lst = lst.OrderByDescending(m => m.Type);
            }
            List<Models.T_Base_Mail> list = lst.ToList();
            List<Models.T_Base_Mail> list1 = new List<Models.T_Base_Mail>();
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
    }
}
