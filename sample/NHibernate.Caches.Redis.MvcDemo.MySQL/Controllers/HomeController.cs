using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Demo.MySQL.Models;
using NHibernate;
using NHibernate.Linq;

namespace Demo.MySQL.Controllers
{
  public class HomeController : Controller
  {
    public readonly TestService _TextService;

    public HomeController(TestService textService)
    {
      _TextService = textService;
    }




    [HttpGet]
    public ActionResult Index(int id = 200)
    {
      for (int i = 0; i < id; i++)
      {
        _TextService.Create(new TestInfo()
        {
          Id = i * id * new Random().Next(id),
          Body = DateTime.Now + Guid.NewGuid().ToString() + "-" + i * 3,
          DateCreated = DateTime.Now,
          DecimalValue = Convert.ToDecimal(i * id * new Random().Next(id)),
          FloatValue = Convert.ToSingle(i * id * new Random().Next(id)),
          IsDel = true,
          LongValue = Convert.ToInt64(i * id * new Random().Next(id)),
          Status = i * 3,
          Type = new Random().Next(255)
        });
      }


      Stopwatch sw = new Stopwatch();

      sw.Start();
      var posts = _TextService.GetList();
      sw.Stop();
      var a1 = sw.ElapsedMilliseconds;
      sw.Restart();
      //var posts1 = _TextService._repository.Table;
      sw.Stop();
      var a2 = sw.ElapsedMilliseconds;
      ViewBag.Name = a1 + "<br />:-------:<br />" + a2;
      return View(posts);
    }







  }
}