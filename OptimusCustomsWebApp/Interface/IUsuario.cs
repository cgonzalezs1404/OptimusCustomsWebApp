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
    public interface IUsuario
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<UsuarioModel>> GetUsuarios(int? idTipoUsuario, string? RFC);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<UsuarioModel> LoginUsuario(string username, string password);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        Task<UsuarioModel> GetUsuario(int id);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> CreateUsuario(UsuarioModel model);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> UpdateUsuario(UsuarioModel model);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> DeleteUsuario(int idCliente);
    }
}
