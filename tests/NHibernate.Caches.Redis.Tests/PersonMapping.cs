using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;

namespace NHibernate.Caches.Redis.Tests
{
    /// <summary>
    /// 
    /// </summary>
    public class PersonMapping : ClassMapping<Person>
    {
        /// <summary>
        /// 
        /// </summary>
        public PersonMapping()
        {
            Table("Person");
            Cache(map => map.Usage(CacheUsage.ReadWrite));
            Id(x => x.Id, map => map.Generator(Generators.Native));
            Property(x => x.Age);
            Property(x => x.Name); 
           
        }
    }
    
}
