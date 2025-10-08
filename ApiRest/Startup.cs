using ApiRest.Repositorio;
using ApiRest.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ApiRest
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
            var cadenaConexionSqlConfiguracion = new AccesoDatos(Configuration.GetConnectionString("SQL"));
            services.AddSingleton(cadenaConexionSqlConfiguracion);

            //services.AddControllers();
           // services.AddSingleton<IConsultaEnMemoria, ConsultaSQLServer>();
            services.AddSingleton<IClaroEnMemoria, ClaroSQLServer>();
            services.AddSingleton<IUsuariosSQLServer, UsuariosSQLServer>();
            services.AddControllers(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;

            });
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null; // Mantener nombres originales
                    options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
                });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiRest", Version = "v1" });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "Autorizacion JWT esquema. \r\n\r\n Escribe 'Bearer' [espacio] y escribe tu token.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                },
                                Scheme = "oauth2",
                                Name = "Bearer",
                                In = ParameterLocation.Header,

                            },
                        new List<string>()
                    }
                });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["JWT:Issuer"],
                    ValidAudience = Configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Configuration["JWT:ClaveSecreta"]))
                };
            });


            services.AddAuthorization(options =>
            {
                options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador").RequireClaim("conecel", "False"));
                options.AddPolicy("Conecel", policy => policy.RequireRole("conecel").RequireClaim("conecel", "True"));
            //    //options.AddPolicy("Usuario", policy => policy.RequireRole("Usuario"));
            //    //options.AddPolicy("UsuarioYAdministrador", policy => policy.RequireRole("Administrador").RequireRole("Usuario"));

            //    //Uso de claims
            //    //options.AddPolicy("AdministradorCrear", policy => policy.RequireRole("Administrador").RequireClaim("Crear", "True"));
            //   // options.AddPolicy("AdministradorEditarBorrar", policy => policy.RequireRole("Administrador").RequireClaim("Editar", "True").RequireClaim("Borrar", "True"));
            //   // options.AddPolicy("AdministradorCrearEditarBorrar", policy => policy.RequireRole("Administrador").RequireClaim("Crear", "True")
            //  //  .RequireClaim("Editar", "True").RequireClaim("Borrar", "True"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            //app.UseSwagger(c =>
            //{
            //    c.RouteTemplate = "ApiRest/swagger/{documentName}/swagger.json";
            //});
            app.UseSwaggerUI(c =>
            {
                //c.RoutePrefix = string.Empty;
                //c.DocumentTitle="ddddd";
                c.DefaultModelsExpandDepth(-1); //quitar schemas de la pantalla
                c.SwaggerEndpoint("./v1/swagger.json", "ApiRest v1");
           
            });


            app.Use(async (ctx, next) =>
            {
                await next();
                if (ctx.Response.StatusCode == 204)
                {
                    ctx.Response.ContentLength = 0;
                }
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseCors();
        }
    }
}
