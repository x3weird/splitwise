using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Splitwise.DomainModel.Models;
using Splitwise.Repository.UnitOfWork;
using System;
using System.Collections.Generic;
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
            await _unitOfWork.Comment.AddComment(commentData);
            return Ok();
        }

        [HttpDelete]
        public object DeleteComment(string commentId)
        {
            _unitOfWork.Comment.DeleteComment(commentId);
            return Ok();
        }
    }
}
