using MatesPirru.Backend.Service;
using MatesPirru.Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Agregamos los recepcionistas (Controllers) y manejamos ciclos de referencias
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Generador de la API (Nativo de .NET 9)
builder.Services.AddOpenApi();

// Conectamos la base de datos SQLite
builder.Services.AddDbContext<MatesPirru.Backend.Data.AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ConexionSQL")));

// Inyección de dependencias de los servicios
builder.Services.AddScoped<IProductoService, ProductoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IPedidoService, PedidoService>();

// Configuración de Autenticación JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            RoleClaimType = ClaimTypes.Role
        };
    });

// Configuración de CORS para React
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirReact", app =>
    {
        app.AllowAnyOrigin()
           .AllowAnyHeader()
           .AllowAnyMethod();
    });
});

var app = builder.Build();

// Interfaz visual de documentación moderna (Scalar) en entorno de desarrollo
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors("PermitirReact");
app.UseAuthentication();
app.UseAuthorization();

try
{
    app.MapControllers();
}
catch (System.Reflection.ReflectionTypeLoadException ex)
{
    foreach (var subEx in ex.LoaderExceptions)
    {
        Console.WriteLine($"---> ERROR DETALLADO: {subEx.Message}");
    }
    throw;
}

app.Run();