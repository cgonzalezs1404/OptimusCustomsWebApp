using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace OptimusCustomsWebApp.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public class SessionBootstrapper
    {
        private readonly IHttpContextAccessor accessor;
        private readonly SessionState session;
        public string Username;
        public string Password;
        public int? IdUsuario;

        CookieContainer cookie;
        HttpClient HttpClient;
        public SessionBootstrapper(IHttpContextAccessor _accessor, SessionState _session)
        {
            accessor = _accessor;
            session = _session;

            string user = accessor.HttpContext.Request.Cookies["Username"];
            string pass = accessor.HttpContext.Request.Cookies["Password"];
            int? idUser = Convert.ToInt32(accessor.HttpContext.Request.Cookies["Id"]);

            if(user!= null && pass != null && idUser != null)
            {
                Username = user;
                Password = pass;
                IdUsuario = idUser;

                accessor.HttpContext.Session.SetString("Username", Username);
                accessor.HttpContext.Session.SetString("Password", Password);
                accessor.HttpContext.Session.SetInt32("Id", IdUsuario.Value);

                accessor.HttpContext.Response.Redirect("/factura");
            }
            else
            {
                accessor.HttpContext.Response.Redirect("/login");
            }

        }
        public void Bootstrap()
        {
            //Singleton Item: services.AddSingleton<SessionState>(); in Startup.cs

            //Code to save data in server side session

            //If session already has data
            Username = accessor.HttpContext.Session.GetString("Username");
            Password = accessor.HttpContext.Session.GetString("Password");
            IdUsuario = accessor.HttpContext.Session.GetInt32("Id");

            //If server session is null
            if (session.Items.ContainsKey("Username") && Username == null)
            {
                //get from singleton item
                Username = session.Items["Username"]?.ToString();
                // save to server side session
                accessor.HttpContext.Session.SetString("Username", Username);
                //remove from singleton Item
                session.Items.Remove("Username");
            }

            if (session.Items.ContainsKey("Password") && Password == null)
            {
                Password = session.Items["Password"].ToString();
                accessor.HttpContext.Session.SetString("Password", Password);
                session.Items.Remove("Password");
            }

            if(session.Items.ContainsKey("Id") && IdUsuario == null)
            {
                IdUsuario = Convert.ToInt32(session.Items["Id"]);
                accessor.HttpContext.Session.SetInt32("Id", IdUsuario.Value);
                session.Items.Remove("Id");
            }

            //If Session is not expired yet then  navigate to home
            if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password) && IdUsuario != null && accessor.HttpContext.Request.Path == "/")
            {
                accessor.HttpContext.Response.Redirect("/factura");
            }
            //If Session is expired then navigate to login
            else if (string.IsNullOrEmpty(Username) && string.IsNullOrEmpty(Password) && IdUsuario == null && accessor.HttpContext.Request.Path != "/")
            {
                accessor.HttpContext.Response.Redirect("/");
            }
        }
    }
}
