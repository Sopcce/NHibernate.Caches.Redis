using System.Collections.Generic;
using System.Linq;
using MvcDemo.MySQL.Database;

namespace MvcDemo.MySQL.Models
{

  public class TestService
  {
    public IRepository<TestInfo> _repository { get; set; }


    public void Create(TestInfo log)
    {
      _repository.Create(log);
    }


    public void Delete(long id)
    {
      _repository.Delete(log => log.Id == id);
    }

    public void Update(TestInfo log)
    {
      _repository.Update(log);
    }
    public List<TestInfo> GetList()
    {
      return _repository.Table.ToList();
    }

    /// <summary>
    /// 获取拥有者的操作日志
    /// </summary>
    /// <param name="Id"></param>
    /// <param name="pageSize">分页大小</param>
    /// <param name="pageIndex">页码</param>
    /// <returns></returns>
    public PagingDataSet<TestInfo> Gets(int pageSize, int pageIndex)
    {
      var data = _repository.Table;

      return new PagingDataSet<TestInfo>(data.Skip(pageSize * (pageIndex - 1)).Take(pageSize))
      {
        PageSize = pageSize,
        PageIndex = pageIndex,
        TotalRecords = data.LongCount()
      };
    }
  }
}
