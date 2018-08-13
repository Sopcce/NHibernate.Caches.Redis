using System;

namespace MvcDemo.MySQL.Database.Specification
{
    /// <summary>
    /// 此规约描述：一个给定的对象永远都会满足的情况
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AnySpecification<T>:SpecificationBase<T>
    {
        public override System.Linq.Expressions.Expression<Func<T, bool>> GetExpression()
        {
            return o => true;
        }
    }
}
