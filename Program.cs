using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using DotnetAPI.Data;
using DotnetAPI.Repository; // ✅ Import your DataContext class from the Data folder

var builder = WebApplication.CreateBuilder(args);

// Register MVC Controller support (for APIs)
builder.Services.AddControllers();

// Register the DataContext as a service for dependency injection using IConfiguration manually
builder.Services.AddScoped<DataContext>();

// Register the UserRepository as a service for dependency injection
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Register Swagger for API documentation/testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger UI only in development mode
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect HTTP to HTTPS
app.UseHttpsRedirection();

// Use authorization middleware
app.UseAuthorization();

// Map HTTP routes to controller endpoints
app.MapControllers();

app.Run(); // 🚀 Run the web application
