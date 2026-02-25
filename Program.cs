using BibliotecaVirtual.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de la Base de Datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. CONFIGURACIÓN DE CORS (Indispensable para Blazor)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 3. Configuración del pipeline (EL ORDEN IMPORTA)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 4. HABILITAR ARCHIVOS ESTÁTICOS (Para ver las imágenes)
app.UseStaticFiles();

// 5. HABILITAR CORS (Debe ir después de StaticFiles y antes de MapControllers)
app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();