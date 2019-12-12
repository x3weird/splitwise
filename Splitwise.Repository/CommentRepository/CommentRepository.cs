using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Splitwise.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splitwise.Repository.CommentRepository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly SplitwiseDbContext _db;
        private readonly IMapper _mapper;

        public CommentRepository(SplitwiseDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public async Task AddComment(CommentData commentData,string currentUserId)
        {

            Comment comment = _mapper.Map<Comment>(commentData);
            comment.UserId = currentUserId;

            await _db.Comments.AddAsync(comment);
        }

        public async Task DeleteComment(string commentId)
        {
            Comment comment = await _db.Comments.Where(c => c.Id.Equals(commentId)).FirstOrDefaultAsync();
            _db.Comments.Remove(comment);
        }
    }
}
