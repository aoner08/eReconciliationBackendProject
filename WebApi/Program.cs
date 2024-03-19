using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Business.DependencyResolvers.Autofac;
using Core.DependencyResolvers;
using Core.Extensions;
using Core.Utilities.Security.Encryption;
using Core.Utilities.Security.JWT;
using DataAccess.Concrete.EntityFramework.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
//Aşağıdaki iki satırda AutofacBusinessModule'ı WebAPI'ye tanıttık
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory()); //1
builder.Host.ConfigureContainer<ContainerBuilder>(builder => builder.RegisterModule(new AutofacBusinessModule())); //2

// Add services to the container.

//CONFIGURATİON VE TokenOptions ayarlarını  buraya ekliyoruz!!!!

//builder.Services.AddDbContext<ContextDb>(options => //veri tabaný baðlantýsý
//{
//	options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
//});

IConfiguration configuration = builder.Configuration; //3 genel olarak proje ayarları

builder.Services.AddControllers(); //Program.cs'de varolan yapıydı
 
builder.Services.AddCors(options =>  //4 (güvenliği sağlamak için)
{
	options.AddPolicy("AllowOrigin", builder => builder.WithOrigins("https://localhost:7220"));
});

var tokenOptions = configuration.GetSection("TokenOptions").Get<TokenOptions>(); //5
//6
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => 
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = false, //true olursa expiration süresi dolduğunda token'i düşer.
		ValidIssuer= tokenOptions.Issuer,
		ValidAudience= tokenOptions.Audience,
		ValidateIssuerSigningKey = true,
		IssuerSigningKey=SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey),	
	};
});

builder.Services.AddDependencyResolvers(new  Core.Utilities.IoC.ICoreModule[] //6 
{
	new CoreModule(),
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

//CONFIGURATİON VE TokenOptions ayarlarının devamı::: Cors ekleyeceğiz
app.UseCors(builder => builder.WithOrigins("https://localhost:7220").AllowAnyHeader());//AllowAnyHeader ile bu adresten gelen tüm isteklier karşıla


app.UseHttpsRedirection();

app.UseAuthentication();	//giriş işlemi yapar aşağıdaki ise giriş işlemini yaptıktan sonra yetkilendirmeden sorumlu

app.UseAuthorization();

app.MapControllers();

app.Run();
