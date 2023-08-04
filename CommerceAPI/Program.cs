using CommerceAPI.DataAccess;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<CommerceApiContext>(options =>
    options
        .UseNpgsql(builder.Configuration.GetConnectionString("CommerceApiDb"))
        .UseSnakeCaseNamingConvention()

    );

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
