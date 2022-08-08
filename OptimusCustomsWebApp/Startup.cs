using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OptimusCustomsWebApp.Data;
using OptimusCustomsWebApp.Data.Service;
using OptimusCustomsWebApp.Helpers;
using OptimusCustomsWebApp.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OptimusCustomsWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //Manejo de sesion en la app
            services.AddSingleton<SessionState>();
            services.AddSingleton<SessionBootstrapper>();
            services.AddHttpContextAccessor();
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            //Carga de servicios por componentes en la app
            services.AddSingleton<FacturaService>();
            services.AddSingleton<UsuarioService>();
            services.AddSingleton<CatalogoService>();
            services.AddSingleton<OperacionService>();

            //Carga de clientes HTTP para invocacion de API REST
            services.AddHttpClient<IFactura, FacturaService>(client => { client.BaseAddress = new Uri("http://localhost:43248/Factura"); });
            services.AddHttpClient<IUsuario, UsuarioService>(client => { client.BaseAddress = new Uri("http://localhost:43248/Usuario"); });
            services.AddHttpClient<ICatalogo, CatalogoService>(client => { client.BaseAddress = new Uri("http://localhost:43248/Catalogos"); });
            services.AddHttpClient<IOperacion, OperacionService>(client => { client.BaseAddress = new Uri("http://localhost:43248/Operacion"); });

            services.AddRazorPages();
            services.AddServerSideBlazor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSession();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}