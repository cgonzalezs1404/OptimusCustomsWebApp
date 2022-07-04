using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptimusCustomsWebApp.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public class SessionState
    {
        /// <summary>
        /// 
        /// </summary>
        public SessionState()
        {
            Items = new Dictionary<string, object>();
        }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, object> Items { get; set; }
    }

}
