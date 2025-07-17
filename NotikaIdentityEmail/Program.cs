using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NotikaIdentityEmail.Context;
using NotikaIdentityEmail.Entities;
using NotikaIdentityEmail.Models.IdentityModels;
using NotikaIdentityEmail.Models.JwtModels;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EmailContext>();
builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<EmailContext>()
    .AddErrorDescriber<CustomIdentityValidator>();

builder.Services.Configure<JwtSettingsModel>(builder.Configuration.GetSection("JwtSettingsKey"));

// Uygulaman�n servislerine JWT tabanl� kimlik do�rulama (authentication) ekleniyor.
builder.Services.AddAuthentication(options =>
{
    // Varsay�lan kimlik do�rulama �emas� olarak JWT Bearer belirleniyor.
    // Yani API istekleri "Authorization: Bearer <token>" ba�l���yla g�nderildi�inde bu sistem devreye girer.
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

    // Kimlik do�rulama ba�ar�s�z olursa da JWT Bearer �emas� kullan�lacak (�rne�in 401 Unauthorized d�n���).
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

})
// JWT Bearer kimlik do�rulama sistemi yap�land�r�l�yor.
.AddJwtBearer(opt =>
{
    // JWT ayarlar� (anahtar, s�re vb.) appsettings.json i�indeki "JwtSettingsKey" ba�l��� alt�ndan okunur.
    // Bu sat�r, JWT ile ilgili yap�land�rma ayarlar�n� bir modele (JwtSettingsModel) bind eder.
    var jwtSettings = builder.Configuration.GetSection("JwtSettingsKey").Get<JwtSettingsModel>();
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Issuer kontrol� yap�lacak m�?
        ValidIssuer = jwtSettings.Issuer, // Ge�erli issuer de�eri

        ValidateAudience = true, // Audience kontrol� yap�lacak m�?
        ValidAudience = jwtSettings.Audience, // Ge�erli audience de�eri

        ValidateLifetime = true, // Token s�resi kontrol edilsin mi?
        ValidateIssuerSigningKey = true, // Token imzas� kontrol edilsin mi?

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)), // Anahtar
        ClockSkew = TimeSpan.Zero // S�re sapmas� (iste�e ba�l�)
    };

   
});


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStatusCodePagesWithReExecute("/ErrorPage/HandleError", "?statusCode={0}");


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
