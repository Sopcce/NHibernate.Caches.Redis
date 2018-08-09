using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace Demo.MySQL.Models
{
 
  public class TestInfoMapping : ClassMapping<TestInfo>
  {
    public TestInfoMapping()
    {
      Table("item_testinfo");
      Cache(map => map.Usage(CacheUsage.ReadWrite));
      Id(t => t.Id, map => map.Generator(Generators.Native));
      Property(t => t.Type);
      Property(t => t.Body);
      Property(t => t.DateCreated);
      Property(t => t.DecimalValue);
      Property(t => t.FloatValue);
      Property(t => t.IsDel);
      Property(t => t.LongValue);
      Property(t => t.Status);
   

    }
  }
}
