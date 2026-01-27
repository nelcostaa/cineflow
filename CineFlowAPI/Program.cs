var builder = WebApplication.CreateBuilder(args);

// Controllers (MVC)
builder.Services.AddControllers();

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

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("default");

// Swagger (eu recomendo deixar em Dev só)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Se você quiser Swagger sempre (mesmo em container Production), use:
// app.UseSwagger();
// app.UseSwaggerUI();

app.UseAuthorization();

// Rotas dos controllers
app.MapControllers();

// (Opcional) rota básica pra testar
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
