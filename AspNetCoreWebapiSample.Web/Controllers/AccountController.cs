using AspNetCoreWebapiSample.Infrastructure.Identity;
using AspNetCoreWebapiSample.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreWebapiSample.Web.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, SignInManager<AppUser> signInManager, IConfiguration configuration)
        {
            this._userManager = userManager;
            this._configuration = configuration;
            this._roleManager = roleManager;
        }

        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterAccountModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var appUser = new AppUser
            {
                UserName = model.Name,
                Email = model.Email
            };


            if (!await _roleManager.RoleExistsAsync(model.Role))
                return BadRequest("Role doesn't exist");


            var result = await _userManager.CreateAsync(appUser, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(appUser, model.Role);
            }
            else
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return BadRequest(ModelState);
            }

            var token = await GenerateToken(appUser);
            return Ok(new { access_token = token });

        }

        [AllowAnonymous]
        [HttpPost("Authenticate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Authenticate([FromBody]AuthenticateModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest("Invalid email or password");

            var passwordIsValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordIsValid)
                return BadRequest("Invalid email or password");


            var token = await GenerateToken(user);
            return Ok(new { access_token = token });
        }



        private async Task<object> GenerateToken(AppUser appUser)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, appUser.UserName),
                new Claim(JwtRegisteredClaimNames.Email, appUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
            };

            var roles = await _userManager.GetRolesAsync(appUser);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfiguration:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToInt16(_configuration["JwtConfiguration:ExpireDays"]));

            var token = new JwtSecurityToken(
                    _configuration["JwtConfiguration:Issuer"],
                    _configuration["JwtConfiguration:Issuer"],
                    claims,
                    expires: expires,
                    signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);





        }



    }
}
