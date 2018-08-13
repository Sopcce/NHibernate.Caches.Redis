using System;
using System.Linq.Expressions;

namespace MvcDemo.MySQL.Database.Specification
{
    public class AndSpecification<T> : CompositeSpecification<T>
    {
        public AndSpecification(ISpecification<T> left, ISpecification<T> right) : base(left, right) { }

        public override Expression<Func<T, bool>> GetExpression()
        {
            /* 在NHibernate 中，这行代码报错：无法将类型为“NHibernate.Hql.Ast.HqlBitwiseAnd”的
             * 对象强制转换为类型“NHibernate.Hql.Ast.HqlBooleanExpression”。
             * 在此处找到解决方法：http://www.cnblogs.com/hyl8218/archive/2013/03/12/2955074.html
             * 在 ExpressionFuncExtender 扩展类中，添加了 AndAlso方法，表示把两个表达式按照 AndAlso 的方式进行组合
             */
            //return Left.GetExpression().And(Right.GetExpression());
           
            return Left.GetExpression().AndAlso(Right.GetExpression());
        }
    }
}
