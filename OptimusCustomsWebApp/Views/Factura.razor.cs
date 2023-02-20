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
        protected IJSRuntime JSRuntime { get; set; }
        [Inject]
        protected IHttpContextAccessor Accessor { get; set; }
        [Inject]
        private NavigationManager NavManager { get; set; }
        [Inject]
        private ServiceContainer ServiceContainer { get; set; }

        public List<FacturaModel> List;
        public List<CatalogoModel> CatalogoTFactura { get; set; }
        public List<CatalogoModel> CatalogoUsuarios { get; set; }
        public List<CatalogoModel> CatalogoEFactura { get; set; }
        public int SelectedTFacturaId { get; set; }
        public int SelectedUsuarioId { get; set; }
        public int SelectedEFacturaId { get; set; }
        public bool DeleteDialogOpen { get; set; }
        public int IdFactura { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public SessionData Usuario { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ServiceContainer.IsBusy = true;
            CatalogoTFactura = await ServiceContainer.CatalogoService.GetTipoFactura();
            CatalogoUsuarios = await ServiceContainer.CatalogoService.GetUsuarios();
            CatalogoEFactura = await ServiceContainer.CatalogoService.GetEstadoFactura();
            Usuario = await ServiceContainer.UsuarioService.GetSessionData();

            var uri = NavManager.ToAbsoluteUri(NavManager.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("fromDate", out var fromDate)
                && QueryHelpers.ParseQuery(uri.Query).TryGetValue("toDate", out var toDate)
                && QueryHelpers.ParseQuery(uri.Query).TryGetValue("idEstadoFactura", out var estadoFactura)
                && QueryHelpers.ParseQuery(uri.Query).TryGetValue("idTipoFactura", out var tipoFactura)
                && QueryHelpers.ParseQuery(uri.Query).TryGetValue("idUsuario", out var usuario)
                )
            {
                FromDate = Convert.ToDateTime(fromDate);
                ToDate = Convert.ToDateTime(toDate);
                SelectedEFacturaId = Convert.ToInt32(estadoFactura);
                SelectedTFacturaId = Convert.ToInt32(tipoFactura);
                SelectedUsuarioId = Convert.ToInt32(usuario);
            }
            else
            {
                FromDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                ToDate = DateTime.Today;
                SelectedEFacturaId = 0;
                SelectedTFacturaId = 0;
                SelectedUsuarioId = Usuario == null ? 0 : Usuario.IdUsuario.Value;
            }
            List = await ServiceContainer.FacturaService.GetFacturas(GetFilters());
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
            
            NavManager.NavigateTo(QueryHelpers.AddQueryString("https://localhost:44307/factura", GetFilters()));
            ServiceContainer.QueryService.SetQueryString(Model.Enum.TipoPagina.Factura, GetFilters());
            List = await ServiceContainer.FacturaService.GetFacturas(GetFilters());
        }

        protected async Task OnDelete(int idFactura)
        {
            var response = await ServiceContainer.FacturaService.DeleteFactura(idFactura);
            if (response.IsSuccessStatusCode)
            {
                List = await ServiceContainer.FacturaService.GetFacturas(GetFilters());
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

        private Dictionary<string, string> GetFilters()
        {
            return new Dictionary<string, string>
            {
                {"fromDate", FromDate.ToString("yyyy-MM-dd")},
                {"toDate", ToDate.ToString("yyyy-MM-dd") },
                {"idEstadoFactura", SelectedEFacturaId.ToString() },
                {"idTipoFactura", SelectedTFacturaId.ToString()},
                {"idUsuario", SelectedUsuarioId.ToString()},
            };
        }
    }
}
