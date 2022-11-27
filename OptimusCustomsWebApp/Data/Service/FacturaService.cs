﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using OptimusCustomsWebApp.Interface;
using OptimusCustomsWebApp.Model;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.IO;

namespace OptimusCustomsWebApp.Data.Service
{
    public class FacturaService : IFactura
    {
        private readonly HttpClient httpClient;

        public bool IsBusy { get; set; }

        public FacturaService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<FacturaModel>> GetFacturas(DateTime fromdate, DateTime toDate)
        {
            string endpoint = "http://localhost:43248/Factura?" + "fromDate=" + fromdate.ToString("yyyy-MM-dd") + "&" + "toDate=" + toDate.ToString("yyyy-MM-dd");
            var list = await httpClient.GetFromJsonAsync<List<FacturaModel>>(endpoint);
            return list;
        }

        public async Task<FacturaModel> GetFactura(int idFactura)
        {
            var result = await httpClient.GetFromJsonAsync<FacturaModel>("http://localhost:43248/Factura/" + idFactura);
            return result;
        }

        public async Task<HttpResponseMessage> CreateFactura(FacturaModel model)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.Processing);
            try
            {
                string stringPayload = JsonConvert.SerializeObject(model);
                var buffer = System.Text.Encoding.UTF8.GetBytes(stringPayload);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                result = await httpClient.PostAsync("http://localhost:43248/Factura", byteContent);

            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public Task<HttpResponseMessage> UpdateFactura(FacturaModel model)
        {
            throw new NotImplementedException();
        }

        public async Task<HttpResponseMessage> DeleteFactura(int id)
        {
            HttpResponseMessage result = new HttpResponseMessage(System.Net.HttpStatusCode.Processing);
            try
            {
                result = await httpClient.DeleteAsync("http://localhost:43248/Factura/" + id.ToString());
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public async Task<Stream> GetFacturaPdf(int idFactura)
        {
            string endpoint = "http://localhost:43248/Factura/pdf?idFactura=" + idFactura;
            var response = await httpClient.GetAsync(endpoint, HttpCompletionOption.ResponseHeadersRead);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Content is object)
                {
                    var contentStream = await response.Content.ReadAsStreamAsync();

                    using var streamReader = new StreamReader(contentStream);
                    using var jsonReader = new JsonTextReader(streamReader);

                    JsonSerializer serializer = new JsonSerializer();

                    try
                    {
                        return serializer.Deserialize<Stream>(jsonReader);
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
    }
}
