using Microsoft.OpenApi.Models;
using FinalProject.Domain.Interfaces.Services;
using FinalProject.Core.Services;
using FinalProject.Domain.Interfaces.Repositories;
using FinalProject.Infrastructure.Repositories;
using FinalProject.Core.Common;
using Credo.Core.Shared.Middleware;
using Serilog;
using FinalProject.Domain.Common;
using Amazon.SimpleEmail;
using Amazon;

var builder = WebApplication.CreateBuilder(args);

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

//Add Logging
builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration)
        .WriteTo.Console())
    .ConfigureAppConfiguration((hostContext, builder) =>
    {
        builder.AddEnvironmentVariables();
    });



// Add services to the container.
builder.Services.AddControllersWithViews();

// Add session services
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.DescribeAllParametersInCamelCase();
    c.AddServer(new OpenApiServer
    {
        Description = "Development Server",
        Url = "https://skyc0nnect.com/" // Adjust as needed
    });
    c.CustomOperationIds(e => $"{e.ActionDescriptor.RouteValues["action"] + e.ActionDescriptor.RouteValues["controller"]}");
});

// Register services
builder.Services.AddSingleton<IAmazonSimpleEmailService>(sp =>
    new AmazonSimpleEmailServiceClient(RegionEndpoint.EUNorth1));
builder.Services.Configure<AviationStackSettings>(builder.Configuration.GetSection("AviationStack"));
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IFlightRepository, FlightRepository>();
builder.Services.AddScoped<IPassengerRepository, PassengerRepository>();
builder.Services.AddScoped<IPacketRepository, PacketRepository>();
builder.Services.AddScoped<IMapRepository, MapRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IFlightService, FlightService>();
builder.Services.AddScoped<IPassengerService, PassengerService>();
builder.Services.AddScoped<IPacketService, PacketService>();
builder.Services.AddScoped<IMapService, MapService>();
builder.Services.AddScoped<IHelper, Helper>();
builder.Services.AddSingleton<IUserContext, UserContext>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();

var app = builder.Build();

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

// Use the CORS policy
app.UseCors("AllowAll");

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware(typeof(LogMiddleware));
app.UseMiddleware(typeof(ErrorHandlingMiddleware));
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();
