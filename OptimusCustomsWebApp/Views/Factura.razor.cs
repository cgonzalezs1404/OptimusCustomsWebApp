using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using OptimusCustomsWebApp.Data.Service;
using OptimusCustomsWebApp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
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

        public List<FacturaModel> List;
        public bool DeleteDialogOpen { get; set; }
        public int IdFactura { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Service.IsBusy = true;
            FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            ToDate = DateTime.Today;
            await OnSearch();

            Thread.Sleep(2000);
            Service.IsBusy = false;
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
