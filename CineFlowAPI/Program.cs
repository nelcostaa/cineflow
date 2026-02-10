using Microsoft.EntityFrameworkCore;
using Cineflow.Data;
using Cineflow.Services;

var builder = WebApplication.CreateBuilder(args);

// Controllers (MVC)
builder.Services.AddControllers();


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
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);




var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}


app.UseHttpsRedirection();

app.UseCors("default");

// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
