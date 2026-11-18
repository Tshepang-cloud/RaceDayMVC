using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RacedayMvc.Services; // Ensures IApiService is found

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// Add Session Support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add HttpClient for API consumption
builder.Services.AddHttpClient("RaceDayApi", client =>
{
    // ⚠️ IMPORTANT: Check your API terminal and make sure this port is correct!
    client.BaseAddress = new Uri("http://localhost:5144/");
});

// Register custom API Service
builder.Services.AddScoped<IApiService, ApiService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// IMPORTANT: Use Session BEFORE Authorization
app.UseSession();
app.UseAuthorization();

// Redirect root URL to Login page
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();