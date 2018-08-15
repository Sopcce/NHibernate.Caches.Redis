using System.Collections.Generic;
using System.Linq;
using MvcDemo.MySQL.Database;
using MvcDemo.MySQL.Repositories;

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
      _repository.Dao.CreateSQLQuery()
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
    public List<TestInfo> Gets()
    {
      return _repository.Gets(n => n.Id < 2000 && n.Id > 700 && n.LongValue > 20, order => order.Asc(n => n.LongValue, n => n.Id), 20, 1);
    }

  }
}
