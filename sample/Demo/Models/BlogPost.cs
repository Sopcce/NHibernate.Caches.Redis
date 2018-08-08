using FluentNHibernate.Mapping;
using System;

namespace Demo.Models
{
  public  class BlogPost
  {
    public virtual int Id { get; set; }
    public virtual string Title { get; set; }
    public virtual string Body { get; set; }
    public virtual DateTime Created { get; set; }






  }
  public class BlogPostMapping : ClassMap<BlogPost>
  {
    public BlogPostMapping()
    {
      Id(x => x.Id);

      Map(x => x.Title);
      Map(x => x.Body);
      Map(x => x.Created);

      Cache.ReadWrite();
    }
  }

}