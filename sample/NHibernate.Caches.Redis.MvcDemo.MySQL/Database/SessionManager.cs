using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Web;
using Demo.MySQL.Environment;
using NHibernate;
using NHibernate.Cache.DynamicCacheBuster;
using NHibernate.Caches.Redis;
using NHibernate.Cfg;
using NHibernate.Context;
using NHibernate.Mapping.ByCode;
using StackExchange.Redis;
//using Common.Logging;

namespace Demo.MySQL.Database
{
  /// <summary>
  /// Session和Transaction管理类，单例类。
  /// </summary>
  public class SessionManager
  {
    //private static readonly ILog logger = LogManager.GetLogger<SessionManager>();

    /// <summary>
    /// SessionFactory
    /// </summary>
    private readonly ISessionFactory SessionFactory;


    /// <summary>
    /// 构造器
    /// </summary>
    public SessionManager(Assembly[] assemblies)
    {
      //通过Mapping by code加载映射
      var mapper = new ModelMapper();
      foreach (var assembly in assemblies)
      {
        try
        {
          mapper.AddMappings(assembly.GetExportedTypes());
        }
        catch (Exception e)
        {
          //有些程序集里不包含NH配置信息，会抛异常，捕获但不处理
        }
      }

      //This will write all the XML into the bin/mappings folder
      if (HttpContext.Current == null)
      {
        mapper.CompileMappingForEachExplicitlyAddedEntity().WriteAllXmlMapping();
      }

      var hbmMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

      var configure = new Configuration();
      configure.Configure();
      configure.AddDeserializedMapping(hbmMapping, "");
      configure.CurrentSessionContext<WebSessionContext>();

      //设置NHibernate使用Redis缓存
      //https://github.com/TheCloudlessSky/NHibernate.Caches.Redis
      var connectionMultiplexer = DIContainer.Resolve<ConnectionMultiplexer>();
      RedisCacheProvider.SetConnectionMultiplexer(connectionMultiplexer);

      //设置Redis的序列化器
      var options = new RedisCacheProviderOptions()
      {
        Serializer = new NhJsonCacheSerializer(),
        SerializerType = SerializerType.JSON
      };
      RedisCacheProvider.SetOptions(options);

      //设置NHibernate在表结构变化时自动更新缓存
      //https://github.com/TheCloudlessSky/NHibernate.Cache.DynamicCacheBuster
      new CacheBuster().AppendVersionToCacheRegionNames(configure);


      SessionFactory = configure.BuildSessionFactory();
    }


    /// <summary>
    /// ISession实例
    /// </summary>
    public ISession Session
    {
      get
      {
        ISession session = null;

        //如果当前的HttpContext为空，则从CallContext中获取当前的Session。
        if (HttpContext.Current == null)
        {
          session = (ISession)CallContext.GetData(typeof(ISession).FullName);

          if (session == null || !session.IsOpen)
          {
            session = SessionFactory.OpenSession(new DebugInterceptor());
            CallContext.SetData(typeof(ISession).FullName, session);
          }
        }
        //从绑定的WebContext获取Session。
        else
        {
          if (CurrentSessionContext.HasBind(SessionFactory))
          {
            session = SessionFactory.GetCurrentSession();
          }

          if (session == null || !session.IsOpen)
          {
            session = SessionFactory.OpenSession(new DebugInterceptor());
            CurrentSessionContext.Bind(session);
          }
        }

        return session;
      }
    }

    /// <summary>
    /// 提交事务并关闭Session
    /// </summary>
    public void CloseSession()
    {
      ISession session = null;

      if (HttpContext.Current == null)
      {
        session = (ISession)CallContext.GetData(typeof(ISession).FullName);
      }
      else
      {
        session = CurrentSessionContext.Unbind(SessionFactory);
      }

      if (session == null || !session.IsOpen)
      {
        return;
      }

      if (session.Transaction != null)
      {
        if (session.Transaction.IsActive)
        {
          try
          {
            session.Transaction.Commit();
          }
          catch (Exception e)
          {
            //logger.Error("Error while committing the transaction.", e);
          }
        }

        session.Transaction.Dispose();
      }

      session.Close();
    }
  }

  /// <summary>
  /// 用于查看生成的Sql的拦截器
  /// </summary>
  internal class DebugInterceptor : EmptyInterceptor
  {
    public override global::NHibernate.SqlCommand.SqlString OnPrepareStatement(global::NHibernate.SqlCommand.SqlString sql)
    {
      Debug.WriteLine(sql.ToString());

      return base.OnPrepareStatement(sql);
    }
  }
}