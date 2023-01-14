using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using MedAdvisor.Api.Dtos;
using MedAdvisor.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using MedAdvisor.DataAccess.MySql.DataContext;
using Microsoft.VisualBasic;
using System.Globalization;
using Superpower;

namespace MedAdvisor.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _db;
    public AuthController(IConfiguration config, AppDbContext db)
    {
        _configuration = config;
        _db = db;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto requestDto)
    {
        if (ModelState.IsValid)
        {
            var user_exist = await _db.Users.FirstOrDefaultAsync(
                    user => user.Email == requestDto.Email);


            if (user_exist == null)
            {
                var encoder = new HMACSHA512();
                byte[] passwordSalt = encoder.Key;
                byte[] passwordHash = encoder.ComputeHash(
                        Encoding.UTF8.GetBytes(requestDto.Password));

                // creating new user
                var new_user = new User()
                {
                    Email = requestDto.Email,
                    FullName = requestDto.FirstName + " " + requestDto.LastName,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt
                };
                await _db.Users.AddAsync(new_user);
                await _db.SaveChangesAsync();

                return Ok(new 
                    { 
                        status = "success",
                        user =  new_user
                    });
            }
            return BadRequest(" email alerady exists ");
        }
        return BadRequest();
    }



    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto model)
    {
        var user = await _db.Users.Include(u=>u.Allergies).FirstOrDefaultAsync(
                            user => user.Email == model.Email); 
        
        if (user != null)
        {
            // hashing imput user password
            var encode = new HMACSHA512(user.PasswordSalt);
            var computedhash = encode.ComputeHash(
                Encoding.UTF8.GetBytes(model.Password));

            // comparing hashed password with user.password
            if (!computedhash.SequenceEqual(user.PasswordHash))
                return BadRequest("invalid credentials!");

            return Ok(new
            {
                status = "success",
                user,
                token = CreateToken(user)
            });
        }
        return BadRequest("user does not exist!");
       ;
    }


    // creating user token
    private string CreateToken(User user)
    {

        var claims = new List<Claim>{
             new Claim("Id",user.Id.ToString()),
             new Claim(ClaimTypes.Email, user.Email),
             new Claim(ClaimTypes.Name, user.FullName),
        };

        var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JWT:Secret").Value);
        var SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature);

          var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(45),
            signingCredentials: SigningCredentials
        );

        string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
        return jwtToken;
    }

}















//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//using MedAdvisor.Api.Dtos;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.IdentityModel.Tokens;


//namespace MedAdvisor.Api.Controllers
//{
//    [Route("api/[controller]")]   // api/auth
//    [ApiController]
//    public class AuthenticationController : ControllerBase
//    {

//        private readonly UserManager<IdentityUser> _userManager;
//        private readonly IConfiguration _configeration;

//        public AuthenticationController(IConfiguration configeration,
//            UserManager<IdentityUser> userManager
//            )
//        {
//            _configeration = configeration;
//            _userManager = userManager;
//        }



//        [HttpPost]
//        [Route("register")]
//        public async Task<IActionResult> Register([FromBody] UserRegistrationRequestDto reqDto)
//        {


//            // validating incomming reques 
//            if (ModelState.IsValid)
//            {

//                // check if email exists
//                var user_exist = await _userManager.FindByEmailAsync(reqDto.Email);
//                if (user_exist != null)
//                {
//                    return BadRequest(" email alerady exists ");
//                }

//                // create user if email is null 
//                var new_user = new IdentityUser()
//                {
//                    Email = reqDto.Email,
//                    UserName = reqDto.Email,
//                };

//                var is_created = await _userManager.CreateAsync(new_user, reqDto.Password);
//                if (is_created.Succeeded)
//                {
//                    var token = GenerateIdentityToken(new_user);
//                    return Ok(token);
//                }
//                return BadRequest("server error gax");

//            }
//            return BadRequest();

//        }


//        // generating token for user 
//        private string GenerateIdentityToken(IdentityUser user)
//        {
//            var jwtTokenHandler = new JwtSecurityTokenHandler();
//            var key = Encoding.UTF8.GetBytes(_configeration.GetSection("JWT:Secret").Value);

//            var tokenDescriptor = new SecurityTokenDescriptor()
//            {
//                Subject = new ClaimsIdentity(
//                    new[]
//                    {
//                    new Claim("id", user.Id),
//                    new Claim(JwtRegisteredClaimNames.Sub,user.Email),
//                    new Claim(JwtRegisteredClaimNames.Email,user.Email),
//                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
//                    }
//                    ),
//                Expires = DateTime.Now.AddHours(1),
//                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)

//            };

//         var token = jwtTokenHandler.CreateToken(tokenDescriptor);
//         string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
//         return jwtToken;
//        }

//    }
//}
