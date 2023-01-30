using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using OptimusCustomsWebApp.Data.Service;
using OptimusCustomsWebApp.Helpers;
using OptimusCustomsWebApp.Interface;
using OptimusCustomsWebApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptimusCustomsWebApp.Shared
{
    public partial class LoginDisplay
    {
        [Inject]
        protected SessionState Session { get; set; }
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }
        [Inject]
        protected IHttpContextAccessor Accessor { get; set; }
        [Inject]
        protected UsuarioService UsuarioService { get; set; }
        public SessionData Usuario { get; set; }

        protected override async Task OnInitializedAsync()
        {
            //await GetSession();
            Usuario = await UsuarioService.GetSessionData();
            await InvokeAsync(() => StateHasChanged());
        }

        private async Task LogOut()
        {
            Accessor.HttpContext.Session.Clear();
            await JSRuntime.InvokeAsync<string>("clientJsMethods.RedirectTo", "/login");

            await JSRuntime.InvokeVoidAsync("clientCookiesMethods.DeleteCookie", "Username");
            await JSRuntime.InvokeVoidAsync("clientCookiesMethods.DeleteCookie", "Password");
            await JSRuntime.InvokeVoidAsync("clientCookiesMethods.DeleteCookie", "Id");
        }
    }
}
