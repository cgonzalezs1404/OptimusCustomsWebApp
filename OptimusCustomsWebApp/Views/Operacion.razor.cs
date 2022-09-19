using Microsoft.AspNetCore.Components;
using OptimusCustomsWebApp.Data.Service;
using OptimusCustomsWebApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptimusCustomsWebApp.Views
{
    public partial class Operacion
    {
        [Inject]
        protected OperacionService Service { get; set; }
        public List<OperacionModel> List;
        public bool DeleteDialogOpen { get; set; }
        public int Id { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        protected override async Task OnInitializedAsync()
        {
            FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            ToDate = DateTime.Today;
            List = await Service.GetOperaciones(FromDate, ToDate);
        }

        private async Task OnSearch()
        {
            List = await Service.GetOperaciones(FromDate, ToDate);
        }

        protected async Task OnDelete(int idFactura)
        {
            var response = await Service.DeleteOperacion(idFactura);
            if (response.IsSuccessStatusCode)
            {
                List = await Service.GetOperaciones(FromDate, ToDate);
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

        private string SetRowStyle(string tipoFactura)
        {
            if (tipoFactura.Equals("Por vencer")) { return "background-color: #F1EB9C; color: black;"; }
            else if (tipoFactura.Equals("Vencido")) { return "background-color: #FF7276; color: black;"; }
            else { return ""; }
        }
    }
}
