using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Splitwise.DomainModel.Models;
using Splitwise.Repository.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Splitwise.Core.ApiControllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<object> GetUserDetail()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var email = claimsIdentity.FindFirst(ClaimTypes.Email)?.Value;
            var user = await _unitOfWork.User.GetUserDetails(email);
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<object> CreateUser(UserDetails userDetails)
        {
            var result = await _unitOfWork.User.CreateUser(userDetails);
            if(result != null)
            {
                return Ok(result);
            } else
            {
                return BadRequest("User already exists");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<object> Login(Login login)
        {
            var loginReturnModel = await _unitOfWork.User.Login(login);
            if(loginReturnModel != null)
            {
                return Ok(loginReturnModel);
            } else {
                return BadRequest("InvalId Email and Password");
            }
            
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("logout")]
        public async Task<object> Logout()
        {
            await _unitOfWork.User.Logout();
            return Ok();
        }

        [HttpPost]
        public void EditUserDetails(UserDetails userDetails)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            _unitOfWork.User.EditUserDetails(userDetails, userId);
        }
    }
}