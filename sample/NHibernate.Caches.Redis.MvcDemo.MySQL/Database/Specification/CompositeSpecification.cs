using HuobanYun.Specification;

namespace Demo.MySQL.Database.Specification
{
  public interface ICompositeSpecification<T> : ISpecification<T>
  {
    /// <summary>
    /// Gets the left side of the specification.
    /// </summary>
    ISpecification<T> Left { get; }
    /// <summary>
    /// Gets the right side of the specification.
    /// </summary>
    ISpecification<T> Right { get; }
  }

  public abstract class CompositeSpecification<T> : SpecificationBase<T>, ICompositeSpecification<T>
  {
    protected CompositeSpecification(ISpecification<T> left, ISpecification<T> right)
    {
      this.Left = left;
      this.Right = right;
    }

    public ISpecification<T> Left { get; }

    public ISpecification<T> Right { get; }
  }
}
