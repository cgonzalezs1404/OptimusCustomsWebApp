using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using OptimusCustomsWebApp.Data.Service;
using OptimusCustomsWebApp.Interface;
using OptimusCustomsWebApp.Model;
using OptimusCustomsWebApp.Model.Enum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace OptimusCustomsWebApp.Views
{
    public partial class Operacion
    {
        [Inject]
        protected OperacionService Service { get; set; }
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }
        [Inject]
        protected IHttpContextAccessor Accessor { get; set; }
        [Inject]
        private IWebHostEnvironment Environment { get; set; }
        [Inject]
        private NavigationManager NavManager { get; set; }

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

        private Dictionary<string, string> Query { get; set; }
        public SessionData Usuario { get; set; }

        protected override async Task OnInitializedAsync()
        {

            Service.IsBusy = true;
            var uri = NavManager.ToAbsoluteUri(NavManager.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("fromDate", out var fromDate) && QueryHelpers.ParseQuery(uri.Query).TryGetValue("toDate", out var toDate))
            {
                FromDate = Convert.ToDateTime(fromDate);
                ToDate = Convert.ToDateTime(toDate);
            }
            else
            {
                FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                ToDate = DateTime.Today;
            }
            List = await Service.GetOperaciones(FromDate.ToString("yyyy-MM-dd"), ToDate.ToString("yyyy-MM-dd"));
            Thread.Sleep(1000);
            Service.IsBusy = false;

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            Usuario = await GetSession();
            if (Usuario != null && (Usuario.Username == null && Usuario.Password == null))
            {
                await JSRuntime.InvokeAsync<string>("clientJsMethods.RedirectTo", "/login");
            }
        }

        private async Task<SessionData> GetSession()
        {
            var result = new SessionData();
            result.Username = Accessor.HttpContext == null ? null : Accessor.HttpContext.Session.GetString("Username");
            result.Password = Accessor.HttpContext == null ? null : Accessor.HttpContext.Session.GetString("Password");
            await InvokeAsync(() => StateHasChanged());
            return result;
        }

        private async Task OnSearch()
        {
            var query = new Dictionary<string, string> { { "fromDate", FromDate.ToString("yyyy-MM-dd") },
                                                         { "toDate", ToDate.ToString("yyyy-MM-dd")} };
            NavManager.NavigateTo(QueryHelpers.AddQueryString("https://localhost:44307/operacion", query));
            List = await Service.GetOperaciones(FromDate.ToString("yyyy-MM-dd"), ToDate.ToString("yyyy-MM-dd"));
        }

        protected async Task OnDelete(int idFactura)
        {
            var response = await Service.DeleteOperacion(idFactura);
            if (response.IsSuccessStatusCode)
            {
                List = await Service.GetOperaciones(FromDate.ToString("yyyy-MM-dd"), ToDate.ToString("yyyy-MM-dd"));
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

                        var response = await Service.UpdateDocumento(DocumentoUpload);
                        if (response.IsSuccessStatusCode)
                        {
                            List = await Service.GetOperaciones(FromDate.ToString("yyyy-MM-dd"), ToDate.ToString("yyyy-MM-dd"));
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
            Id = model.IdFactura.Value;
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
            var query = new Dictionary<string, string> { { "idFactura", model.IdFactura.ToString() },
                                                         { "idOperacion", model.IdOperacion.ToString()} };
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
                builder.AddMarkupContent(1, "<div class='btn-group-sm'>");

                builder.OpenElement(2, "button");
                builder.AddAttribute(3, "class", "btn btn-success btn-sm");
                builder.AddAttribute(4, "onclick",
                    EventCallback.Factory.Create(owner,
                       () => OnViewFileAction(element, tipoDocumento)));
                builder.AddMarkupContent(5, "<span class='oi oi-check' aria-hidden='true' />");
                builder.CloseElement();

                if (TipoDocumento == TipoDocumento.Factura)
                {
                    builder.OpenElement(6, "button");
                    builder.AddAttribute(7, "class", "btn btn-primary btn-sm");
                    builder.AddAttribute(8, "onclick",
                        EventCallback.Factory.Create(owner,
                           () => OpenCreateFactura(element)));
                    builder.AddMarkupContent(9, "<span class='oi oi-pencil' aria-hidden='true' />");
                    builder.CloseElement();

                    builder.AddMarkupContent(10, "</div>");
                }
                else
                {
                    builder.OpenElement(6, "button");
                    builder.AddAttribute(7, "class", "btn btn-primary btn-sm");
                    builder.AddAttribute(8, "onclick",
                        EventCallback.Factory.Create(owner,
                           () => OpenUpdFileDialog(element, tipoDocumento)));
                    builder.AddMarkupContent(9, "<span class='oi oi-pencil' aria-hidden='true' />");
                    builder.CloseElement();

                    builder.AddMarkupContent(10, "</div>");
                }

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


        };

        public static string RemoveQueryStringByKey(string url, string key)
        {
            var uri = new Uri(url);

            // this gets all the query string key value pairs as a collection
            var newQueryString = HttpUtility.ParseQueryString(uri.Query);

            // this removes the key if exists
            newQueryString.Remove(key);

            // this gets the page path from root without QueryString
            string pagePathWithoutQueryString = uri.GetLeftPart(UriPartial.Path);

            return newQueryString.Count > 0
                ? String.Format("{0}?{1}", pagePathWithoutQueryString, newQueryString)
                : pagePathWithoutQueryString;
        }
    }
}
