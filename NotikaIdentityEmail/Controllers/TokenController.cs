using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NotikaIdentityEmail.Models.JwtModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NotikaIdentityEmail.Controllers
{
    public class TokenController : Controller
    {
        private readonly JwtSettingsModel _jwtSettingsModel;

        public TokenController(IOptions<JwtSettingsModel> jwtSettingsModel)
        {
            _jwtSettingsModel = jwtSettingsModel.Value;
        }
        [HttpGet]
        public IActionResult Generate()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Generate(SimpleUserViewModel simpleUserViewModel)
        {
            // Claim (hak/iddia) kavramı, JWT token içine eklenen küçük veri parçalarıdır.
            // Her claim, kullanıcı hakkında bilgi içerir ve uygulama tarafından doğrulama (authentication)
            // veya yetkilendirme (authorization) işlemlerinde kullanılır.

            // Aşağıda oluşturulan claim'ler, bir JWT token oluşturulurken içine eklenecek olan verilerdir.
            // Örneğin bir kullanıcı giriş yaptığında bu bilgiler token içine yazılır ve sonraki isteklerde taşınır.

            var claim = new[]
            {
        // Kullanıcının adı token içinde "Name" olarak taşınacak
        new Claim("Name", simpleUserViewModel.Name),

        // Kullanıcının soyadı token'a "Surname" olarak eklenir
        new Claim("Surname", simpleUserViewModel.Surname),

        // Kullanıcının yaşadığı şehir bilgisi eklenir
        new Claim("City", simpleUserViewModel.City),

        // Kullanıcının kullanıcı adı eklenir (sistemde eşsiz kullanıcıyı temsil edebilir)
        new Claim("Username", simpleUserViewModel.Username),

        // JTI (JWT ID): Token’a özel benzersiz bir kimlik numarasıdır, güvenlik ve izleme için kullanılır
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };
            // JWT token'ı imzalamak için kullanılacak güvenli anahtar oluşturuluyor.
            // _jwtSettingsModel.Key -> appsettings.json'daki gizli anahtar (secret key)
            // Encoding.UTF8.GetBytes(...) -> string anahtarı byte dizisine çevirir
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettingsModel.Key));

            // Token'ın dijital olarak imzalanabilmesi için gerekli olan imzalama bilgisi oluşturuluyor.
            // Bu satırda HMAC SHA256 algoritması ile simetrik anahtar kullanılarak imza işlemi yapılacağı belirtiliyor.
            // Bu imza, token'ın güvenliğini sağlar ve değiştirilmediğini ispatlar.
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettingsModel.Issuer,
                audience: _jwtSettingsModel.Audience,
                claims: claim,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettingsModel.ExpireMinutes),
                signingCredentials: creds);
            simpleUserViewModel.Token = new JwtSecurityTokenHandler().WriteToken(token);
            return View(simpleUserViewModel);
        }
    }
}
