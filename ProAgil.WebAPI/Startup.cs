﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.EntityFrameworkCore;
using ProAgil.Repository;
using AutoMapper;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using ProAgil.Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ProAgil.WebAPI
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
      services.AddDbContext<ProAgilContext>(
        x => x.UseSqlite(Configuration.GetConnectionString("DefaultConnection"))
      );

      //Configuração geral
      IdentityBuilder builder = services.AddIdentityCore<User>(options => {
        //SENHA
        //Resetando valores padrões da validação de senha
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 4;
      });
      //Instância do IdentityBuilder criado anteriormente(builder.Services)
      //Configurações do Context, das Roles e dos Usuários
      builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
      builder.AddEntityFrameworkStores<ProAgilContext>();
      builder.AddRoleValidator<RoleValidator<Role>>();
      builder.AddRoleManager<RoleManager<Role>>();
      builder.AddSignInManager<SignInManager<User>>();

      //Configuração do JWT (Json Web Token)
      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters{
          //Assinatura da chave do emissor da api
          ValidateIssuerSigningKey = true,
          //Descriptografa a chave(Token) que estiver definido em AppSettings
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
          
          ValidateAudience = false,
          ValidateIssuer = false
        };
      });

      //Determina qual Controller será chamada
      services.AddMvc(options => {
          //Toda vez que uma rota for chamada, ele vai requerir que o usuário esteja autenticado
          //Em sequência irá usar o AuthorizeFilter para filtrar todas as chamadas que tiver
          var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
          options.Filters.Add(new AuthorizeFilter(policy));

        }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
        //O AddJsonOptions controla qualquer redundância que houver em relação ao retorno da serialização dos itens. Resolve qualquer looping infinito que houver entre a relação das Entidades

      services.AddScoped<IProAgilRepository, ProAgilRepository>();
      //Definindo que a aplicação irá trabalhar com o AutoMapper
      services.AddAutoMapper();
      services.AddCors();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }


      app.UseAuthentication();
      //app.UseHttpsRedirection();
      app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
      app.UseStaticFiles();
      app.UseStaticFiles(new StaticFileOptions(){
        FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
        RequestPath = new PathString("/Resources")
      });
      app.UseMvc();
    }
  }
}
