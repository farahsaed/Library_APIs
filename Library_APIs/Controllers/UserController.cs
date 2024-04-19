using Library_APIs.Data;
using Library_APIs.DTO;
using Library_APIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Library_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly LibraryContext _context;
        private readonly IConfiguration _configuration;
        public UserController(UserManager<ApplicationUser> userManager , LibraryContext context ,IConfiguration configuration)

        {
            this._userManager = userManager;
            this._context = context;
            this._configuration = configuration;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> register(UserDataDTO registerDTO)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser();
                user.Email = registerDTO.Email;
                user.FirstName = registerDTO.FirstName;
                user.LastName = registerDTO.LastName;
                user.UserName = registerDTO.UserName;

                IdentityResult result = await _userManager.CreateAsync(user, registerDTO.Password);
                if (result.Succeeded)
                {
                    return Ok("Account created successfully");
                }
                return BadRequest(result.Errors.FirstOrDefault());
            }
            return BadRequest(ModelState);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> login(LoginDTO userDTO)
        {
            if(ModelState.IsValid)
            {
                ApplicationUser? user = await _userManager.FindByNameAsync(userDTO.UserName);
                if(user != null)
                {
                   bool found = await _userManager.CheckPasswordAsync(user, userDTO.Password);
                    if (found)
                    {
                        var claims = new List<Claim>();
                        claims.Add(new Claim(ClaimTypes.Name, userDTO.UserName));
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                        claims.Add(new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()));

                        var roles = await _userManager.GetRolesAsync(user);
                        foreach (var item in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, item));
                        }

                        SecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
                        SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                        JwtSecurityToken token = new JwtSecurityToken(
                              issuer: _configuration["JWT:issuer"],
                              audience: _configuration["JWT:audience"],
                              claims: claims,
                              expires: DateTime.Now.AddHours(3),
                              signingCredentials:signingCredentials 
                            );
                        return Ok(new {
                            message = "Logged in successfully",
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expires = token.ValidTo
                        });
                    }
                }
                return Unauthorized("Unauthorized");
            }
            return Unauthorized("Unauthorized");
        }
            
    }
}
