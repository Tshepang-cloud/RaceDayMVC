using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RacedayMvc.Services;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 1. Add HttpContextAccessor (needed to access Session in Views)
builder.Services.AddHttpContextAccessor();

// 2. Add Session Support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 3. Add HttpClient for API consumption
builder.Services.AddHttpClient("RaceDayApi", client =>
{
    // REPLACE WITH YOUR PART 2 API URL
    client.BaseAddress = new Uri("https://localhost:7000/");
});

// 4. Register your custom API Service
builder.Services.AddScoped<IApiService, ApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// 5. IMPORTANT: Use Session BEFORE Authorization
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();