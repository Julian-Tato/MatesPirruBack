using MatesPirru.Backend.Service;
using MatesPirru.Backend.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore; // <-- Agregamos la librería de Scalar

var builder = WebApplication.CreateBuilder(args);

// Agregamos los recepcionistas (Controllers)
builder.Services.AddControllers();

// Agregamos el generador de la API (Nativo de .NET 9)
builder.Services.AddOpenApi();

// Conectamos la base de datos
builder.Services.AddDbContext<MatesPirru.Backend.Data.AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ConexionSQL")));

// Conectamos tu negocio de Productos
builder.Services.AddScoped<IProductoService, ProductoService>();

var app = builder.Build();

// Configuramos la página web de pruebas
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Genera los datos en segundo plano
    app.MapScalarApiReference(); // <-- Dibuja la nueva interfaz web moderna
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();