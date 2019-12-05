using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Splitwise.DomainModel.Models;
using Splitwise.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Splitwise.Repository.User
{
    public class UserRepository : IUserRepository
    {
        private readonly SplitwiseDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOptions<ApplicationSettings> _appSettings;

        public UserRepository(SplitwiseDbContext db,
                                UserManager<ApplicationUser> userManager,
                                RoleManager<IdentityRole> roleManager,
                                SignInManager<ApplicationUser> signInManager,
                                IOptions<ApplicationSettings> appSettings)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _appSettings = appSettings;
        }

        public async Task<UserDetails> GetUserDetails(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            UserDetails userDetails = new UserDetails()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Password = "",
                Email = user.Email,
                Currency = user.Currency,
                IsRegistered = user.IsRegistered,
                Number = user.PhoneNumber
            };
            return userDetails;
        }

        public async Task EditUserDetails(UserDetails userDetails, string id)
        {
            var user = _db.Users.Where(u => u.Id.Equals(id)).SingleOrDefault();

            user.FirstName = userDetails.FirstName;
            user.LastName = userDetails.LastName;
            user.Email = userDetails.Email;
            user.Currency = userDetails.Currency;
            user.PhoneNumber = userDetails.Number;

            await _db.SaveChangesAsync();
        }

        public async Task<LoginReturnModel> Login(Login login)
        {
            var LoggedInUser = await _userManager.FindByEmailAsync(login.Email);
            if (LoggedInUser != null)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    LoggedInUser.UserName, login.Password, login.RememberMe, false);

                if (result.Succeeded)
                {

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                         {
                                     new Claim(ClaimTypes.Email, LoggedInUser.Email.ToString()),
                                     new Claim(ClaimTypes.Name, LoggedInUser.Id.ToString())
                         }),
                        Expires = DateTime.UtcNow.AddDays(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Value.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                    var token = tokenHandler.WriteToken(securityToken);
                    LoginReturnModel loginReturnModel = new LoginReturnModel()
                    {
                        Token = token,
                        Email = LoggedInUser.Email
                    };
                    return loginReturnModel;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<IdentityResult> CreateUser(UserDetails userDetails)
        {
            var user = new ApplicationUser
            {
                UserName = userDetails.Email,
                Email = userDetails.Email,
                FirstName = userDetails.FirstName,
                LastName = userDetails.LastName,
                Currency = userDetails.Currency,
                PhoneNumber = userDetails.Number
            };

            try
            {
                var userExist = await _userManager.FindByEmailAsync(userDetails.Email);
                if (userExist == null)
                {
                    return await _userManager.CreateAsync(user, userDetails.Password);

                }
                else
                {
                    userExist.IsRegistered = true;
                    userExist.PhoneNumber = userDetails.Number;
                    userExist.FirstName = userDetails.FirstName;
                    userExist.LastName = userDetails.LastName;
                    return null;
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ApplicationUser> GetCurrentUser(ClaimsPrincipal claimsPrincipal)
        {
            return await _userManager.GetUserAsync(claimsPrincipal);
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
