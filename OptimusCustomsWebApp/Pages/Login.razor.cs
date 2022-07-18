﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using OptimusCustomsWebApp.Data.Service;
using OptimusCustomsWebApp.Helpers;
using OptimusCustomsWebApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private async Task SignIn()
        {
            var result = await Service.LoginUsuario(Model.Username, Model.Password);
            if(result != null)
            {
                if (!Session.Items.ContainsKey("Username") && !Session.Items.ContainsKey("Password"))
                {
                    //Add to the Singleton scoped Item
                    Session.Items.Add("Username", Model.Username);
                    Session.Items.Add("Password", Model.Password);

                    await JSRuntime.InvokeAsync<string>(
                    "clientJsMethods.RedirectTo", "/factura");
                }
            }
            else
            {

            }
            
        }


    }
}