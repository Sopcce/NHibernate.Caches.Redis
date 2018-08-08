using System;
using System.Linq.Expressions;
using HuobanYun.Specification;

namespace Demo.MySQL.Database.Specification
{
    /// <summary>
    /// 规约接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISpecification<T>
    {
        /// <summary>
        /// 判断一个对象是否满足当前的规约
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool IsSatisfiedBy(T obj);

        /// <summary>
        /// 把两个规约以“And”条件组合到一起
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        ISpecification<T> And(ISpecification<T> other);

        /// <summary>
        /// 获取代表当前规约的linq表达式
        /// </summary>
        /// <returns></returns>
        Expression<Func<T, bool>> GetExpression();
    }

    /// <summary>
    /// 规约抽象基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class SpecificationBase<T> : ISpecification<T>
    {
        #region ISpecification<T> 成员

        /// <summary>
        /// 判断一个对象是否满足当前的规约
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool IsSatisfiedBy(T obj)
        {
            return GetExpression().Compile()(obj);
        }

        /// <summary>
        /// 获取代表当前规约的linq表达式
        /// </summary>
        /// <returns></returns>
        public abstract Expression<Func<T, bool>> GetExpression();

        /// <summary>
        /// 把两个规约以“And”条件组合到一起
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public ISpecification<T> And(ISpecification<T> other)
        {
            return new AndSpecification<T>(this, other);
        }

        #endregion
    }

}
