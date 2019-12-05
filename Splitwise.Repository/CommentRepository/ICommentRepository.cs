using Splitwise.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Splitwise.Repository.CommentRepository
{
    public interface ICommentRepository
    {
        Task AddComment(CommentData commentData);
        Task DeleteComment(string commentId);
    }
}
