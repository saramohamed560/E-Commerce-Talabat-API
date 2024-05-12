using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.APIs.Helpers;
using Talabat.APIs.Middlewares;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Repositories.Contract;
using Talabat.Repositotry;
using Talabat.Repositotry._Identity;
using Talabat.Repositotry.Data;

namespace Talabat.APIs
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            #region Configure Services
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            //Add Required Web Api Services to DI Container
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            builder.Services.AddSwaggerServices();

            builder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
            });
            builder.Services.AddSingleton<IConnectionMultiplexer>(options => {

                var connection = builder.Configuration.GetConnectionString("RedisConnection");

                return ConnectionMultiplexer.Connect(connection);
            });

            builder.Services.AddApplicationServices();
            builder.Services.AddIdentityServices(builder.Configuration);
            builder.Services.AddCors( Options =>
            {
                Options.AddPolicy("MyPolicy", options =>
                {
                    options.AllowAnyHeader();
                    options.AllowAnyMethod();
                    options.WithOrigins(builder.Configuration["FrontBaseUrl"]);
                });
            });

            //builder.Services.AddScoped<IGenericRepository<Product>, GenericRepository<Product>>();
            //builder.Services.AddScoped<IGenericRepository<ProductBrand>, GenericRepository<ProductBrand>>();
            //builder.Services.AddScoped<IGenericRepository<ProductCategory>, GenericRepository<ProductCategory>>();



            //Before Build App Add 6 Services
            //1.Configuration Services (WebApplicationBuilder.Services.)
            //2.(builder.Configuartion) => Any Thing Related to AppSetting
            //3.(builder.Environment) ..... 
            #endregion

            var app = builder.Build();
            #region Update DataBase
            var scope = app.Services.CreateScope(); //Create scope (time)
            var services = scope.ServiceProvider;//servieprovider allow to ask any object from services
            var dbcontext = services.GetRequiredService<StoreContext>();
            var Identitydbcontext = services.GetRequiredService<ApplicationIdentityDbContext>();
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            //Ask CLR TO Create object from  DbContext Explicitly
            try
            {
                await dbcontext.Database.MigrateAsync();
                await StoreContextSeed.SeedAsync(dbcontext);
                await Identitydbcontext.Database.MigrateAsync();
                var _userManager= services.GetRequiredService<UserManager<ApplicationUser>>();
                //must allow DE For UserManager
                await ApplicationIdentityDataSeed.SeedUsersAsync(_userManager);

            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "An Error occured during Apply Migraion");
            }
            // using try finally to dispose finally { scope.Dispose(); }




            #endregion

            // Configure the HTTP request pipeline.
            #region Configure Kestrel MiddleWares

            app.UseMiddleware<ExceptionMiddleware>();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerMiddleWares();
            }
            //app.UseStatusCodePagesWithRedirects("/errors/{0}");//make two  requests
            app.UseStatusCodePagesWithReExecute("/errors/{0}");//make one  request
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("MyPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseRouting();
            //app.UseEndpoints(endPoint => endPoint.MapControllers());
            // ==
            app.MapControllers();
            //Excute Route For Each Controller based on Route Attribute 
            #endregion

            app.Run();
        }
    }
}