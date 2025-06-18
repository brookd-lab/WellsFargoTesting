using Advanced;

var builder = WebApplication.CreateBuilder(args);

// Register our service with DI container
builder.Services.AddScoped<IGreetingService, GreetingService>();

var app = builder.Build();

// Simple test endpoint to verify DI is working
app.MapGet("/greeting/{name}", (string name, IGreetingService greetingService) =>
{
    return greetingService.GetGreeting(name);
});

app.Run();
