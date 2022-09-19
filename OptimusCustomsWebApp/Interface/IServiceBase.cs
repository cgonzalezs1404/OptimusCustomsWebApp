using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptimusCustomsWebApp.Interface
{
    public interface IServiceBase
    {
        bool IsBusy { get; set; }
    }
}
