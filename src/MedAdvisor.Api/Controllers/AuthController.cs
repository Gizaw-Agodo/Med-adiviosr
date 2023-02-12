using MedAdvisor.DataAccess.MySql.Repositories.Users;
using MedAdvisor.DataAccess.MySql.DataContext;
using MedAdvisor.Services.Okta.Interfaces;
using System.Security.Cryptography;
using MedAdvisor.Api.Responses;
using Microsoft.AspNetCore.Mvc;
using MedAdvisor.Api.Dtos;
using MedAdvisor.Models;
using System.Text;
using Microsoft.AspNetCore.Identity;
using rentX.Common.Email;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using MimeKit;
using MedAdvisor.Commons.Email;
using Microsoft.Extensions.Options;
using AutoMapper.Internal;

namespace MedAdvisor.Api.Controllers;

[ApiController]
[Route("api/user/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IUserServices _userService;
    private readonly IAuthService _authService;

    public AuthController(
        IUserRepository userRepository,
        IUserServices userService,
        IAuthService authService
       
        )
    {
        _userRepository = userRepository;
        _authService = authService;
        _userService = userService;
    }

   

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationDto requestDto)
    {
        if (ModelState.IsValid)
        {
            var user_exist = await _userService.GetUserByEmail(requestDto.Email);
            
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

                 var user = await _userRepository.AddUserAsync(new_user);
                return Ok(new RegistrationResponse("success",user));
            }
            return BadRequest(" email alerady exists ");
        }
        return BadRequest();
    }



    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto model)
    {

        if (ModelState.IsValid)
        {
            var user = await _userService.FetchUserData(model.Email);
            if (user != null)
            {
                var encode = new HMACSHA512(user?.PasswordSalt);
                var computedhash = encode.ComputeHash(
                    Encoding.UTF8.GetBytes(model.Password));

                if (!computedhash.SequenceEqual(user?.PasswordHash))
                    return BadRequest("invalid credentials!");

                var token = _authService.CreateToken(user);
                return Ok(new LoginResponse("success",user,token));
            }
            return BadRequest("user does not exist!");
        }
        return BadRequest();
       
    }


    [HttpPost]
    [Route("logIn/google")]

    public async Task<IActionResult> GoggleLogin(ExternalLoginDto resource)
    {
        var payload = await _authService.VerifyGoogleToken(resource.AccessToken);
        if (payload == null)
        {
         return BadRequest(new Models.ErrorModel("Invalid Google Token!", "GoogleSignIn"));
        }
        var user_exist = await _userService.GetUserByEmail(payload.Email);
        if (user_exist == null)
        {
            var new_user = new User()
            {
                FullName = payload.GivenName + " " + payload.FamilyName,
                Email = payload.Email,
            };
            var user = await _userRepository.AddUserAsync(new_user);
            return Ok(new RegistrationResponse("success", user));
        }
        return Ok(new RegistrationResponse("success", user_exist));


    }



    [HttpPost("password/forgot")]
    public  async Task<IActionResult> ForgotPassword(string email)
    {
        var user = await _userService.GetUserByEmail(email);

        if (user == null)
        {
          return NotFound("User not found");
        }
        var response = _authService.SendEmail(email, "e");
        return Ok(response);
       
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
//                return BadRequest("server error");

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
