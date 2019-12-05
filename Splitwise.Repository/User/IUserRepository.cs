using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Splitwise.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Splitwise.Repository.User
{
    public interface IUserRepository
    {
        Task<LoginReturnModel> Login(Login login);
        Task<IdentityResult> CreateUser(UserDetails userDetails);
        Task<UserDetails> GetUserDetails(string email);
        Task<ApplicationUser> GetCurrentUser(ClaimsPrincipal claimsPrincipal);
        Task Logout();
        Task EditUserDetails(UserDetails userDetails, string email);
    }
}
