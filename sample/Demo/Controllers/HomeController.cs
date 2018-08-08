using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Demo.Models;
using NHibernate;

namespace Demo.Controllers
{
  public class HomeController : Controller
  {
    private ISession session;

    [HttpGet]
    public ActionResult Index()
    {
      Stopwatch sw = new Stopwatch();

      sw.Start();
      var posts = session.QueryOver<BlogPost>().Cacheable().List();
      sw.Stop();
      var a1 = sw.ElapsedMilliseconds;
      sw.Restart();
      var pp = session.Query<BlogPost>().ToList();
      sw.Stop();
      var a2 = sw.ElapsedMilliseconds;
      ViewBag.Name = a1 + "<br />:no-:<br />" + a2;
      return View(posts);
    }

    [HttpPost]
    public ActionResult Create(string title, string body)
    {
      session.Save(new BlogPost()
      {
        Title = title,
        Body = body,
        Created = DateTime.Now
      });
      return RedirectToAction("index");
    }

    public ActionResult Set(int id = 10000)
    {
      for (int i = 0; i < id; i++)
      {
        session.Save(new BlogPost()
        {
          Title = i + "_" + DateTime.Now.ToString("yyyy-MM-ddhhmmssffff"),
          Body = i + "body-" + DateTime.Now.ToString("yyyy-MM-ddhhmmssffff"),
          Created = DateTime.Now
        });
      }

      return Content("成功添加");
    }

    protected override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      base.OnActionExecuting(filterContext);
      MvcApplication.SessionFactory.Statistics.Clear();
      this.session = MvcApplication.SessionFactory.OpenSession();
      this.session.BeginTransaction();
    }

    protected override void OnResultExecuted(ResultExecutedContext filterContext)
    {
      this.session.Transaction.Commit();
      this.session.Dispose();
      base.OnResultExecuted(filterContext);
    }
  }
}