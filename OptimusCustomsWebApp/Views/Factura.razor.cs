using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using OptimusCustomsWebApp.Data.Service;
using OptimusCustomsWebApp.Model;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OptimusCustomsWebApp.Views
{
    public partial class Factura : ComponentBase
    {
        [Inject]
        protected FacturaService Service { get; set; }
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }
        [Inject]
        protected IHttpContextAccessor Accessor { get; set; }
        [Inject]
        private NavigationManager NavManager { get; set; }
        [Inject]
        private NavigationQueryService QueryService { get; set; }
        [Inject]
        private UsuarioService UsuarioService { get; set; }

        public List<FacturaModel> List;
        public bool DeleteDialogOpen { get; set; }
        public int IdFactura { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
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
            List = await Service.GetFacturas(FromDate, ToDate);
            Thread.Sleep(500);
            Service.IsBusy = false;

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            Usuario = await UsuarioService.GetSessionData();
            if (Usuario != null && (Usuario.Username == null && Usuario.Password == null))
            {
                await JSRuntime.InvokeAsync<string>("clientJsMethods.RedirectTo", "/login");
            }
        }

        private async Task OnSearch()
        {
            var query = new Dictionary<string, string> { { "fromDate", FromDate.ToString("yyyy-MM-dd") },
                                                         { "toDate", ToDate.ToString("yyyy-MM-dd")} };
            NavManager.NavigateTo(QueryHelpers.AddQueryString("https://localhost:44307/factura", query));
            QueryService.SetQueryString(Model.Enum.TipoPagina.Factura, query);
            List = await Service.GetFacturas(FromDate, ToDate);
        }

        protected async Task OnDelete(int idFactura)
        {
            var response = await Service.DeleteFactura(idFactura);
            if (response.IsSuccessStatusCode)
            {
                List = await Service.GetFacturas(FromDate, ToDate);
            }
        }

        private async Task OnDeleteDialogClose(bool accepted)
        {
            if (accepted)
            {
                await OnDelete(IdFactura);
            }
            DeleteDialogOpen = false;
            StateHasChanged();

        }

        private void OpenDeleteDialog(FacturaModel model)
        {
            DeleteDialogOpen = true;
            StateHasChanged();
            IdFactura = model.IdFactura;
        }

        private string SetRowStyle(string tipoFactura)
        {
            if (tipoFactura.Equals("Por vencer")) { return "background-color: #F1EB9C; color: black;"; }
            else if (tipoFactura.Equals("Vencido")) { return "background-color: #FF7276; color: black;"; }
            else { return ""; }
        }

        private async Task GetPdfFile(int idFactura)
        {
            await JSRuntime.InvokeAsync<object>("open", "http://localhost:43248/Factura/pdf?idFactura=" + idFactura, "_blank");
        }
    }
}
