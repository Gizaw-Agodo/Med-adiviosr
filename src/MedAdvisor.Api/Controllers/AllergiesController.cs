using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MedAdvisor.Api.Dtos;
using MedAdvisor.DataAccess.MySql.DataContext;
using MedAdvisor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace MedAdvisor.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllergiesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _db;
        public AllergiesController(IConfiguration config, AppDbContext db)
        {
            _configuration = config;
            _db = db;
        }


        [HttpPost]
        [Route("add/{id}")]
        public async Task<IActionResult> AddAllergy([FromRoute] Guid id)
        {
         Request.Headers.TryGetValue("Authorization", out StringValues token);
            if (String.IsNullOrEmpty(token))
            {
                return BadRequest("un authorized user");
            }

            var Id =  GetId(token);
            var User_id = new Guid(Id);

            //var Employee = await _db.Users.Include(m => m.Profile)
            //    .ThenInclude(m=>m.Allergies)
            //    .ThenInclude(u=>u.Allergy)
            //    .FirstOrDefaultAsync(m => m.UserId == userId);
            //return Ok(Employee);

            //var alergy = new Allergy
            //{
            //    Name = "the man",
            //    Code = "12345",
            //};
            //await _db.Allergies.AddAsync(alergy);
            //await _db.SaveChangesAsync();
            //return Ok(alergy);
            //var allergy = await _db.Allergies.FindAsync(request.AllergyId);

            var allergy = await _db.Allergies.Include(a => a.Users)
            .FirstOrDefaultAsync(a => a.Id == id);

            var user = await _db.Users.Include(u => u.Allergies)
                .FirstOrDefaultAsync(u => u.Id == User_id);

            user?.Allergies?.Add(allergy); 
            allergy?.Users?.Add(user);       
            await _db.SaveChangesAsync();
            return Ok(user);

        }


        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteAllergy([FromRoute] Guid id)
        {
            Request.Headers.TryGetValue("Authorization", out StringValues token);
            if (String.IsNullOrEmpty(token))
            {
                return BadRequest("un authorized user");
            }

            var Id = GetId(token);
            var User_id = new Guid(Id);

            var allergy = await _db.Allergies.Include(a => a.Users)
             .FirstOrDefaultAsync(a => a.Id == id);

            var user = await _db.Users.Include(u => u.Allergies)
                .FirstOrDefaultAsync(u => u.Id == User_id);

            user.Allergies.Remove(allergy);
            await _db.SaveChangesAsync();

            //var user = await _db.Users.Include(m => m.Profile).ThenInclude(u => u.Allergies)
            // .AsNoTracking().FirstOrDefaultAsync(m => m.UserId == request.UserId);
            return Ok(user);


            //var allergy = new allergy()
            //{
            //    allergyid = guid.newguid(),
            //    name = newallergy.name,
            //    code = newallergy.code,
            //};
            //await _db..addasync(contact);
            //await _db.savechangesasync();

            //return Ok();
        }

        [HttpGet]
        [Route("search")]

        public async Task<IActionResult> search(string name)
        {

            var  allergies =   await _db.Allergies.Where(a => a.Name.Contains(name)).ToListAsync();
            return Ok( allergies);
        }

        private string GetId(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration.GetSection("JWT:Secret").Value);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var id = jwtToken.Claims.First(x => x.Type == "Id").Value;
            return id;
        }

    }
}
