using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NET_MVC.Data;
using NET_MVC.Interface;
using System.Diagnostics;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Thêm dịch vụ Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                // Log này chắc chắn sẽ hiện ở cửa sổ "Output" (chọn Show output from: Debug)
                Debug.WriteLine("Lỗi Auth thật sự đây: " + context.Exception.Message);
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                var authFailure = context.AuthenticateFailure;
                if (authFailure != null)
                {
                    // Nếu nó không Null, lỗi nằm ở đây (hết hạn, sai key, v.v.)
                    System.Diagnostics.Debug.WriteLine("Lý do thất bại: " + authFailure.Message);
                }
                else
                {
                    // Nếu nó Null, nghĩa là nó CÒN CHƯA ĐỌC ĐƯỢC Token từ Header
                    System.Diagnostics.Debug.WriteLine("Không tìm thấy Token trong Header!");
                }
                // Ngắt luồng chuyển hướng mặc định của MVC
                context.HandleResponse();
                // Ép trả về mã 401 và JSON
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                return context.Response.WriteAsync("{\"message\": \"Mày không có Token hoặc Token hết hạn rồi!\"}");
            }
        };
    });

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            // Lấy các lỗi validation ra
            var errors = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            var response = new ApiResponse<object>(
                status: false,
                code: 400,
                message: "Tham số truyền vào không hợp lệ!",
                data: errors
            );

            return new BadRequestObjectResult(response);
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.builder.Services
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

//app.MapStaticAssets();
app.UseStaticFiles();   // ⚠️ CÁI NÀY PHẢI CÓ

// Kích hoạt Middleware (Thứ tự rất quan trọng!)
app.UseAuthentication(); // Xác thực xem mày là ai?
app.UseAuthorization();  // Mày có quyền làm gì?

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
