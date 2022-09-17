﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using OptimusCustomsWebApp.Data;
using OptimusCustomsWebApp.Data.Service;
using OptimusCustomsWebApp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace OptimusCustomsWebApp.Views
{
    public partial class FacturaForm : ComponentBase
    {
        [Inject]
        public NavigationManager Navigation { get; set; }

        [Inject]
        private IWebHostEnvironment Environment { get; set; }
        public FacturaModel Model { get; set; }
        public OperacionModel OpModel { get; set; }

        [Inject]
        public FacturaService Service { get; set; }
        [Inject]
        public OperacionService OperacionService { get; set; }

        [Parameter]
        public string IdFactura { get; set; }
        public string NumOp { get; set; }
        public RenderFragment DynamicFragment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? IsOperationValid { get; set; }
        public bool FileSelected { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await Task.Factory.StartNew(() =>
            {
                Model = new FacturaModel();
                NumOp = "";
            });
        }

        protected async override Task OnParametersSetAsync()
        {
            if (IdFactura != null && !IdFactura.Equals(""))
            {
                Model = await Service.GetFactura(int.Parse(IdFactura));
                if (Model != null && Model.IdFactura != 0)
                {
                    OpModel = await OperacionService.GetOperacion(Model.IdOperacion);
                }
            }

        }

        protected async Task OnCreate()
        {
            var response = await Service.CreateFactura(Model);
            if (response.IsSuccessStatusCode)
            {
                Navigation.NavigateTo("/factura");
            }
        }

        protected async Task OnCancel()
        {
            await Task.Factory.StartNew(() =>
            {
                Navigation.NavigateTo("/factura");
            });
        }

        protected async Task OnUpdate()
        {
            var response = await Service.UpdateFactura(Model);
            if (response.IsSuccessStatusCode)
            {
                Navigation.NavigateTo("/factura");
            }
        }

        private async Task OnValidateOperacion()
        {

            if (!NumOp.Equals(""))
            {
                var response = await OperacionService.ValidateOperacion(NumOp);
                if (response != null && !response.NumOperacion.Equals(""))
                {
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

        }

        private async Task OnInputFileChangeAsync(InputFileChangeEventArgs e)
        {
            try
            {
                var files = e.GetMultipleFiles(10);
                foreach (var file in files)
                {
                    using (var stream = file.OpenReadStream())
                    {
                        if (file.ContentType.Equals("text/xml"))
                        {
                            Model = await CargaFactura(file);
                        }
                    }

                    var path = Path.Combine(Environment.ContentRootPath, "wwwroot", "upload", file.Name);
                    byte[] fs = File.ReadAllBytes(path);
                    FileModel fm = new FileModel
                    {
                        FileStream = fs,
                        FileSize = fs.Length
                    };
                    Model.FileXml = fm.FileStream;
                    Model.FilePdf = new byte[0];
                    FileSelected = true;


                }
            }
            catch (Exception ex)
            {

            }
        }

        private async Task<FacturaModel> CargaFactura(IBrowserFile file)
        {
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
                    builder.AddAttribute(2, "class", "text-success");
                    builder.AddContent(3, "Número de operación válido");
                    builder.CloseElement();
                }
                else
                {
                    builder.OpenElement(1, "label");
                    builder.AddAttribute(2, "class", "text-danger");
                    builder.AddContent(3, "Número de operación inválido");
                    builder.CloseElement();
                }
            }
            else
            {
                builder.OpenElement(1, "label");
                builder.AddAttribute(2, "class", "text-danger");
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
