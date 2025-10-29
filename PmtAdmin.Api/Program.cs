using PmtAdmin.Api.Middleware;
using PmtAdmin.Application;
using PmtAdmin.Infrastructure;
using PmtAdmin.Infrastructure.Context.Seeding;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCorsPolicy", policy =>
    {
        // Allow frontend Angular dev server (both HTTP and HTTPS)
        policy.WithOrigins(
                "http://localhost:4200",
                "https://localhost:4200"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();  // Enable credentials for cookies/auth
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Application and Infrastructure services
builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);

// Register the DataSeedingService
//builder.Services.AddHostedService<DataSeedingService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

// Enable CORS for development
app.UseCors("DevCorsPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
