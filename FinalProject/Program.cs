using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Flights.Data;
using Flights.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("https://localhost:44492")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<Entities>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Flights")));


// Add session services
builder.Services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set the timeout for the session
    options.Cookie.HttpOnly = true; // Make the session cookie HTTP only
    options.Cookie.IsEssential = true; // Make the session cookie essential
});

// Add Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.DescribeAllParametersInCamelCase();
    c.AddServer(new OpenApiServer
    {
        Description = "Development Server",
        Url = "https://localhost:7280"
    });
    c.CustomOperationIds(e => $"{e.ActionDescriptor.RouteValues["action"] + e.ActionDescriptor.RouteValues["controller"]}");
});

builder.Services.AddScoped<Entities>();

var app = builder.Build();

// Ensure the database is created and seed data
using (var scope = app.Services.CreateScope())
{
    var entities = scope.ServiceProvider.GetService<Entities>();

    entities.Database.EnsureCreated();

    var random = new Random();

    if (!entities.Flights.Any())
    {
        Flight[] flightsToSeed = new Flight[]
        {
            new (Guid.NewGuid(), "American Airlines", random.Next(90, 5000).ToString(), new TimePlace("Los Angeles", DateTime.Now.AddHours(random.Next(1, 3))), new TimePlace("Istanbul", DateTime.Now.AddHours(random.Next(4, 10))), 2),
            new (Guid.NewGuid(), "Deutsche BA", random.Next(90, 5000).ToString(), new TimePlace("Munchen", DateTime.Now.AddHours(random.Next(1, 10))), new TimePlace("Schiphol", DateTime.Now.AddHours(random.Next(4, 15))), random.Next(1, 853)),
            new (Guid.NewGuid(), "British Airways", random.Next(90, 5000).ToString(), new TimePlace("London, England", DateTime.Now.AddHours(random.Next(1, 15))), new TimePlace("Vizzola-Ticino", DateTime.Now.AddHours(random.Next(4, 18))), random.Next(1, 853)),
            new (Guid.NewGuid(), "Basiq Air", random.Next(90, 5000).ToString(), new TimePlace("Amsterdam", DateTime.Now.AddHours(random.Next(1, 21))), new TimePlace("Glasgow, Scotland", DateTime.Now.AddHours(random.Next(4, 21))), random.Next(1, 853)),
            new (Guid.NewGuid(), "BB Heliag", random.Next(90, 5000).ToString(), new TimePlace("Zurich", DateTime.Now.AddHours(random.Next(1, 23))), new TimePlace("Baku", DateTime.Now.AddHours(random.Next(4, 25))), random.Next(1, 853)),
            new (Guid.NewGuid(), "Adria Airways", random.Next(90, 5000).ToString(), new TimePlace("Ljubljana", DateTime.Now.AddHours(random.Next(1, 15))), new TimePlace("Warsaw", DateTime.Now.AddHours(random.Next(4, 19))), random.Next(1, 853)),
            new (Guid.NewGuid(), "ABA Air", random.Next(90, 5000).ToString(), new TimePlace("Praha Ruzyne", DateTime.Now.AddHours(random.Next(1, 55))), new TimePlace("Paris", DateTime.Now.AddHours(random.Next(4, 58))), random.Next(1, 853)),
            new (Guid.NewGuid(), "AB Corporate Aviation", random.Next(90, 5000).ToString(), new TimePlace("Le Bourget", DateTime.Now.AddHours(random.Next(1, 58))), new TimePlace("Zagreb", DateTime.Now.AddHours(random.Next(4, 60))), random.Next(1, 853))
        };
        entities.Flights.AddRange(flightsToSeed);
        entities.SaveChanges();
    }
}

// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Enable session
app.UseSession();

app.UseCors(builder => builder
    .WithOrigins("*")
    .AllowAnyMethod()
    .AllowAnyHeader()
);

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();
