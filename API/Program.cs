using Core.Interfaces;
using Infrastructure.config;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. *** 

builder.Services.AddControllers();
builder.Services.AddDbContext<StoreContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// singleton = created when app starts and used until app shuts down
// transient = scoped to the method level (too soon for repository)
// scoped = exists through the lifetime of the http request
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// End Services

// Middleware begin ***

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapControllers();

// seed data

try
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<StoreContext>();
    await context.Database.MigrateAsync();
    await StoreContextSeed.SeedAsync(context);
}
catch(Exception)
{
    throw;
}

app.Run();
