using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaterProject.Models;
using MySql.EntityFrameworkCore;
namespace WaterProject
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        public IConfiguration Configuration { get; set; }

        public Startup (IConfiguration temp)
        {
            Configuration = temp;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<WaterProjectContext>(options =>
           {
               options.UseMySQL(Configuration["ConnectionStrings:WaterDBConnection"]);
           });


            services.AddScoped<IWaterProjectRepository, EFWaterProjectRepository>();
            services.AddScoped<IDonationRepository, EFDonationRepository>();
            services.AddRazorPages();

            services.AddDistributedMemoryCache();
            services.AddSession();

            //This line says 'when we are talking about Basket, we are referring to SessionBasket, which is either the already existing basket or if there is not one, it creates one'
            services.AddScoped<Basket>(x => SessionBasket.GetBasket(x));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //This tells it to use the files in wwwroot
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            
            app.UseEndpoints(endpoints =>
            {
                //You do not have to put 'name','pattern','defaults'. You can set it up like this and it is the same
                //  endpoints.MapControllerRoute("typepage","{projectType}/Page{pageNum}", new { Controller = "Home", action = "Index"});
                endpoints.MapControllerRoute(
                    name:"typepage",
                    pattern:"{projectType}/Page{pageNum}",
                    defaults:new { Controller = "Home", action = "Index" }
                    );

                endpoints.MapControllerRoute(
                    name: "Paging",
                    pattern: "Page{pageNum}",
                    defaults: new { Controller = "Home", action = "Index", pageNum = 1 }

                    );

                endpoints.MapControllerRoute("type",
                    "{projectType}",
                    new
                    {
                        Controller = "Home",
                        action = "Index",
                        pageNum = 1
                    });

                

                endpoints.MapDefaultControllerRoute();

                endpoints.MapRazorPages();
            });
        }
    }
}
