using Microsoft.AspNetCore.Components;
using OptimusCustomsWebApp.Data.Service;
using OptimusCustomsWebApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace OptimusCustomsWebApp.Views
{
    public partial class Factura : ComponentBase
    {
        [Inject]
        protected FacturaService Service { get; set; }
        public List<FacturaModel> List;
        public bool DeleteDialogOpen { get; set; }
        public int IdFactura { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        protected override async Task OnInitializedAsync()
        {
            FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            ToDate = DateTime.Today;
            List = await Service.GetFacturas(FromDate, ToDate);
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
    }
}
