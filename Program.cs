using ASP_SPR311.Data;
using ASP_SPR311.Services.Kdf;
using ASP_SPR311.Services.Timestamp;
using ASP_SPR311.Middleware;
using Microsoft.EntityFrameworkCore;
using ASP_SPR311.Services.Storage;
using Microsoft.AspNetCore.Mvc.Rendering;
using ASP_SPR311.Services.ProductService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//builder.Services.AddSingleton<ITimestampService, SystemTimestampService>();
builder.Services.AddSingleton<ITimestampService, UnixTimestampService>();

builder.Services.AddSingleton<IkdfService, PbKdf1Service>();
builder.Services.AddSingleton<IStorageService, FileStorageService>();

builder.Services.AddScoped<IPopularProductService, PopularProductService>();

builder.Services.AddSingleton<OtpService>();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<DataContext>(
    options =>
        options.
        UseSqlServer(
            builder.Configuration
            .GetConnectionString("LocalMs")
        )
);

builder.Services.AddScoped<DataAccessor>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: "CorsPolicy",
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.UseSession();

app.UseAuthSession();

app.UseAuthToken();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
