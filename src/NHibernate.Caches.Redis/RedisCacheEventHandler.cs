using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHibernate.Caches.Redis
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <typeparam name="TEventArgs"></typeparam>
    public delegate void RedisCacheEventHandler<TEventArgs>(RedisCache sender, TEventArgs e);
}
