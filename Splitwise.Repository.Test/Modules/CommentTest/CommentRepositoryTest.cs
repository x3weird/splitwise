using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using Splitwise.DomainModel.Models;
using Splitwise.Repository.DataRepository;
using Splitwise.Repository.Test.Bootstrap;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Splitwise.Repository.CommentRepository;
using System.Linq.Expressions;
using System.Linq;
using MockQueryable.Moq;

namespace Splitwise.Repository.Test.Modules.CommentTest
{
    [Collection("Register Dependency")]
    public class CommentRepositoryTest
    {
        private Mock<IDataRepository> _dataRepositoryMock { get; }
        private ICommentRepository _commentRepository { get; }

        public CommentRepositoryTest(Initialize initialize)
        {
            _dataRepositoryMock = initialize.ServiceProvider.GetService<Mock<IDataRepository>>();
            
            _commentRepository = initialize.ServiceProvider.GetService<ICommentRepository>();
        }

        [Fact]
        public async Task AddComment()
        {
            //Arrange
            string currentUserId = "7800b494-9cf4-44ca-ab1a-cef1bcc056b4";

            CommentData commentData = new CommentData()
            {
                UserId = "7800b494-9cf4-44ca-ab1a-cef1bcc056b4",
                Content = "great",
                CreatedOn = new DateTime(2020,1,1,05,20,22),
                ExpenseId = "f2ddd4fd-bd9d-4bd1-8ed3-87ae699cee62"
            };
            //Act
            await _commentRepository.AddComment(commentData, currentUserId);
            //Assert
            _dataRepositoryMock.Verify(x=>x.AddAsync(It.IsAny<Comment>()), Times.Once);
        }

        [Fact]
        public async Task DeleteComment()
        {
            //Arrage

            string commentId = "17927372-477b-4c3d-bff5-fa4954595fd7";
            List<Comment> comments = new List<Comment>()
            {
                new Comment()
                {
                    Id = "17927372-477b-4c3d-bff5-fa4954595fd7",
                    CommentData = "great",
                    CreatedOn = new DateTime(2020,1,1,05,20,22),
                    ExpenseId = "f2ddd4fd-bd9d-4bd1-8ed3-87ae699cee62",
                    UserId = "7800b494-9cf4-44ca-ab1a-cef1bcc056b4"
                }
            };
            
            //Act

            _dataRepositoryMock.Setup(x=>x.Where(It.IsAny<Expression<Func<Comment, bool>>>())).Returns(comments.AsQueryable().BuildMock().Object);
            await _commentRepository.DeleteComment(commentId);
            
            //Assert
            _dataRepositoryMock.Verify(x=>x.Remove(It.IsAny<Comment>()));
        }
    }
}

