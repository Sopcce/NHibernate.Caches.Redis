using System;
using System.Linq;
using System.Linq.Expressions;
using Demo.MySQL.Database.Specification;
using HuobanYun.Database;
using NHibernate;

namespace Demo.MySQL.Database
{
    /// <summary>
    /// 仓储接口
    /// </summary>
    /// <typeparam name="T">仓储对应的实体</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// ISession实例
        /// </summary>
        ISession Session { get; }

        /// <summary>
        /// 根据Id查询实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T Get(object id);

        /// <summary>
        /// 根据指定的条件查询实体
        /// </summary>
        /// <param name="predicate">查询条件委托</param>
        /// <returns>实体</returns>
        T SingleOrDefault(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 根据指定的条件查询实体
        /// </summary>
        /// <param name="predicate">查询的条件委托</param>
        /// <param name="order">排序</param>
        /// <returns>实体</returns>
        T SingleOrDefault(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order);

        /// <summary>
        /// 查询指定实体的指定字段
        /// </summary>
        /// <typeparam name="TS">要查询的字段的类型</typeparam>
        /// <param name="predicate">查询条件</param>
        /// <param name="property">要查询的字段</param>
        /// <returns>指定的字段，如果没有查询结果，则为该字段类型的默认值。</returns>
        TS GetProperty<TS>(Expression<Func<T, bool>> predicate, Func<T, TS> property);

        /// <summary>
        /// 创建实体
        /// </summary>
        /// <param name="entity">实体</param>
        void Create(T entity);

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        void Update(T entity);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="entity">实体</param>
        void Delete(T entity);

        /// <summary>
        /// 根据查询条件批量删除
        /// </summary>
        /// <param name="predicate">查询条件</param>
        void Delete(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 实体集合
        /// </summary>
        IQueryable<T> Table { get; }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <returns>IQueryable类型的实体集合</returns>
        IQueryable<T> Fetch(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="order">排序</param>
        /// <returns>IQueryable类型的实体集合</returns>
        IQueryable<T> Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order);

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="order">排序</param>
        /// <param name="topNumber">要获取的数目</param>
        /// <returns>IQueryable类型的实体集合</returns>
        IQueryable<T> Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order, int topNumber);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="order">排序</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="pageIndex">第几页</param>
        PagingDataSet<T> GetPagingData(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order, int pageSize, int pageIndex);

        /// <summary>
        /// 根据规约进行分页查询
        /// </summary>
        /// <param name="specification"></param>
        /// <param name="order"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        PagingDataSet<T> GetPagingDataBySpecification(ISpecification<T> predicate, Action<Orderable<T>> order, int pageSize, int pageIndex);
    }
}