using System;
using System.Linq;
using System.Linq.Expressions;
using Demo.MySQL.Database.Specification;
using HuobanYun.Database;
using NHibernate;
using NHibernate.Linq;
//using NHibernate.Linq;

namespace Demo.MySQL.Database
{
  /// <summary>
  /// 仓储基类
  /// </summary>
  /// <typeparam name="T">仓储对应的实体</typeparam>
  public class Repository<T> : IRepository<T> where T : class
  {
    public SessionManager sessionManager { get; set; }

    /// <summary>
    /// ISession实例
    /// </summary>
    public ISession Session
    {
      get
      {

        return sessionManager.Session;
      }
    }

    /// <summary>
    /// 根据Id查询实体
    /// </summary>
    /// <param name="id">实体Id</param>
    /// <returns>实体</returns>
    public T Get(object id)
    {
      return Session.Get<T>(id);
    }

    /// <summary>
    /// 根据指定的条件查询实体
    /// </summary>
    /// <param name="predicate">查询条件委托</param>
    /// <returns>实体</returns>
    public T SingleOrDefault(Expression<Func<T, bool>> predicate)
    {
      return Fetch(predicate).SingleOrDefault();
    }

    /// <summary>
    /// 根据指定的条件查询实体
    /// </summary>
    /// <param name="predicate">查询的条件委托</param>
    /// <param name="order">排序</param>
    /// <returns>实体</returns>
    public T SingleOrDefault(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order)
    {
      return Fetch(predicate, order).SingleOrDefault();
    }

    /// <summary>
    /// 查询指定实体的指定字段
    /// </summary>
    /// <typeparam name="TS">要查询的字段的类型</typeparam>
    /// <param name="predicate">查询条件</param>
    /// <param name="property">要查询的字段</param>
    /// <returns>指定的字段，如果没有查询结果，则为该字段类型的默认值。</returns>
    public TS GetProperty<TS>(Expression<Func<T, bool>> predicate, Func<T, TS> property)
    {
      return Fetch(predicate).Select(property).SingleOrDefault();
    }

    /// <summary>
    /// 创建实体
    /// </summary>
    /// <param name="entity">实体</param>
    public void Create(T entity)
    {
      using (var transaction = Session.BeginTransaction())
      {
        Session.Save(entity);
        transaction.Commit();
      }
    }

    /// <summary>
    /// 更新实体
    /// </summary>
    /// <param name="entity">实体</param>
    public void Update(T entity)
    {
      using (var transaction = Session.BeginTransaction())
      {
        Session.Update(entity);
        transaction.Commit();
      }
    }

    /// <summary>
    /// 删除实体
    /// </summary>
    /// <param name="entity">实体</param>
    public void Delete(T entity)
    {
      using (var transaction = Session.BeginTransaction())
      {
        Session.Delete(entity);
        transaction.Commit();
      }
    }

    /// <summary>
    /// 根据查询条件批量删除
    /// </summary>
    /// <param name="predicate">查询条件</param>
    public void Delete(Expression<Func<T, bool>> predicate)
    {
      using (var transaction = Session.BeginTransaction())
      {
       
        var dd = this.Fetch(predicate);
        dd.Delete();
        transaction.Commit();
      }
    }

    /// <summary>
    /// 实体集合
    /// </summary>
    public IQueryable<T> Table
    {
      get { return Session.Query<T>(); }
    }

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <returns>IQueryable类型的实体集合</returns>
    public IQueryable<T> Fetch(Expression<Func<T, bool>> predicate)
    {
      return predicate == null ? Table : Table.Where(predicate);
    }

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <param name="order">排序</param>
    /// <returns>IQueryable类型的实体集合</returns>
    public IQueryable<T> Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order)
    {
      if (order == null)
      {
        return Fetch(predicate);
      }

      var orderable = new Orderable<T>(Fetch(predicate));
      order(orderable);
      return orderable.Queryable;
    }

    /// <summary>
    /// 查询
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <param name="order">排序</param>
    /// <param name="topNumber">要获取的数目</param>
    /// <returns>IQueryable类型的实体集合</returns>
    public IQueryable<T> Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order, int topNumber)
    {
      return Fetch(predicate, order).Take(topNumber);
    }

    /// <summary>
    /// 分页查询
    /// </summary>
    /// <param name="predicate">查询条件</param>
    /// <param name="order">排序</param>
    /// <param name="pageSize">每页条数</param>
    /// <param name="pageIndex">第几页</param>
    public PagingDataSet<T> GetPagingData(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order, int pageSize, int pageIndex)
    {
      var ts = Fetch(predicate, order);

      var totalCount = ts.ToFuture<T>().Count();
      var pagingTs = ts.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToFuture();

      return new PagingDataSet<T>(pagingTs.ToList()) { PageIndex = pageIndex, PageSize = pageSize, TotalRecords = totalCount };
    }

    /// <summary>
    /// 根据规约进行分页查询
    /// </summary>
    /// <param name="specification"></param>
    /// <param name="order"></param>
    /// <param name="pageSize"></param>
    /// <param name="pageIndex"></param>
    /// <returns></returns>
    public PagingDataSet<T> GetPagingDataBySpecification(ISpecification<T> specification, Action<Orderable<T>> order, int pageSize, int pageIndex)
    {
      return GetPagingData(specification.GetExpression(), order, pageSize, pageIndex);
    }
  }
}