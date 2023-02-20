using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using OptimusCustomsWebApp.Data.Service;
using OptimusCustomsWebApp.Model;
using OptimusCustomsWebApp.Model.Enum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OptimusCustomsWebApp.Views
{
    public partial class Operacion
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }
        [Inject]
        private IWebHostEnvironment Environment { get; set; }
        [Inject]
        private NavigationManager NavManager { get; set; }
        [Inject]
        private ServiceContainer ServiceContainer { get; set; }

        public List<OperacionModel> List;
        public OperacionModel SelectedOperacion { get; set; }
        public bool DeleteDialogOpen { get; set; }
        public bool UpFileDialogOpen { get; set; }
        public int Id { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public RenderFragment DynamicFragment { get; set; }
        public TipoDocumento TipoDocumento { get; set; }
        public IBrowserFile BrowserFile { get; set; }
        public DocumentoModel DocumentoUpload { get; set; }
        public SessionData Usuario { get; set; }
        public List<CatalogoModel> CatalogoTOperacion { get; set; }
        public List<CatalogoModel> CatalogoUsuarios { get; set; }
        public int SelectedOperacionId { get; set; }
        public int SelectedUsuarioId { get; set; }

        protected override async Task OnInitializedAsync()
        {

            ServiceContainer.IsBusy = true;
            Usuario = await ServiceContainer.UsuarioService.GetSessionData();
            CatalogoTOperacion = await ServiceContainer.CatalogoService.GetTipoOperacion();
            CatalogoUsuarios = await ServiceContainer.CatalogoService.GetUsuarios();

            var uri = NavManager.ToAbsoluteUri(NavManager.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("fromDate", out var fromDate)
                && QueryHelpers.ParseQuery(uri.Query).TryGetValue("toDate", out var toDate)
                && QueryHelpers.ParseQuery(uri.Query).TryGetValue("tipoOperacion", out var tipoOperacion)
                && QueryHelpers.ParseQuery(uri.Query).TryGetValue("usuario", out var usuario)
                )
            {
                FromDate = Convert.ToDateTime(fromDate);
                ToDate = Convert.ToDateTime(toDate);
                SelectedOperacionId = Convert.ToInt32(tipoOperacion);
                SelectedUsuarioId = Convert.ToInt32(usuario);
            }
            else
            {
                FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                ToDate = DateTime.Today;
                SelectedOperacionId = 0;
                SelectedUsuarioId = Usuario == null ? 0 : Usuario.IdUsuario.Value;
            }
            List = await ServiceContainer.OperacionService.GetOperaciones(GetFilters()); 
            Thread.Sleep(500);
            ServiceContainer.IsBusy = false;

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            Usuario = await ServiceContainer.UsuarioService.GetSessionData();
            if (Usuario != null && (Usuario.Username == null && Usuario.Password == null))
            {
                await JSRuntime.InvokeAsync<string>("clientJsMethods.RedirectTo", "/login");
            }
        }

        private async Task OnSearch()
        {

            ServiceContainer.QueryService.SetQueryString(TipoPagina.Operacion, GetFilters());
            NavManager.NavigateTo(QueryHelpers.AddQueryString("https://localhost:44307/operacion", GetFilters()));
            List = await ServiceContainer.OperacionService.GetOperaciones(GetFilters());
        }

        protected async Task OnDelete(int idFactura)
        {
            var response = await ServiceContainer.OperacionService.DeleteOperacion(idFactura);
            if (response.IsSuccessStatusCode)
            {
                List = await ServiceContainer.OperacionService.GetOperaciones(GetFilters());
            }
        }

        protected async Task OnUploadDocument()
        {
            if (BrowserFile != null)
            {
                try
                {
                    Stream stream = BrowserFile.OpenReadStream();
                    var path = Path.Combine(Environment.WebRootPath, "upload", BrowserFile.Name);
                    FileStream fs = File.Create(path);
                    await stream.CopyToAsync(fs);
                    stream.Close();
                    fs.Close();

                    byte[] bytes = File.ReadAllBytes(path);

                    if (BrowserFile.ContentType.Equals("image/jpeg"))
                    {
                        DocumentoUpload = new DocumentoModel();
                        DocumentoUpload.IdOperacion = SelectedOperacion.IdOperacion;
                        DocumentoUpload.IdTipoDocumento = ((int)TipoDocumento);
                        DocumentoUpload.SourceFile = bytes;

                        var response = await ServiceContainer.OperacionService.UpdateDocumento(DocumentoUpload);
                        if (response.IsSuccessStatusCode)
                        {
                            List = await ServiceContainer.OperacionService.GetOperaciones(GetFilters());
                            UpFileDialogOpen = false;
                            StateHasChanged();
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        private async Task OnDeleteDialogClose(bool accepted)
        {
            if (accepted)
            {
                await OnDelete(Id);
            }
            DeleteDialogOpen = false;
            StateHasChanged();

        }

        private void OpenDeleteDialog(OperacionModel model)
        {
            DeleteDialogOpen = true;
            StateHasChanged();
            Id = model.IdOperacion;
        }

        private void OpenUpdFileDialog(OperacionModel model, TipoDocumento tipoDocumento)
        {
            UpFileDialogOpen = true;
            TipoDocumento = tipoDocumento;
            SelectedOperacion = model;
            StateHasChanged();
        }

        private void OpenCreateFactura(OperacionModel model)
        {
            var query = new Dictionary<string, string> { { "idFactura", model.IdFactura == null ? "0" : model.IdFactura.ToString() },
                                                         { "idOperacion", model.IdOperacion.ToString()} };
            ServiceContainer.QueryService.SetQueryString(TipoPagina.Operacion, query);
            NavManager.NavigateTo(QueryHelpers.AddQueryString("https://localhost:44307/factura/redirect", query));
        }

        private async Task OnUpFileClose(bool accepted)
        {
            if (accepted)
            {
                await OnUploadDocument();
            }
            else
            {
                UpFileDialogOpen = false;
                StateHasChanged();
            }
        }

        private void OnObtainFile(IBrowserFile file)
        {
            BrowserFile = file;
        }

        private Dictionary<string, string> GetFilters()
        {
            return new Dictionary<string, string>
            {
                {"fromDate", FromDate.ToString("yyyy-MM-dd")},
                {"toDate", ToDate.ToString("yyyy-MM-dd") },
                {"idTipoOperacion", SelectedOperacionId.ToString() },
                {"idUsuario", SelectedUsuarioId.ToString()},
            };
        }

        private async Task OnViewFileAction(OperacionModel model, TipoDocumento tipoDocumento)
        {
            try
            {
                TipoDocumento = tipoDocumento;
                if (TipoDocumento == TipoDocumento.Factura)
                {
                    await JSRuntime.InvokeAsync<object>("open", "http://localhost:43248/Factura/pdf?idFactura=" + model.IdFactura, "_blank");
                }
                else
                {
                    await JSRuntime.InvokeAsync<object>("open", "http://localhost:43248/Operacion/document?idOperacion=" + model.IdOperacion + "&idTipoDocumento=" + ((int)TipoDocumento).ToString(), "_blank");
                }

            }
            catch (Exception ex)
            {

            }
        }

        private string SetRowStyle(string tipoFactura)
        {
            if (tipoFactura.Equals("Por vencer")) { return "background-color: #F1EB9C; color: black;"; }
            else if (tipoFactura.Equals("Vencido")) { return "background-color: #FF7276; color: black;"; }
            else { return ""; }
        }

        private RenderFragment RenderComponent(IComponent owner, bool status, OperacionModel element, TipoDocumento tipoDocumento) => builder =>
        {
            if (status)
            {
                builder.OpenElement(1, "button");
                builder.AddAttribute(2, "class", "btn btn-success btn-sm");
                builder.AddAttribute(3, "onclick",
                    EventCallback.Factory.Create(owner,
                       () => OnViewFileAction(element, tipoDocumento)));
                builder.AddMarkupContent(4, "<span class='oi oi-check' aria-hidden='true' />");
                builder.CloseElement();

                if (tipoDocumento == TipoDocumento.Factura)
                {
                    builder.OpenElement(5, "button");
                    builder.AddAttribute(6, "class", "btn btn-primary btn-sm");
                    builder.AddAttribute(7, "onclick",
                        EventCallback.Factory.Create(owner,
                           () => OpenCreateFactura(element)));
                    builder.AddMarkupContent(8, "<span class='oi oi-pencil' aria-hidden='true' />");
                    builder.CloseElement();
                }
                else
                {
                    builder.OpenElement(5, "button");
                    builder.AddAttribute(6, "class", "btn btn-primary btn-sm");
                    builder.AddAttribute(7, "onclick",
                        EventCallback.Factory.Create(owner,
                           () => OpenUpdFileDialog(element, tipoDocumento)));
                    builder.AddMarkupContent(8, "<span class='oi oi-pencil' aria-hidden='true' />");
                    builder.CloseElement();

                }

            }
            else
            {
                if (tipoDocumento == TipoDocumento.Factura)
                {
                    builder.OpenElement(1, "button");
                    builder.AddAttribute(2, "class", "btn btn-danger btn-sm");
                    builder.AddAttribute(3, "onclick",
                        EventCallback.Factory.Create(owner,
                           () => OpenCreateFactura(element)));
                    builder.AddMarkupContent(4, "<span class='oi oi-x' aria-hidden='true' />");
                    builder.CloseElement();
                }
                else
                {
                    builder.OpenElement(1, "button");
                    builder.AddAttribute(2, "class", "btn btn-danger btn-sm");
                    builder.AddAttribute(3, "onclick",
                        EventCallback.Factory.Create(owner,
                           () => OpenUpdFileDialog(element, tipoDocumento)));
                    builder.AddMarkupContent(4, "<span class='oi oi-x' aria-hidden='true' />");
                    builder.CloseElement();
                }

            }


        };
    }
}
