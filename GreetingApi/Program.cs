using GreetingApi.Factory;
using GreetingApi.Services.Cache;
using GreetingApi.Services.CreditCardPayment;
using GreetingApi.Services.Greeting;
using GreetingApi.Services.Inventory;
using GreetingApi.Services.Order;
using GreetingApi.Services.PaymentServices;
using GreetingApi.Services.UserData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Payment Processing API",
        Version = "v1",
        Description = "A comprehensive API for payment processing with dependency injection examples"
    });
});

// Existing registrations

// This should now compile but will cause a circular dependency exception at startup
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();

// Register services - no circular dependency now!
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderValidator>(provider => provider.GetRequiredService<IOrderService>() as IOrderValidator);
builder.Services.AddScoped<IInventoryService, InventoryService>();


builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddScoped<IUserDataService, UserDataService>();
builder.Services.AddScoped<IGreetingService, GreetingService>();

// Register all payment services
builder.Services.AddTransient<CreditCardPaymentService>();
builder.Services.AddTransient<PayPalPaymentService>();
builder.Services.AddTransient<BankTransferPaymentService>();

// Register factory as Scoped
builder.Services.AddScoped<IPaymentServiceFactory, PaymentServiceFactory>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment Processing API v1");
        c.RoutePrefix = string.Empty; // Makes Swagger UI available at root URL
    });
}

app.UseHttpsRedirection();

// Original endpoints
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
});

// Single service injection
app.MapGet("/greeting/{name}", (string name, [FromServices] IGreetingService greetingService) =>
{
    return new { Greeting1 = greetingService.GetGreeting(name) };
});

// Double service injection in same request
app.MapGet("/double-greeting/{name}", (string name, [FromServices] IGreetingService service1, [FromServices] IGreetingService service2) =>
{
    return new
    {
        Greeting1 = service1.GetGreeting(name),
        Greeting2 = service2.GetGreeting(name),
        SameInstance = ReferenceEquals(service1, service2)
    };
});

app.MapPost("/payment", (PaymentRequest request, [FromServices] IPaymentServiceFactory factory) =>
{
    var paymentService = factory.CreatePaymentService(request);
    var result = paymentService.ProcessPayment(request.Amount, request.Currency);

    return new
    {
        Provider = paymentService.GetProviderName(),
        Result = result,
        RequestDetails = request
    };
})
.WithName("ProcessPayment")
.WithSummary("Process a payment using the appropriate payment provider")
.WithDescription("Automatically selects payment provider based on amount and user type:\n" +
                "- Amounts >= $10,000: Bank Transfer\n" +
                "- Premium users: PayPal\n" +
                "- Default: Credit Card")
.WithOpenApi();


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
