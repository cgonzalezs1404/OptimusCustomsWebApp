using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
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

        public List<FacturaModel> List;
        public bool DeleteDialogOpen { get; set; }
        public int IdFactura { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public SessionData Usuario { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Service.IsBusy = true;
            FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            ToDate = DateTime.Today;
            await OnSearch();
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
