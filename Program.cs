using InterRapidisimoBack.Application;
using InterRapidisimoBack.API.Swagger;
using InterRapidisimoBack.Infraestructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Falta configurar ConnectionStrings:DefaultConnection.");
var databasePassword = builder.Configuration["ConnectionStrings:Password"];
if (string.IsNullOrWhiteSpace(databasePassword))
{
    throw new InvalidOperationException("Falta configurar el secreto ConnectionStrings:Password.");
}

var sqlConnectionString = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connectionString)
{
    Password = databasePassword,
    Encrypt = true,
    TrustServerCertificate = builder.Environment.IsDevelopment()
};

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(sqlConnectionString.ConnectionString));
builder.Services.AddScoped<EstudianteService>();
builder.Services.AddScoped<CatalogService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
    options.OperationFilter<EstudianteCreateExampleOperationFilter>());
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseExceptionHandler(handler => handler.Run(async context =>
{
    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    var isBusinessRuleError = exception is BusinessRuleException;
    var statusCode = isBusinessRuleError
        ? StatusCodes.Status400BadRequest
        : StatusCodes.Status500InternalServerError;

    context.Response.StatusCode = statusCode;
    await context.Response.WriteAsJsonAsync(new ProblemDetails
    {
        Status = statusCode,
        Title = isBusinessRuleError ? "Regla de negocio incumplida" : "Error interno del servidor",
        Detail = isBusinessRuleError ? exception!.Message : "Ocurrió un error inesperado."
    });
}));

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
// CORS debe estar antes de MapControllers
app.UseCors("AngularPolicy");
app.MapControllers();

app.Run();
