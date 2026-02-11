using Microsoft.EntityFrameworkCore;
using Cineflow.Data;
using Cineflow.Services;
using Cineflow.Middleware;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configuração de Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Controllers (MVC) com configuração de JSON para evitar ciclos
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });


//Services
builder.Services.AddScoped<ITmdbService, TmdbService>();
builder.Services.AddScoped<IFilmeService, FilmeService>();
builder.Services.AddScoped<ISessaoService, SessaoService>();
builder.Services.AddScoped<IIngressoService, IngressoService>();
builder.Services.AddScoped<ISalaService, SalaService>();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("default", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});




var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("\n" +
    "═══════════════════════════════════════════════════════════════\n" +
    "   🎬 CineFlow API - Sistema de Gestão de Cinema\n" +
    "═══════════════════════════════════════════════════════════════\n" +
    "   🚀 Servidor iniciado\n" +
    "   📍 Ambiente: {Environment}\n" +
    "   🔗 URLs: {Urls}\n" +
    "═══════════════════════════════════════════════════════════════\n",
    app.Environment.EnvironmentName,
    string.Join(", ", app.Urls));

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    logger.LogInformation("Aplicando migrações do banco de dados...");
    db.Database.Migrate();
    logger.LogInformation("Banco de dados pronto!");

    var tmdbService = scope.ServiceProvider.GetRequiredService<ITmdbService>();
    var seederLogger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseSeeder>>();
    var seeder = new DatabaseSeeder(db, tmdbService, seederLogger);
    await seeder.SeedAsync();
    logger.LogInformation("");
}

app.UseMiddleware<RequestLoggingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("default");


app.UseSwagger();
app.UseSwaggerUI();



app.UseAuthorization();

app.MapControllers();

app.Run();
