using AutoMapper;
using Ceplan.Backend.Siga.Application.Command;
using Ceplan.Backend.Siga.Application.Mapper;
using Ceplan.Backend.Siga.Application.Query;
using Ceplan.Backend.Siga.Domain.Core;
using Ceplan.Backend.Siga.Domain.Interface;
using Ceplan.Backend.Siga.Infraestructure.Data;
using Ceplan.Backend.Siga.Infraestructure.Interface;
using Ceplan.Backend.Siga.Infraestructure.Repository;
using Ceplan.Backend.Siga.Rest.Controllers;
using Ceplan.Backend.Siga.Transversal.Configuration;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

//creacion de la api
var builder = WebApplication.CreateBuilder(args);

// agregar los servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region IOC

#region Mapper IOC
//configuracion de la capa de mapeo de la arqui
//definicion de mapas entre modelos dtos y identidades
var mapperConfiguration = new MapperConfiguration(mc =>
{
    mc.AddProfile(new ConsultaProfile());
    mc.AddProfile(new AsignacionConsultaProfile());
    mc.AddProfile(new AsignacionBienProfile());
    mc.AddProfile(new DesplaProfile());
    mc.AddProfile(new RrhhProfile());
});
IMapper mapper = mapperConfiguration.CreateMapper(); //creacion de una instacia del automape que se utilizara en toda la api es definicion de la interfaz que luego se implementa
builder.Services.AddSingleton(mapper); //registro en el contenedor de inversion de control con singleton - una sola creacion para toda la vida de la app

#endregion

#region Settings

builder.Services.Configure<DbAppSettings>(builder.Configuration.GetSection("ConnectionStrings")); //mapeando una sesion del appsetting a una clase fuerte tipa

builder.Services.Configure<AppSettings>(builder.Configuration); //mapeas todo el archivo appsetting

#endregion

#region Data IOC
builder.Services.AddSingleton<IConnectionFactorySqlServer, ConnectionFactorySqlServer>();
//centraliza la creacion de conexion a una sola clase 
#endregion

#region Infraestructure IOC
//capa de acceso a datos - ejecucion del sp
builder.Services.AddScoped<IConsultaRepository, ConsultaRepository>();
builder.Services.AddScoped<IAsignacionConsultaRepository, AsignacionConsultaRepository>();
builder.Services.AddScoped<IAsignacionBienRepository, AsignacionBienRepository>();
builder.Services.AddScoped<IDesplaRepository, DesplaRepository>();
builder.Services.AddScoped<IRrhhRepository, RrhhRepository>();
builder.Services.AddScoped<IFormatosRepository, FormatosRepository>();
#endregion

#region Domain IOC
//logica y reglas que no pertenecen al negocio y es un intermedio entre la capa controller y repo
builder.Services.AddScoped<IConsultaDomain, ConsultaDomain>();
builder.Services.AddScoped<IAsignacionConsultaDomain, AsignacionConsultaDomain>();
builder.Services.AddScoped<IAsignacionBienDomain, AsignacionBienDomain>();
builder.Services.AddScoped<IDesplaDomain, DesplaDomain>();
builder.Services.AddScoped<IRrhhDomain, RrhhDomain>();
#endregion

#region Application
//definicion del mediador que permite enviar comandos, consultas y ejecutar sp 
builder.Services.AddMediatR(typeof(IniCommand).Assembly);
builder.Services.AddMediatR(typeof(IniQuery).Assembly);
#endregion

#region Utils
#endregion

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.DefaultModelsExpandDepth(-1);
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.Run();
