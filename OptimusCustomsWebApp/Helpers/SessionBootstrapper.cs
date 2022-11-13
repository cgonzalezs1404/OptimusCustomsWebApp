using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public SessionBootstrapper(IHttpContextAccessor _accessor, SessionState _session)
        {
            accessor = _accessor;
            session = _session;
        }
        public void Bootstrap()
        {
            //Singleton Item: services.AddSingleton<SessionState>(); in Startup.cs

            //Code to save data in server side session

            //If session already has data
            Username = accessor.HttpContext.Session.GetString("Username");
            Password = accessor.HttpContext.Session.GetString("Password");

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

            //If Session is not expired yet then  navigate to home
            if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password) && accessor.HttpContext.Request.Path == "/")
            {
                accessor.HttpContext.Response.Redirect("/home");
            }
            //If Session is expired then navigate to login
            else if (string.IsNullOrEmpty(Username) && string.IsNullOrEmpty(Password) && accessor.HttpContext.Request.Path != "/")
            {
                accessor.HttpContext.Response.Redirect("/");
            }
        }
    }
}
