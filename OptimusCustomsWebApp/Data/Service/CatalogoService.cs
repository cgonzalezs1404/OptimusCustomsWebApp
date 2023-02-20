using OptimusCustomsWebApp.Interface;
using OptimusCustomsWebApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace OptimusCustomsWebApp.Data.Service
{
    public class CatalogoService : ICatalogo
    {
        private readonly HttpClient httpClient;

        public CatalogoService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<CatalogoModel>> GetTipoUsuario()
        {
            var list = await httpClient.GetFromJsonAsync<List<CatalogoModel>>("http://localhost:43248/Catalogos/1");
            return list;
        }

        public async Task<List<CatalogoModel>> GetEstadoFactura()
        {
            var list = await httpClient.GetFromJsonAsync<List<CatalogoModel>>("http://localhost:43248/Catalogos/2");
            return list;
        }

        public async Task<List<CatalogoModel>> GetTipoFactura()
        {
            var list = await httpClient.GetFromJsonAsync<List<CatalogoModel>>("http://localhost:43248/Catalogos/3");
            return list;
        }

        public async Task<List<CatalogoModel>> GetTipoOperacion()
        {
            var list = await httpClient.GetFromJsonAsync<List<CatalogoModel>>("http://localhost:43248/Catalogos/4");
            return list;
        }

        public async Task<List<CatalogoModel>> GetUsuarios()
        {
            var list = await httpClient.GetFromJsonAsync<List<CatalogoModel>>("http://localhost:43248/Catalogos/5");
            return list;
        }

        public async Task<List<CatalogoModel>> GetTipoDocumento()
        {
            var list = await httpClient.GetFromJsonAsync<List<CatalogoModel>>("http://localhost:43248/Catalogos/6");
            return list;
        }

    }
}
