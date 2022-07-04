using OptimusCustomsWebApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OptimusCustomsWebApp.Interface
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFactura
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        Task<List<FacturaModel>> GetFacturas(DateTime fromDate, DateTime toDate);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idFactura"></param>
        /// <returns></returns>
        Task<FacturaModel> GetFactura(int idCliente);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idFactura"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> CreateFactura(FacturaModel model);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> UpdateFactura(FacturaModel model);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> DeleteFactura(int idFactura);
    }
}
