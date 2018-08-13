using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using MvcDemo.MySQL.Models;

namespace MvcDemo.MySQL.Controllers
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
    
      Stopwatch sw = new Stopwatch();

      sw.Start();
      var posts = _TextService.Gets(20, 1).ToList();
      sw.Stop();
      var a1 = sw.ElapsedMilliseconds;
      sw.Restart();
      var posts1 = _TextService.GetList();
      sw.Stop();
      var a2 = sw.ElapsedMilliseconds;
      ViewBag.Name = $"分页查询20条{a1}毫秒<br />:-------:<br />全部查询{posts1.Count}条{a2}毫秒。";
      return View(posts);
    }

    public ActionResult D()
    {
      int id = 200;
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
      return Content("CCC");
    }





  }
}