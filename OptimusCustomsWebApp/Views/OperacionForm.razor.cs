using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.JSInterop;
using OptimusCustomsWebApp.Data.Service;
using OptimusCustomsWebApp.Model;
using OptimusCustomsWebApp.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptimusCustomsWebApp.Views
{
    public partial class OperacionForm : ComponentBase
    {
        [Inject]
        public NavigationManager Navigation { get; set; }
        [Inject]
        private IWebHostEnvironment Environment { get; set; }
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }
        public OperacionModel Model { get; set; }
        [Inject]
        public OperacionService Service { get; set; }
        [Inject]
        public CatalogoService CatalogoService { get; set; }

        public List<CatalogoModel> UsersList { get; set; }
        public List<CatalogoModel> TipoOperacionList { get; set; }

        [Parameter]
        public string IdOperacion { get; set; }

        public string IdUsuario { get; set; }
        public string IdTipoOperacion { get; set; }

        protected async override Task OnInitializedAsync()
        {
            Model = new OperacionModel();
            UsersList = await CatalogoService.GetUsuarios();
            TipoOperacionList = await CatalogoService.GetTipoOperacion();
        }

        protected async override Task OnParametersSetAsync()
        {
            if (IdOperacion != null && !IdOperacion.Equals(""))
            {
                Model = await Service.GetOperacion(int.Parse(IdOperacion));
                IdUsuario = Model.IdUsuario.ToString();
                IdTipoOperacion = Model.IdTipoOperacion.ToString();
            }

        }

        protected async Task OnCreate()
        {
            Model.IdUsuario = int.Parse(IdUsuario);
            Model.IdTipoOperacion = int.Parse(IdTipoOperacion);
            var response = await Service.CreateOperacion(Model);
            if (response.IsSuccessStatusCode)
            {
                Navigation.NavigateTo("/operacion");
            }
        }

        protected async Task OnCancel()
        {
            await JSRuntime.InvokeAsync<object>("history.go", -1);
        }

        protected async Task OnUpdate()
        {
            Model.IdUsuario = int.Parse(IdUsuario);
            Model.IdTipoOperacion = int.Parse(IdTipoOperacion);
            var response = await Service.UpdateOperacion(Model);
            if (response.IsSuccessStatusCode)
            {
                Navigation.NavigateTo("/operacion");
            }
        }
    }
}
