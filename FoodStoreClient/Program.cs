using FoodStoreAPI.Models;
using FoodStoreClient.Pages.Guest.Verify_Email;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Thêm dịch vụ Razor Pages & HttpClient
builder.Services.AddRazorPages();
builder.Services.AddHttpClient();


// Cấu hình Session

builder.Services.AddDistributedMemoryCache();
builder.Services.AddTransient<EmailService>();
builder.Services.AddTransient<ForgotPasswordService>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian hết hạn session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddDbContext<FoodStoreContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("value")));

var app = builder.Build();

// Cấu hình Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();
app.UseRouting();

// Kích hoạt Session
app.UseSession();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapGet("/", context =>
    {
        context.Response.Redirect("/Guest/Restoran_Home/Home");
        return Task.CompletedTask;
    });
});

app.Run();
