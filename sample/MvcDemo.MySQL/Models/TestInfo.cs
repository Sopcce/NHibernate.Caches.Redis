using System;

namespace MvcDemo.MySQL.Models
{
  /// <summary>
  /// 标签
  /// </summary>
  public class TestInfo
  {
    /// <summary>
    /// 主键
    /// </summary>
    public virtual long Id { get; set; }


    public virtual int Type { get; set; }


    public virtual bool IsDel { get; set; }


    public virtual int Status { get; set; }


    public virtual long LongValue { get; set; }
    public virtual float FloatValue { get; set; }


    public virtual decimal DecimalValue { get; set; }

    public virtual string Body { get; set; }
    public virtual DateTime DateCreated { get; set; }

  }
}
