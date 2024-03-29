﻿using Microsoft.AspNetCore.WebUtilities;
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
using System.Threading.Tasks;

namespace OptimusCustomsWebApp.Data.Service
{
    public class OperacionService : IOperacion
    {
        private readonly HttpClient httpClient;
        public bool IsBusy { get; set; }
        public OperacionService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> CreateOperacion(OperacionModel model)
        {
            string stringPayload = JsonConvert.SerializeObject(model);
            var buffer = System.Text.Encoding.UTF8.GetBytes(stringPayload);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            string endpoint = "http://localhost:43248/Operacion";
            var response = await httpClient.PostAsync(endpoint, byteContent);

            if (response != null && response.IsSuccessStatusCode)
                return response;
            else
                return null;
        }

        public async Task<HttpResponseMessage> UpdateOperacion(OperacionModel model)
        {
            string payload = JsonConvert.SerializeObject(model);
            var buffer = System.Text.Encoding.UTF8.GetBytes(payload);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            string endpoint = "http://localhost:43248/Operacion";
            var response = await httpClient.PutAsync(endpoint, byteContent);

            if (response != null && response.IsSuccessStatusCode)
                return response;
            else
                return null;
        }

        public async Task<HttpResponseMessage> DeleteOperacion(int id)
        {
            HttpResponseMessage result = new HttpResponseMessage(System.Net.HttpStatusCode.Processing);
            try
            {
                result = await httpClient.DeleteAsync("http://localhost:43248/Operacion/" + id.ToString());
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public async Task<List<OperacionModel>> GetOperaciones(Dictionary<string, string> query)
        {
            string endpoint = QueryHelpers.AddQueryString("http://localhost:43248/Operacion", query);
            var response = await httpClient.GetAsync(endpoint, HttpCompletionOption.ResponseHeadersRead);

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
                        return serializer.Deserialize<List<OperacionModel>>(jsonReader);
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

        public async Task<OperacionModel> GetOperacion(int id)
        {
            string endpoint = "http://localhost:43248/Operacion/" + id;
            var response = await httpClient.GetAsync(endpoint, HttpCompletionOption.ResponseHeadersRead);

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
                        return serializer.Deserialize<OperacionModel>(jsonReader);
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

        public async Task<OperacionModel> ValidateOperacion(string operacion)
        {
            string endpoint = "http://localhost:43248/Operacion/validate?numOperacion=" + operacion;
            var response = await httpClient.GetAsync(endpoint, HttpCompletionOption.ResponseHeadersRead);

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
                        return serializer.Deserialize<OperacionModel>(jsonReader);
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

        public async Task<HttpResponseMessage> CreateDocumento(DocumentoModel model)
        {
            string stringPayload = JsonConvert.SerializeObject(model);
            var buffer = System.Text.Encoding.UTF8.GetBytes(stringPayload);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            string endpoint = "http://localhost:43248/Operacion/document";
            var response = await httpClient.PostAsync(endpoint, byteContent);

            if (response != null && response.IsSuccessStatusCode)
                return response;
            else
                return null;
        }

        public async Task<HttpResponseMessage> UpdateDocumento(DocumentoModel model)
        {
            string payload = JsonConvert.SerializeObject(model);
            var buffer = System.Text.Encoding.UTF8.GetBytes(payload);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            string endpoint = "http://localhost:43248/Operacion/document";
            var response = await httpClient.PutAsync(endpoint, byteContent);

            if (response != null && response.IsSuccessStatusCode)
                return response;
            else
                return null;
        }
    }
}
