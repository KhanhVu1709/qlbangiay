using Microsoft.AspNetCore.Authentication.Cookies;
using QlBanGiay.Service;

var builder = WebApplication.CreateBuilder(args);
//---
//đăng ký sử dụng mô hình mvc
builder.Services.AddControllersWithViews();
//đăng ký sử dụng session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(120);     // thời gian hết phiên
});

// đăng ký authen và authorization trong .net
//builder.Services.AddAuthentication();


//builder.Services.AddAuthorization();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IVnPayService, VnPayService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
	// Thiết lập thời gian sống của cookie (ExpireTimeSpan)
	options.ExpireTimeSpan = TimeSpan.FromMinutes(20);

	// Đường dẫn để chuyển hướng người dùng đến trang đăng nhập khi họ chưa xác thực
	options.LoginPath = "/Account/Login";

	// Đường dẫn để chuyển hướng người dùng khi họ không có quyền truy cập
	options.AccessDeniedPath = "/AccessDenied";
});

builder.Services.AddAuthorization(option =>
{
	option.AddPolicy("AnyRole", policy =>
	{
		policy.RequireRole("quản lý khách hàng", "quản lý đơn hàng", "quản lý sản phẩm");
	});
});

//---
var app = builder.Build();

//app.MapGet("/", () => "Hello World!");
//---
//sử dụng session
app.UseSession();
app.UseStaticFiles(); // muốn truy cập được các file trong thư mục wwwroot thì phải có dòng này
//sử dụng url
app.UseRouting();

// sử dụng authen, autho
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
            name: "area",
            pattern: "{area:exists}/{controller=Account}/{action=Login}/{id?}");
app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Products}/{action=TrangChu}/{id?}");
//---

app.Run();