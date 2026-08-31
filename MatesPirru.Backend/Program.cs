using MatesPirru.Backend.Service;
using MatesPirru.Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore; // <-- Agregamos la librería de Scalar
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Agregamos los recepcionistas (Controllers)
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
// Agregamos el generador de la API (Nativo de .NET 9)
builder.Services.AddOpenApi();

// Conectamos la base de datos
builder.Services.AddDbContext<MatesPirru.Backend.Data.AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ConexionSQL")));

// Conectamos tu negocio de Productos
builder.Services.AddScoped<IProductoService, ProductoService>();
// conectamos al negocio de Categoria
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
// conectamos al negocio de Usuarios
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
// conectamos al negocio de Pedido
builder.Services.AddScoped<IPedidoService, PedidoService>();

// PAra los tocken de los usuarios.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true, // Controla que el token no esté vencido
            ValidateIssuerSigningKey = true, // Exige que esté firmado con nuestra llave
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            RoleClaimType = ClaimTypes.Role // <-- AGREGAR ESTA LÍNEA MÁGICA

        };
    });

var app = builder.Build();

// Configuramos la página web de pruebas
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Genera los datos en segundo plano
    app.MapScalarApiReference(); // <-- Dibuja la nueva interfaz web moderna
}

app.UseHttpsRedirection();
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
        // Esto va a imprimir en la consola el motivo real por el cual falla al cargar el controlador
        Console.WriteLine($"---> ERROR DETALLADO: {subEx.Message}");
    }
    throw; // Vuelve a lanzar la excepción para mostrar el error exacto
}

app.Run();