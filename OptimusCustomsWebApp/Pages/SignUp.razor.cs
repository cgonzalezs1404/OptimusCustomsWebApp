using Microsoft.AspNetCore.Components;
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
        public UsuarioModel Model { get; set; }
        public List<CatalogoModel> TipoUsuario { get; set; }
        public string IdTipoUsuario { get; set; }

        protected override async Task OnInitializedAsync()
        {
            TipoUsuario = await CatalogoService.GetTipoUsuario();
            Model = new UsuarioModel();
        }

        private async Task Register()
        {
            Model.IdPrivilegio = Int32.Parse(IdTipoUsuario);
            var response = await UsuarioService.CreateUsuario(Model);
            if (response.IsSuccessStatusCode)
            {
                await JSRuntime.InvokeAsync<string>("clientJsMethods.RedirectTo", "/home");
            }

        }

        private async Task Cancel()
        {
            await JSRuntime.InvokeAsync<string>("clientJsMethods.RedirectTo", "/home");


        }
    }
}
