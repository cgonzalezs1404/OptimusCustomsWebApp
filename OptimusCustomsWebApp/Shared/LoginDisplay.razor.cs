using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using OptimusCustomsWebApp.Helpers;
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
        public SessionData Usuario { get; set; }

        private async Task GetSession()
        {
            Usuario = new SessionData();
            Usuario.Username = Accessor.HttpContext.Session.GetString("Username");
            Usuario.Password = Accessor.HttpContext.Session.GetString("Password");
            await InvokeAsync(() => StateHasChanged());
        }

        protected override async Task OnInitializedAsync()
        {
            await GetSession();
        }

        private async Task LogOut()
        {
            if (Accessor.HttpContext.Session.Keys.Contains("Username") && Accessor.HttpContext.Session.Keys.Contains("Password"))
            {
                Accessor.HttpContext.Session.Clear();

                await JSRuntime.InvokeAsync<string>(
                "clientJsMethods.RedirectTo", "/login");
            }
        }
    }
}
