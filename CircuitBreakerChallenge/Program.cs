using CircuitBreaker.Services.Implementations;
using CircuitBreaker.Services.Interfaces;
using CircuitBreakerChallenge.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IOrderService, OrderService>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<ICircuitStatesService, CircuitStatesService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseCircuitBreakerMiddleware();

app.Run();
