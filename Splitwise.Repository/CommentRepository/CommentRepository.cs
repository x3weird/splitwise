using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Splitwise.DomainModel.Models;
using Splitwise.Repository.DataRepository;
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
        private readonly IDataRepository _dal;

        public CommentRepository(IMapper mapper, IDataRepository dal)
        {
            _mapper = mapper;
            _dal = dal;
        }
        public async Task AddComment(CommentData commentData,string currentUserId)
        {

            Comment comment = _mapper.Map<Comment>(commentData);
            comment.UserId = currentUserId;

            //await _db.Comments.AddAsync(comment);
            await _dal.AddAsync(comment);
        }

        public async Task DeleteComment(string commentId)
        {
            //Comment comment = await _db.Comments.Where(c => c.Id.Equals(commentId)).SingleAsync();
            Comment comment = await _dal.Where<Comment>(c => c.Id.Equals(commentId)).SingleAsync();
            //_db.Comments.Remove(comment);
            _dal.Remove(comment);
        }
    }
}
