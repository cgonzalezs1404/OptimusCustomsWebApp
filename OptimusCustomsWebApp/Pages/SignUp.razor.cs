using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using OptimusCustomsWebApp.Data.Service;
using OptimusCustomsWebApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptimusCustomsWebApp.Pages
{
    public partial class SignUp
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }
        [Inject]
        protected CatalogoService CatalogoService { get; set; }
        [Inject]
        protected UsuarioService UsuarioService { get; set; }
        [Inject]
        private NavigationManager NavManager { get; set; }
        public UsuarioModel Model { get; set; }
        public List<CatalogoModel> TipoUsuario { get; set; }
        public string IdTipoUsuario { get; set; }
        [Parameter]
        public string IdUsuario { get; set; }

        protected override async Task OnInitializedAsync()
        {
            TipoUsuario = await CatalogoService.GetTipoUsuario();

            Model = new UsuarioModel();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (IdUsuario != null && !IdUsuario.Equals(""))
            {
                Model = await UsuarioService.GetUsuario(Convert.ToInt32(IdUsuario));
            }
        }

        private async Task Register()
        {

            var response = await UsuarioService.CreateUsuario(Model);
            if (response.IsSuccessStatusCode)
            {
                NavManager.NavigateTo("/login");
            }
        }

        private async Task OnUpdate()
        {
            if (IdTipoUsuario != null)
                Model.IdTipoUsuario = Int32.Parse(IdTipoUsuario);
            var response = await UsuarioService.UpdateUsuario(Model);
            if (response.IsSuccessStatusCode)
            {
                NavManager.NavigateTo("/usuario");
            }
        }

        private async Task Cancel()
        {
            if (IdUsuario == null)
                await JSRuntime.InvokeAsync<string>("clientJsMethods.RedirectTo", "/home");
            else
                await JSRuntime.InvokeAsync<string>("clientJsMethods.RedirectTo", "/usuario");

        }
    }
}
