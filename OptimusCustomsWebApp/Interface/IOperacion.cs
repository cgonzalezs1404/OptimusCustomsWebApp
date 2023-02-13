using OptimusCustomsWebApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OptimusCustomsWebApp.Interface
{
    public interface IOperacion
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        Task<List<OperacionModel>> GetOperaciones(Dictionary<string, string> query);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> CreateOperacion(OperacionModel model);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> UpdateOperacion(OperacionModel model);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idFactura"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> DeleteOperacion(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> CreateDocumento(DocumentoModel model);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> UpdateDocumento(DocumentoModel model);
    }
}
