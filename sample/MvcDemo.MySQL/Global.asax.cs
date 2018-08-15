using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.Mvc;
using MvcDemo.MySQL.Database;
using MvcDemo.MySQL.Environment;
using MvcDemo.MySQL.Repositories;
using MvcDemo.MySQL.Repositories.NHibernate;
using StackExchange.Redis;

namespace MvcDemo.MySQL
{
  // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
  // 请访问 http://go.microsoft.com/?LinkId=9394801
  public class MvcApplication : System.Web.HttpApplication
  {
    /// <summary>
    /// 应用程序启动时执行的事件
    /// </summary>
    protected void Application_Start()
    {
      //获取当前业务相关的程序集
      Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies()
        .Where(n => n.FullName.StartsWith("MvcDemo.MySQL")).ToArray();

      //初始化DI容器
      InitializeDIContainer(assemblies);

      //初始化MVC环境
      InitializeMVC();


    }

    /// <summary>
    /// 应用程序终止时执行的事件
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    protected void Application_End(Object source, EventArgs e)
    {
      //记录日志

    }

    /// <summary>
    /// 自定义404、500的信息显示页面
    /// </summary>
    protected void Application_EndRequest()
    {
      if (Response.StatusCode == 404)
      {


      }
      else if (Response.StatusCode == 500)
      {

      }



      // 提交事务并关闭Session
      DiContainer.Resolve<SessionManager>().CloseSession();

    }

    /// <summary>
    /// 处理系统错误日志
    /// </summary>
    protected void Application_OnError()
    {
      //将异常记录到日志
      var exception = Server.GetLastError();
      //logger.Error(Request.Url.ToString(), exception);
    }

    /// <summary>
    /// 去除Response Header中的Server 信息 
    /// </summary>
    protected void Application_PostReleaseRequestState()
    {
      Response.Headers.Remove("Server");
    }

    /// <summary>
    /// 初始化DI容器
    /// </summary>
    private void InitializeDIContainer(Assembly[] assemblies)
    {
      var containerBuilder = new ContainerBuilder();

      //批量注册程序集中的实现
      containerBuilder.RegisterAssemblyTypes(assemblies)
        .Where(t => t.Name.EndsWith("Service")).SingleInstance()
        .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

      //注册Repository
      containerBuilder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).SingleInstance().PropertiesAutowired();

      //注册NHibernate的SessionManager
      containerBuilder.Register(c => new SessionManager(assemblies)).SingleInstance().PropertiesAutowired();

      //注册Redis服务
      string connectionString = "localhost:6379,allowAdmin=true,abortConnect=false,syncTimeout=5000";
      ConfigurationOptions option = new ConfigurationOptions();
      option.AllowAdmin = true;
      option.AbortOnConnectFail = false;
      option.SyncTimeout = 6000;
      option.EndPoints.Add("127.0.0.1", 6379);
      option.EndPoints.Add("127.0.0.1", 6380);
      option.EndPoints.Add("127.0.0.1", 6381);
      option.EndPoints.Add("127.0.0.1", 6382);

      containerBuilder.Register(c => ConnectionMultiplexer.Connect(option)).SingleInstance();


      //注入MVC Autofac.Mvc5.4.0.2
      containerBuilder.RegisterControllers(assemblies).PropertiesAutowired();
      containerBuilder.RegisterSource(new ViewRegistrationSource());
      containerBuilder.RegisterModelBinders(assemblies);
      containerBuilder.RegisterModelBinderProvider();
      containerBuilder.RegisterFilterProvider();
      containerBuilder.RegisterModule(new AutofacWebTypesModule());

      IContainer container = containerBuilder.Build();

      //设置MVC的解析器
      DependencyResolver.SetResolver(new Autofac.Integration.Mvc.AutofacDependencyResolver(container));

      DiContainer.RegisterContainer(container);
    }

    /// <summary>
    /// 初始化MVC环境
    /// </summary>
    private void InitializeMVC()
    {
      //注册MVC路由
      RouteConfig.RegisterRoutes(RouteTable.Routes);

      //禁止Response的header信息包含X-AspNetMvc-Version
      MvcHandler.DisableMvcResponseHeader = true;
    }


  }
}