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
using System.Linq.Expressions;
using System.Linq;
using MockQueryable.Moq;

namespace Splitwise.Repository.Test.Modules.GroupTest
{
    [Collection("Register Dependency")]
    public class GroupRepositoryTest
    {
        private Mock<IDataRepository> _dataRepositoryMock { get; }
        private Mock<UserManager<ApplicationUser>> _userManagerMock { get; }
        private IMapper _mapperMock { get; }
        private IGroupRepository _groupRepository { get; }

        public GroupRepositoryTest(Initialize initialize)
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
            Assert.Equal(userNameWithIds.Count, check.Count);
            //Assert.True(check.Equals(userNameWithIds));
        }

        [Fact]
        public async Task AddGroup_Group_Already_Exists()
        {
            //Arrange
            string Email = "arjun@gmail.com";
            List<Group> groups = new List<Group>()
            {
                new Group()
                {
                    Id= null,
                    AddedBy = "7800b494-9cf4-44ca-ab1a-cef1bcc056b4",
                    Name = "lunch",
                    CreatedOn = new DateTime(2019, 12, 06, 15, 03, 53),
                    SimplifyDebts = false,
                    IsDeleted = false
                }
            };

            GroupAdd groupAdd = new GroupAdd()
            {
                Name = "lunch"
            };

            List<ApplicationUser> applicationUsers = new List<ApplicationUser>() {
                new ApplicationUser
                {
                    Email="arjun@gmail.com",
                    Id="7800b494-9cf4-44ca-ab1a-cef1bcc056b4"
                }
            };
            
            //Act
            _dataRepositoryMock.Setup(x=>x.Where(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(applicationUsers.AsQueryable().BuildMock().Object);

            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<Group, bool>>>())).Returns(groups.AsQueryable().BuildMock().Object);

            var actual = await _groupRepository.AddGroup(groupAdd, Email);
            
            //Assert

            Assert.Null(actual);
        }

        [Fact]
        public async Task AddGroup()
        {
            //Arrange
            string Email = "arjun@gmail.com";

            List<Group> group = new List<Group>();

            GroupAdd groupAdd = new GroupAdd()
            {
                Name = "trip",
                AddedBy = "7800b494-9cf4-44ca-ab1a-cef1bcc056b4",
                CreatedOn = new DateTime(2019, 12, 06, 15, 03, 53),
                SimplifyDebts = false,
                Users = new List<GroupUsers>()
                {
                    new GroupUsers()
                    {
                        Email = "arjun@gmail.com",
                        Name = "arjun"
                    },
                    new GroupUsers()
                    {
                        Email = "abhi@gmail.com",
                        Name = "abhi"
                    }
                }
            };

            List<ApplicationUser> applicationUsers = new List<ApplicationUser>() {
                new ApplicationUser
                {
                    Email="arjun@gmail.com",
                    Id="7800b494-9cf4-44ca-ab1a-cef1bcc056b4"
                }
            };

            //Act
            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(applicationUsers.AsQueryable().BuildMock().Object);

            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<Group, bool>>>())).Returns(group.AsQueryable().BuildMock().Object);

            var actual = await _groupRepository.AddGroup(groupAdd, Email);

            //Assert
            _dataRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Group>()), Times.Once);

            _dataRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Activity>()), Times.Exactly(2));
        }

        [Fact]
        public void AddGroupMembers()
        {
            //Arrange
            string Email = "arjun@gmail.com";

            List<ApplicationUser> applicationUsers = new List<ApplicationUser>() {
                new ApplicationUser
                {
                    Email="arjun@gmail.com",
                    Id="7800b494-9cf4-44ca-ab1a-cef1bcc056b4"
                }
            };

            GroupAdd groupAdd = new GroupAdd()
            {
                Name = "trip",
                AddedBy = "7800b494-9cf4-44ca-ab1a-cef1bcc056b4",
                CreatedOn = new DateTime(2019, 12, 06, 15, 03, 53),
                SimplifyDebts = false,
                Users = new List<GroupUsers>()
                {
                    new GroupUsers()
                    {
                        Email = "arjun@gmail.com",
                        Name = "arjun"
                    },
                    new GroupUsers()
                    {
                        Email = "abhi@gmail.com",
                        Name = "abhi"
                    }
                }
            };

            List<Friend> friends = new List<Friend>() {
                new Friend()
                {
                    FriendId = "82b21620-42b5-4529-8087-331b8b896172",
                    UserId = "7800b494-9cf4-44ca-ab1a-cef1bcc056b4",
                    Id = "deed3c1c-106c-4858-9864-f495dbdb2292"
                }
            };

            Group group = new Group()
            {
                Id = "ba2c34a3-c0da-42c2-9e93-53d1bd307f45",
                AddedBy = "7800b494-9cf4-44ca-ab1a-cef1bcc056b4",
                Name = "trip",
                CreatedOn = new DateTime(2019, 12, 06, 15, 03, 53),
                SimplifyDebts = false,
                IsDeleted = false
            };

            //List<Group> groups = new List<Group>()
            //{
            //    new Group()
            //    {
            //        Id = "ba2c34a3-c0da-42c2-9e93-53d1bd307f45",
            //        AddedBy = "7800b494-9cf4-44ca-ab1a-cef1bcc056b4",
            //        Name = "trip",
            //        CreatedOn = new DateTime(2019, 12, 06, 15, 03, 53),
            //        SimplifyDebts = false,
            //        IsDeleted = false
            //    }
            //};
            List<Group> groups = new List<Group>();

            //Act

            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
                .Returns(applicationUsers.AsQueryable().BuildMock().Object);
             

            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<Group, bool>>>())).Returns(groups.AsQueryable().BuildMock().Object);

            _dataRepositoryMock.SetupSequence(x => x.Where(It.IsAny<Expression<Func<Friend, bool>>>()))
                .Returns(friends.AsQueryable().BuildMock().Object);

            _groupRepository.AddGroupMembers(groupAdd, Email, group);
            //Assert

            _dataRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Friend>()), Times.Never);
            _dataRepositoryMock.Verify(x => x.AddAsync(It.IsAny<GroupMember>()), Times.Once);
            _dataRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Activity>()), Times.Once);
        }

        [Fact]
        public void AddGroupMembers_Group_Not_Found()
        {
            //Arrange

            List<ApplicationUser> applicationUsers = new List<ApplicationUser>() {
                new ApplicationUser
                {
                    Email="arjun@gmail.com",
                    Id="7800b494-9cf4-44ca-ab1a-cef1bcc056b4"
                }
            };

            List<Group> groups = new List<Group>()
            {
                new Group()
                {
                    Id = "ba2c34a3-c0da-42c2-9e93-53d1bd307f45",
                    AddedBy = "7800b494-9cf4-44ca-ab1a-cef1bcc056b4",
                    Name = "trip",
                    CreatedOn = new DateTime(2019, 12, 06, 15, 03, 53),
                    SimplifyDebts = false,
                    IsDeleted = false
                }
            };

            //Act

            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(applicationUsers.AsQueryable().BuildMock().Object);
            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<Group, bool>>>())).Returns(groups.AsQueryable().BuildMock().Object);

            //Assert

            
        }
    }
}
