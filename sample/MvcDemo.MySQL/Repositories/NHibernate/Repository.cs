﻿using System;
using System.Linq;
using System.Linq.Expressions;
using ItemDoc.Framework.Repositories;
using MvcDemo.MySQL.Database;
using NHibernate;
using NHibernate.Linq;

namespace MvcDemo.MySQL.Repositories.NHibernate
{
    /// <summary>
    /// 仓储基类
    /// </summary>
    /// <typeparam name="T">仓储对应的实体</typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        public Repository(SessionManager sessionManager)
        {
            SessionManager = sessionManager;
        }

        private SessionManager SessionManager { get; set; }
        /// <summary>
        /// ISession实例
        /// </summary>
        private ISession Session => SessionManager.Session;
        public ICreateDao Dao => (ICreateDao)Session;


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

                var dd = this.Table.Where(predicate);
                QueryableExtensions.Delete(dd);
                transaction.Commit();
            }
        }

        /// <summary>
        /// 实体集合
        /// </summary>
        public IQueryable<T> Table => Session.Query<T>();


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
                return Table.Where(predicate);
            }
            var orderable = new Orderable<T>(Table.Where(predicate));
            order(orderable);
            return orderable.Queryable;
        }


        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="predicate">查询条件</param>
        /// <param name="order">排序</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="pageIndex">第几页</param>
        public PageList<T> Gets(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order, int pageSize, int pageIndex)
        {
            var ts = Fetch(predicate, order);

            var totalCount = ts.ToFuture<T>().Count();
            var pagingTs = ts.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToFuture();

            return new PageList<T>(pagingTs.ToList(), pageIndex, pageSize);


        }


    }
}