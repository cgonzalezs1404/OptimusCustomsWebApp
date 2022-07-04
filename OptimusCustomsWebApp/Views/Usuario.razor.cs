using Microsoft.AspNetCore.Components;
using OptimusCustomsWebApp.Data;
using OptimusCustomsWebApp.Data.Service;
using OptimusCustomsWebApp.Model;
using OptimusCustomsWebApp.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptimusCustomsWebApp.Views
{
    public partial class Usuario : ComponentBase
    {
        [Inject]
        protected UsuarioService Service { get; set; }
        public List<UsuarioModel> ModelList { get; set; }
        public bool DeleteDialogOpen { get; set; }

        public int Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ModelList = await Service.GetUsuarios(null, null);

        }

        protected async Task OnDelete(int id)
        {
            var response = await Service.DeleteUsuario(id);
            if (response.IsSuccessStatusCode)
            {
                ModelList = await Service.GetUsuarios(null, null);
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

        private void OpenDeleteDialog(UsuarioModel model)
        {
            DeleteDialogOpen = true;
            StateHasChanged();
            Id = model.IdUsuario;
        }
    }
}
