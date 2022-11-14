using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using OptimusCustomsWebApp.Data.Service;
using OptimusCustomsWebApp.Helpers;
using OptimusCustomsWebApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OptimusCustomsWebApp.Pages
{
    public partial class Login
    {
        [Inject]
        protected SessionState Session { get; set; }
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }
        [Inject]
        protected IHttpContextAccessor Accessor { get; set; }
        [Inject]
        protected UsuarioService Service { get; set; }

        public SessionData Model { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await Task.Factory.StartNew(() =>
                {
                    Model = new SessionData();
                });

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {

                string user = await JSRuntime.InvokeAsync<string>("clientCookiesMethods.GetCookie", "Username");
                string pass = await JSRuntime.InvokeAsync<string>("clientCookiesMethods.GetCookie", "Password");

                if (user != null && pass != null)
                {
                    Model.Username = user;
                    Model.Password = pass;
                    Model.RememberMe = true;

                    Session.Items.Add("Username", Model.Username);
                    Session.Items.Add("Password", Model.Password);
                    

                    await JSRuntime.InvokeAsync<string>("clientJsMethods.RedirectTo", "/factura");
                }
            }

        }

        private async Task SignIn()
        {
            var result = await Service.LoginUsuario(Model.Username, Model.Password);
            if (result != null)
            {

                if (!Session.Items.ContainsKey("Username") 
                    && !Session.Items.ContainsKey("Password") 
                    )
                {
                    //Add to the Singleton scoped Item
                    Session.Items.Add("Username", Model.Username);
                    Session.Items.Add("Password", Model.Password);
                    


                    if (Model.RememberMe)
                    {
                        await JSRuntime.InvokeVoidAsync("clientCookiesMethods.SetCookie", new object[] { "Username", Model.Username, 30 });
                        await JSRuntime.InvokeVoidAsync("clientCookiesMethods.SetCookie", new object[] { "Password", Model.Password, 30 });
                    }

                    await JSRuntime.InvokeAsync<string>("clientJsMethods.RedirectTo", "/factura");

                }
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("alert", "Usuario/Contraseña incorrecta!"); // Alert
            }

        }


    }
}
