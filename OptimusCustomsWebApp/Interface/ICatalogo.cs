using OptimusCustomsWebApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptimusCustomsWebApp.Interface
{
    public interface ICatalogo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<CatalogoModel>> GetTipoUsuario();
    }
}
