using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Cache.DynamicCacheBuster
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="oldCacheRegionName"></param>
    /// <param name="newCacheRegionName"></param>
    /// <param name="version"></param>
    public delegate void ChangeEventHandler(string oldCacheRegionName, string newCacheRegionName, string version);
}
