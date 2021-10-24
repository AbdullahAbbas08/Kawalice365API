using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IO;
using System.Text;
using TravelAPI.Core;
using TravelAPI.EF;
using TravelAPI.Interfaces;
using TravelAPI.Models;
using TravelAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using TravelAPI.Core.Helper;
using BalarinaAPI.Core.Model;
using BalarinaAPI.Core.Helper;
using BalarinaAPI.Core.Services;

namespace TravelAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<Helper>(Configuration.GetSection("PATHS"));
            services.Configure<TrendingDuration>(Configuration.GetSection("TrendingDuration"));
            services.Configure<JWTConfig>(Configuration.GetSection("JWT"));

            services.AddControllers()
             .AddJsonOptions(options =>
                options.JsonSerializerOptions.PropertyNamingPolicy = null);
          

            services.AddCors();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TravelAPI", Version = "v1" });
            });
            #region SQL Server Connection AND LazyLoading Status

            services.AddDbContext<BalarinaDatabaseContext>(options =>
            {
                options
                 .UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))/*.UseLazyLoadingProxies()*/;
            });
            #endregion

            #region Use Identiy in My DbContext And Set Constraints On Password
            services.AddIdentity<ApplicationUser, ApplicationRole>(
            option =>
            {
                option.Password.RequireDigit = true;
                option.Password.RequiredLength = 8;
                option.Password.RequireLowercase = true;
                option.Password.RequireUppercase = true;
                option.Password.RequireNonAlphanumeric = true;
                option.SignIn.RequireConfirmedEmail = false;

            }
            ).AddEntityFrameworkStores<BalarinaDatabaseContext>().AddDefaultTokenProviders(); ;
            #endregion

            #region Map Interfaces To Classes( Implementations )
            services.AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAuthService, AuthService>();

            #endregion   

            #region Configuration of Upload Image 
            services.Configure<FormOptions>(o => {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });
            #endregion

            #region JWT Configuration
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = Configuration["JWT:Issuer"],
                        ValidAudience = Configuration["JWT:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"]))
                    };
                });
            #endregion


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("AllowAccess");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TravelAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            //app.UseCors(m => m.WithOrigins("https://localhost:44312", "https://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials());
            //app.UseStaticFiles();
            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(
            //                    Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
            //    RequestPath = new PathString("/Resources")
            //}); 

            app.UseStaticFiles();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
