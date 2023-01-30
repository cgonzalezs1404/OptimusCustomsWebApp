using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
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
        private NavigationManager NavManager { get; set; }
        [Inject]
        private IWebHostEnvironment Environment { get; set; }
        [Inject]
        private NavigationQueryService QueryService { get; set; }
        private OperacionModel Model { get; set; }
        [Inject]
        private OperacionService Service { get; set; }
        [Inject]
        private CatalogoService CatalogoService { get; set; }
        [Inject]
        private UsuarioService UsuarioService { get; set; }
        [Parameter]
        public string IdOperacion { get; set; }
        public List<CatalogoModel> UsersList { get; set; }
        public List<CatalogoModel> TipoOperacionList { get; set; }
        public SessionData Usuario { get; set; }

        public string IdUsuario { get; set; }
        public string IdTipoOperacion { get; set; }
        private EditContext? editContext;

        protected async override Task OnInitializedAsync()
        {
            Model = new OperacionModel();
            UsersList = await CatalogoService.GetUsuarios();
            TipoOperacionList = await CatalogoService.GetTipoOperacion();
            editContext = new EditContext(Model);
            Usuario = await UsuarioService.GetSessionData();
            IdUsuario = Usuario.IdUsuario.ToString();
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
            if (editContext != null && editContext.Validate())
            {
                var response = await Service.CreateOperacion(Model);
                if (response.IsSuccessStatusCode)
                {
                    if (QueryService.GetQueryString(TipoPagina.Operacion) != null)
                        NavManager.NavigateTo(QueryHelpers.AddQueryString("https://localhost:44307/operacion", QueryService.GetQueryString(TipoPagina.Operacion)));
                    else
                        NavManager.NavigateTo("/operacion");
                }
            }

        }

        protected void OnCancel()
        {

            if (QueryService.GetQueryString(TipoPagina.Operacion) != null)
                NavManager.NavigateTo(QueryHelpers.AddQueryString("https://localhost:44307/operacion", QueryService.GetQueryString(TipoPagina.Operacion)));
            else
                NavManager.NavigateTo("/operacion");


        }

        protected async Task OnUpdate()
        {
            var response = await Service.UpdateOperacion(Model);
            if (response.IsSuccessStatusCode)
            {
                if (QueryService.GetQueryString(TipoPagina.Operacion) != null)
                    NavManager.NavigateTo(QueryHelpers.AddQueryString("https://localhost:44307/operacion", QueryService.GetQueryString(TipoPagina.Operacion)));
                else
                    NavManager.NavigateTo("/operacion");
            }
        }
    }
}
