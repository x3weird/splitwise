using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using Splitwise.DomainModel.Models;
using Splitwise.Repository.DataRepository;
using Splitwise.Repository.GroupRepository;
using Splitwise.Repository.Test.Bootstrap;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace Splitwise.Repository.Test.Modules.GroupTest
{
    [Collection("Register Dependency")]
    public class GroupRepository
    {
        private Mock<IDataRepository> _dataRepositoryMock { get; }
        private Mock<UserManager<ApplicationUser>> _userManagerMock { get; }
        private IMapper _mapperMock { get; }
        private IGroupRepository _groupRepository { get; }

        public GroupRepository(Initialize initialize)
        {
            _dataRepositoryMock = initialize.ServiceProvider.GetService<Mock<IDataRepository>>();
            _mapperMock = initialize.ServiceProvider.GetService<IMapper>();
            _userManagerMock = initialize.ServiceProvider.GetService<Mock<UserManager<ApplicationUser>>>();
            _groupRepository = initialize.ServiceProvider.GetService<IGroupRepository>();
        }

        [Fact]
        public async Task GetGroupListAsync()
        {
            List<Group> list = new List<Group>()
            {
                new Group()
                {
                    AddedBy = "7800b494-9cf4-44ca-ab1a-cef1bcc056b4",
                    Name = "lunch",
                    Id = "ba2c34a3-c0da-42c2-9e93-53d1bd307f45",
                    CreatedOn = new DateTime(2019, 6, 12, 15, 3, 27),
                    IsDeleted = false,
                    SimplifyDebts = false
                },
                new Group()
                {
                    AddedBy = null,
                    Name = "trip",
                    Id = "6de3c332-3315-4a95-bbbc-c824ebd07360",
                    CreatedOn = new DateTime(2019, 6, 12, 15, 19, 28),
                    IsDeleted = false,
                    SimplifyDebts = false
                }
            };

            List<UserNameWithId> userNameWithIds = new List<UserNameWithId>()
            {
                new UserNameWithId()
                {
                    Name = "lunch",
                    UserId = "ba2c34a3-c0da-42c2-9e93-53d1bd307f45"
                },
                new UserNameWithId()
                {
                    Name = "trip",
                    UserId = "6de3c332-3315-4a95-bbbc-c824ebd07360"
                }
            };


            _dataRepositoryMock.Setup(x => x.Get<Group>()).Returns(Task.FromResult(list));

            //_dataRepositoryMock.Setup(x => x.Where<Group>()).Returns();

            List<UserNameWithId> check = await _groupRepository.GetGroupList();

            check.TrimExcess();
            userNameWithIds.TrimExcess();
            Assert.NotNull(check);

            var y = 1;
            if (userNameWithIds.Equals(check))
            {
                y = 2;
            }
            else
            {
                y = 3;
            }

            Assert.Equal(userNameWithIds.Count, check.Count);
            //Assert.True(check.Equals(userNameWithIds));
        }

        [Fact]
        public async Task AddGroupMembers()
        {
            //Arrange
            
        }
    }
}
