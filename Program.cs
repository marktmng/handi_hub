using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using DotnetAPI.Data;
using DotnetAPI.Repository; // ✅ Import your DataContext class from the Data folder
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ✅ Register MVC Controller support (for APIs)
builder.Services.AddControllers();

// ✅ Register the DataContext using connection string from appsettings.json
builder.Services.AddDbContext<DataContext>();

// ✅ Register Repositories for Dependency Injection
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IArtistRepository, ArtistRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>(); // Register CustomerRepository
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>(); // Register CategoryRepository
builder.Services.AddScoped<IProductRepository, ProductRepository>(); // Register ProductRepository
builder.Services.AddScoped<IOrderRepository, OrderRepository>(); // Register OrderRepository
builder.Services.AddScoped<ICartRepository, CartRepository>(); // Register CartItemRepository
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

string? tokenKeyString = builder.Configuration.GetSection("AppSettings:TokenKey").Value;

// creating authentication token validation parameters
// pull out token key string
SymmetricSecurityKey tokenKey = new SymmetricSecurityKey( // symmetric key and passing to new symmetric key 
    Encoding.UTF8.GetBytes( // and passing to new symmetric key byte array
        tokenKeyString != null ? tokenKeyString : ""
        )
    );

// token validation parameters for application how to use it
TokenValidationParameters tokenValidationParameters = new TokenValidationParameters()
{
    IssuerSigningKey = tokenKey,
    ValidateIssuer = false,
    ValidateIssuerSigningKey = false,
    ValidateAudience = false
};

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // set authentication scheme
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = tokenValidationParameters;
    });

// ✅ Register Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Register CORS policies
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", corsBuilder =>
    {
        corsBuilder.WithOrigins(
            "http://localhost:4200",
            "http://localhost:3000",
            "http://localhost:8000"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });

    options.AddPolicy("ProdCors", corsBuilder =>
    {
        corsBuilder.WithOrigins("https://myProductionSite.com")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

var app = builder.Build();

// ✅ Enable Swagger only in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ✅ Enforce HTTPS
app.UseHttpsRedirection();

// ✅ Use CORS (choose policy based on environment)
app.UseCors(app.Environment.IsDevelopment() ? "DevCors" : "ProdCors");

// ✅ Use authorization (if using [Authorize] attributes)
app.UseAuthorization();

// ✅ Map controller routes
app.MapControllers();

// ✅ Run the web app
app.Run();
