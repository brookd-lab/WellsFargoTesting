using ATMBankingAPI.Data;
using ATMBankingAPI.Interfaces;
using ATMBankingAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure Entity Framework with In-Memory Database
builder.Services.AddDbContext<BankingContext>(options =>
    options.UseInMemoryDatabase("ATMBankingDB"));

// Register Repository (Dependency Injection)
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

// Add CORS for your Angular app
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Your Angular dev server
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ATM Banking API",
        Version = "v1",
        Description = "Banking API for ATM System with Account Validation"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ATM Banking API V1");
        c.RoutePrefix = "swagger"; // Access via /swagger
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAngularApp");
app.UseAuthorization();
app.MapControllers();

// Initialize database with seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BankingContext>();
    context.Database.EnsureCreated();
}

app.Run();
