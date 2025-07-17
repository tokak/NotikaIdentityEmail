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

// Uygulamanýn servislerine JWT tabanlý kimlik doðrulama (authentication) ekleniyor.
builder.Services.AddAuthentication(options =>
{
    // Varsayýlan kimlik doðrulama þemasý olarak JWT Bearer belirleniyor.
    // Yani API istekleri "Authorization: Bearer <token>" baþlýðýyla gönderildiðinde bu sistem devreye girer.
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

    // Kimlik doðrulama baþarýsýz olursa da JWT Bearer þemasý kullanýlacak (örneðin 401 Unauthorized dönüþü).
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

})
// JWT Bearer kimlik doðrulama sistemi yapýlandýrýlýyor.
.AddJwtBearer(opt =>
{
    // JWT ayarlarý (anahtar, süre vb.) appsettings.json içindeki "JwtSettingsKey" baþlýðý altýndan okunur.
    // Bu satýr, JWT ile ilgili yapýlandýrma ayarlarýný bir modele (JwtSettingsModel) bind eder.
    var jwtSettings = builder.Configuration.GetSection("JwtSettingsKey").Get<JwtSettingsModel>();
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Issuer kontrolü yapýlacak mý?
        ValidIssuer = jwtSettings.Issuer, // Geçerli issuer deðeri

        ValidateAudience = true, // Audience kontrolü yapýlacak mý?
        ValidAudience = jwtSettings.Audience, // Geçerli audience deðeri

        ValidateLifetime = true, // Token süresi kontrol edilsin mi?
        ValidateIssuerSigningKey = true, // Token imzasý kontrol edilsin mi?

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)), // Anahtar
        ClockSkew = TimeSpan.Zero // Süre sapmasý (isteðe baðlý)
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
