using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using OptimusCustomsWebApp.Data;
using OptimusCustomsWebApp.Data.Service;
using OptimusCustomsWebApp.Model;
using OptimusCustomsWebApp.Model.Enum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace OptimusCustomsWebApp.Views
{
    public partial class FacturaForm : ComponentBase
    {
        /// <summary>
        /// 
        /// </summary>
        [Inject]
        public NavigationManager NavManager { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Inject]
        private IWebHostEnvironment Environment { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Inject]
        private NavigationQueryService QueryService { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public FacturaModel Model { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public OperacionModel OpModel { get; set; }
        /// <summary>
        /// 
        /// </summary>

        [Inject]
        public FacturaService Service { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Inject]
        public OperacionService OperacionService { get; set; }
        /// <summary>
        /// 
        /// </summary>

        [Parameter]
        public string IdFactura { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NumOp { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public RenderFragment DynamicFragment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? IsOperationValid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool FileSelected { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected async override Task OnInitializedAsync()
        {
            Service.IsBusy = true;

            //Verificacion de url factura/redirect para tomar parametros de la barra de navegacion.
            var uri = NavManager.ToAbsoluteUri(NavManager.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("idFactura", out var idFactura)
                && QueryHelpers.ParseQuery(uri.Query).TryGetValue("idOperacion", out var idOperacion))
            {

                //Si la operacion y la factura son distintos a 0, significa que es una edicion de factura desde /operacion
                if (!idFactura.Equals("0") && !idOperacion.Equals("0"))
                {
                    Model = await Service.GetFactura(int.Parse(idFactura));
                    OpModel = await OperacionService.GetOperacion(int.Parse(idOperacion));
                    NumOp = OpModel.NumOperacion;
                    await OnValidateOperacion();
                    FileSelected = true;
                }
                //Si la operacion es distinta a 0 y la factura es igual a 0, significa que es una creacion de factura desde /operacion
                if (idFactura.Equals("0") && !idOperacion.Equals("0"))
                {
                    Model = new FacturaModel();
                    OpModel = await OperacionService.GetOperacion(int.Parse(idOperacion));
                    NumOp = OpModel.NumOperacion;
                    await OnValidateOperacion();
                    FileSelected = false;
                }
            }
            //Accion basica desde factura/crear
            else
            {
                await Task.Factory.StartNew(() =>
                {
                    Model = new FacturaModel();
                    NumOp = "";
                    FileSelected = false;
                });
            }


            Service.IsBusy = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected async override Task OnParametersSetAsync()
        {
            Service.IsBusy = true;
            if (IdFactura != null && !IdFactura.Equals(""))
            {
                Model = await Service.GetFactura(int.Parse(IdFactura));
                if (Model != null && Model.IdFactura != 0)
                {
                    OpModel = await OperacionService.GetOperacion(Model.IdOperacion);
                    NumOp = OpModel.NumOperacion;
                    await OnValidateOperacion();
                    FileSelected = true;
                }
            }
            Service.IsBusy = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected async Task OnCreate()
        {
            Service.IsBusy = true;
            var responseF = await Service.CreateFactura(Model);

            if (responseF.StatusCode == HttpStatusCode.OK)
            {
                if (responseF.Content is object && responseF.Content.Headers.ContentType.MediaType == "application/json")
                {
                    var contentStream = await responseF.Content.ReadAsStreamAsync();
                    using var streamReader = new StreamReader(contentStream);
                    using var jsonReader = new JsonTextReader(streamReader);
                    JsonSerializer serializer = new JsonSerializer();
                    Model = serializer.Deserialize<FacturaModel>(jsonReader);
                    OpModel.IdFactura = Model.IdFactura;

                    var responseO = await OperacionService.UpdateOperacion(OpModel);
                    if (responseO.StatusCode == HttpStatusCode.OK)
                    {
                        NavManager.NavigateTo("/factura");
                    }
                }
            }
            Service.IsBusy = false;
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnCancel()
        {
            if (NavManager.Uri.Contains("redirect"))
            {
                if (QueryService.GetQueryString(TipoPagina.Operacion) != null)
                    NavManager.NavigateTo(QueryHelpers.AddQueryString("https://localhost:44307/operacion", QueryService.GetQueryString(TipoPagina.Operacion)));
                else
                    NavManager.NavigateTo("/operacion");
            }
            else
            {
                if (QueryService.GetQueryString(TipoPagina.Factura) != null)
                    NavManager.NavigateTo(QueryHelpers.AddQueryString("https://localhost:44307/factura", QueryService.GetQueryString(TipoPagina.Factura)));
                else
                    NavManager.NavigateTo("/factura");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected async Task OnUpdate()
        {
            Service.IsBusy = true;
            var response = await Service.UpdateFactura(Model);
            if (response.IsSuccessStatusCode)
            {
                if (NavManager.Uri.Contains("redirect"))
                {
                    if (QueryService.GetQueryString(TipoPagina.Operacion) != null)
                        NavManager.NavigateTo(QueryHelpers.AddQueryString("https://localhost:44307/operacion", QueryService.GetQueryString(TipoPagina.Operacion)));
                    else
                        NavManager.NavigateTo("/operacion");
                }
                else
                {
                    if (QueryService.GetQueryString(TipoPagina.Factura) != null)
                        NavManager.NavigateTo(QueryHelpers.AddQueryString("https://localhost:44307/factura", QueryService.GetQueryString(TipoPagina.Factura)));
                    else
                        NavManager.NavigateTo("/factura");
                }
            }
            Service.IsBusy = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task OnValidateOperacion()
        {
            Service.IsBusy = true;
            if (!NumOp.Equals(""))
            {
                var response = await OperacionService.ValidateOperacion(NumOp);
                if (response != null && !response.NumOperacion.Equals(""))
                {
                    OpModel = response;
                    IsOperationValid = true;
                }
                else
                {
                    IsOperationValid = false;
                }
            }
            else
            {
                IsOperationValid = false;
            }
            DynamicFragment = RenderComponent();
            ValidationClass();
            Service.IsBusy = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private async Task OnInputFileChangeAsync(InputFileChangeEventArgs e)
        {

            try
            {
                var file = e.File;
                Stream stream = file.OpenReadStream();
                var path = Path.Combine(Environment.WebRootPath, "upload", file.Name);
                FileStream fs = File.Create(path);
                await stream.CopyToAsync(fs);
                stream.Close();
                fs.Close();

                byte[] bytes = File.ReadAllBytes(path);

                if (file.ContentType.Equals("text/xml"))
                {
                    Model = await CargaFactura(file);
                    Model.FileXml = bytes;
                }
                if (file.ContentType.Equals("application/pdf"))
                {
                    Model.FilePdf = bytes;
                }

                FileSelected = true;
            }
            catch (Exception ex)
            {

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private async Task<FacturaModel> CargaFactura(IBrowserFile file)
        {
            Service.IsBusy = true;
            FacturaModel result = Model;
            try
            {
                XDocument xmlDocument = new XDocument();
                using (Stream stream = file.OpenReadStream())
                {
                    XmlReaderSettings settings = new XmlReaderSettings { Async = true };
                    using (XmlReader xmlReader = XmlReader.Create(stream, settings))
                    {
                        xmlDocument = await XDocument.LoadAsync(xmlReader, LoadOptions.None, CancellationToken.None);
                        result = new FacturaModel();
                        result.IdEstadoFactura = 1;
                        result.EsAprobado = false;
                        result.FilePdf = null;
                        result.FileXml = null;

                        foreach (XElement node in xmlDocument.Descendants())
                        {
                            switch (node.Name.LocalName)
                            {
                                case "Comprobante":
                                    result.FechaEmision = DateTime.Parse(node.Attribute("Fecha").Value);
                                    result.Total = Double.Parse(node.Attribute("Total").Value);
                                    result.Folio = node.Attribute("Folio").Value;
                                    result.Serie = node.Attribute("Serie") is null ? "" : node.Attribute("Serie").Value;
                                    break;
                                case "Emisor":
                                    if (node.Attribute("Rfc").Value.Equals("OCU180309119")) { result.IdTipoFactura = 1; }
                                    if (!node.Attribute("Rfc").Value.Equals("OCU180309119")) { result.IdTipoFactura = 2; }
                                    result.RazonSocial = node.Attribute("Nombre").Value;
                                    result.RFC = node.Attribute("Rfc").Value;
                                    break;
                                case "Conceptos":
                                    foreach (XElement xElement in node.Descendants())
                                    {
                                        if (xElement.Name.LocalName.Equals("Concepto"))
                                        {
                                            result.Descripcion = xElement.Attribute("Descripcion").Value;
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {

            }
            Service.IsBusy = false;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private RenderFragment RenderComponent() => builder =>
        {
            if (!NumOp.Equals(""))
            {
                if (IsOperationValid.Value)
                {
                    builder.OpenElement(1, "label");
                    builder.AddAttribute(2, "class", "text-success col-form-label col-md-2");
                    builder.AddContent(3, "Número de operación válido");
                    builder.CloseElement();
                }
                else
                {
                    builder.OpenElement(1, "label");
                    builder.AddAttribute(2, "class", "text-danger col-form-label col-md-2");
                    builder.AddContent(3, "Número de operación inválido");
                    builder.CloseElement();
                }
            }
            else
            {
                builder.OpenElement(1, "label");
                builder.AddAttribute(2, "class", "text-danger col-form-label col-md-2");
                builder.AddContent(3, "Ingrese un número de operación");
                builder.CloseElement();
            }
        };

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string ValidationClass()
        {
            if (!IsOperationValid != null)
            {
                if (!NumOp.Equals(""))
                {
                    if (IsOperationValid.Value)
                    {
                        return "form-control is-valid";
                    }
                    else
                    {
                        return "form-control is-invalid";
                    }
                }
                else
                {
                    return "form-control is-invalid";
                }
            }
            else
            {
                return "form-control";
            }

        }

    }
}
