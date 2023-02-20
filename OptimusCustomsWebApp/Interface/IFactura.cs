using OptimusCustomsWebApp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OptimusCustomsWebApp.Interface
{
    /// <summary>
    /// 
    /// </summary>
    public interface IFactura : IServiceBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        Task<List<FacturaModel>> GetFacturas(Dictionary<string, string> query);
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idFactura"></param>
        /// <returns></returns>
        Task<Stream> GetFacturaPdf(int idFactura);
    }
}
