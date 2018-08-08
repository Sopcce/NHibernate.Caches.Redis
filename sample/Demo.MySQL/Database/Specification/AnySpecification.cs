using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Demo.MySQL.Database.Specification;

namespace HuobanYun.Specification
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
