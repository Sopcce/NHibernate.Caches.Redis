using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MvcDemo.MySQL.Database
{
  /// <summary>
  /// 分页数据封装
  /// </summary>
  /// <typeparam name="T">分页数据的实体类型</typeparam>
  public class PagingDataSet<T> : ReadOnlyCollection<T>
  {
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="entities">用于分页的实体集合</param>
    public PagingDataSet(IEnumerable<T> entities)
        : base(entities.ToList())
    {
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="entities">用于分页的实体集合</param>
    public PagingDataSet(IList<T> entities)
        : base(entities)
    {
    }

    /// <summary>
    /// 每页显示记录数
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 当前页数
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 总记录数
    /// </summary>
    public long TotalRecords { get; set; } = 0;

    /// <summary>
    /// 页数
    /// </summary>
    public int PageCount
    {
      get
      {
        long result = TotalRecords / PageSize;
        if (TotalRecords % PageSize != 0)
          result++;

        return Convert.ToInt32(result);
      }
    }

    /// <summary>
    /// 搜索执行时间(秒)
    /// </summary>
    public double QueryDuration { get; set; } = 0;

    public object Select(object p)
    {
      throw new NotImplementedException();
    }
  }
}