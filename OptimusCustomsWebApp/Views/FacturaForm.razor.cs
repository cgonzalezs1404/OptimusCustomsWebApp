using Microsoft.AspNetCore.Components;
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

        [Inject]
        public FacturaService Service { get; set; }

        [Parameter]
        public string IdFactura { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await Task.Factory.StartNew(() =>
            {
                Model = new FacturaModel();
            });
        }

        protected async override Task OnParametersSetAsync()
        {
            if (IdFactura != null && !IdFactura.Equals(""))
            {
                Model = await Service.GetFactura(int.Parse(IdFactura));
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



                }
            }
            catch (Exception ex)
            {

            }
        }

        private async Task<FacturaModel> CargaFactura(IBrowserFile file)
        {
            FacturaModel result = null;
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
                                    result.Serie = node.Attribute("Serie").Value;
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


    }
}
