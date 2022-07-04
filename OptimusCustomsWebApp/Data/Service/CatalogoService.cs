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

    }
}
