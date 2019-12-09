using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Splitwise.DomainModel.Models;
using Splitwise.Repository.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Splitwise.Core.ApiControllers
{
    [AllowAnonymous]
    [Route("api/comments")]
    [ApiController]
    public class CommentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<object> AddComment(CommentData commentData)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var currentUserId = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            await _unitOfWork.Comment.AddComment(commentData, currentUserId);
            await _unitOfWork.Commit();
            return Ok();
        }

        [HttpDelete]
        public async Task<object> DeleteComment(string commentId)
        {
            await _unitOfWork.Comment.DeleteComment(commentId);
            return Ok();
        }
    }
}
