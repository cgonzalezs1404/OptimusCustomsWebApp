using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OptimusCustomsWebApp.Interface;
using OptimusCustomsWebApp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace OptimusCustomsWebApp.Data.Service
{
    /// <summary>
    /// 
    /// </summary>
    public class UsuarioService : IUsuario
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly HttpClient httpClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient"></param>
        public UsuarioService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public bool IsBusy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> CreateUsuario(UsuarioModel model)
        {
            HttpResponseMessage result = new HttpResponseMessage(System.Net.HttpStatusCode.Processing);
            try
            {
                string stringPayload = JsonConvert.SerializeObject(model);
                var buffer = System.Text.Encoding.UTF8.GetBytes(stringPayload);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                result = await httpClient.PostAsync("http://localhost:43248/Usuario", byteContent);
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> DeleteUsuario(int id)
        {
            HttpResponseMessage result = new HttpResponseMessage(System.Net.HttpStatusCode.Processing);
            try
            {
                result = await httpClient.DeleteAsync("http://localhost:43248/Usuario/" + id.ToString());
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idUsuario"></param>
        /// <returns></returns>
        public async Task<UsuarioModel> GetUsuario(int id)
        {
            UsuarioModel result = null;
            try
            {
                result = await httpClient.GetFromJsonAsync<UsuarioModel>("http://localhost:43248/Usuario/" + id);
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<UsuarioModel>> GetUsuarios(int? idTipoUsuario, string? RFC)
        {
            List<UsuarioModel> result = null;
            try
            {
                string endpoint = "http://localhost:43248/Usuario";
                result = await httpClient.GetFromJsonAsync<List<UsuarioModel>>(endpoint);

            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public async Task<UsuarioModel> LoginUsuario(string username, string password)
        {
            string endpoint = "http://localhost:43248/Usuario/login?UserName=" + username + "&Paswword=" + password;
            var response = await httpClient.GetAsync(endpoint, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Content is object && response.Content.Headers.ContentType.MediaType == "application/json")
                {
                    var contentStream = await response.Content.ReadAsStreamAsync();

                    using var streamReader = new StreamReader(contentStream);
                    using var jsonReader = new JsonTextReader(streamReader);

                    JsonSerializer serializer = new JsonSerializer();

                    try
                    {
                        return serializer.Deserialize<UsuarioModel>(jsonReader);
                    }
                    catch (JsonReaderException)
                    {
                        Console.WriteLine("Invalid JSON.");
                    }
                }
                else
                {
                    Console.WriteLine("HTTP Response was invalid and cannot be deserialised.");
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> UpdateUsuario(UsuarioModel model)
        {
            HttpResponseMessage result = new HttpResponseMessage(System.Net.HttpStatusCode.Processing);
            try
            {
                string stringPayload = JsonConvert.SerializeObject(model);
                var buffer = System.Text.Encoding.UTF8.GetBytes(stringPayload);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                result = await httpClient.PutAsync("http://localhost:43248/Usuario", byteContent);
            }
            catch (Exception ex)
            {

            }
            return result;
        }
    }
}
