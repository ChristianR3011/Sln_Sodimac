using Microsoft.EntityFrameworkCore;
using Sodimac.Api.Models.Contexts;
using Sodimac.Api.Repositories.Implementations;
using Sodimac.Api.Repositories.Interfaces;
using Sodimac.Api.Services.Implementations;
using Sodimac.Api.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddControllers();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<SodimacContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("conexion")));



// Program.cs
builder.Services.AddScoped<IEntregaService, EntregaService>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();
builder.Services.AddScoped<IPagoService, PagoService>();
builder.Services.AddScoped<IConfirmacionService, ConfirmacionService>();
builder.Services.AddSingleton<IClienteCarroService, ClienteCarroService>();

builder.Services.AddScoped<ICarroRepository, CarroRepository>();
builder.Services.AddScoped<IEntregaRepository, EntregaRepository>();
builder.Services.AddScoped<IComprobanteRepository, ComprobanteRepository>();
builder.Services.AddScoped<IPagoRepository, PagoRepository>();
builder.Services.AddScoped<IOrdenRepository, OrdenRepository>();
builder.Services.AddScoped<INotificacionRepository, NotificacionRepository>();


// ------------------------------------------------------------------------------------------------------------


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Sodimac API v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
