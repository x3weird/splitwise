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
        public async Task AddGroupMembersAsync()
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

            List<Friend> friend = new List<Friend>();

            Group group = new Group()
            {
                Id = "ba2c34a3-c0da-42c2-9e93-53d1bd307f45",
                AddedBy = "7800b494-9cf4-44ca-ab1a-cef1bcc056b4",
                Name = "trip",
                CreatedOn = new DateTime(2019, 12, 06, 15, 03, 53),
                SimplifyDebts = false,
                IsDeleted = false
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

            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
                .Returns(applicationUsers.AsQueryable().BuildMock().Object);
             

            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<Group, bool>>>())).Returns(groups.AsQueryable().BuildMock().Object);

            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<Friend, bool>>>()))
                .Returns(friends.AsQueryable().BuildMock().Object);

            var actual = await _groupRepository.AddGroupMembers(groupAdd, Email, group);
            //Assert

            _dataRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Friend>()), Times.Never);
            _dataRepositoryMock.Verify(x => x.AddAsync(It.IsAny<GroupMember>()), Times.Exactly(2));
            _dataRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Activity>()), Times.Exactly(2));
            Assert.Equal(1,actual);
        }

        [Fact]
        public async Task AddGroupMembers_Group_Not_Found()
        {
            //Arrange

            string Email = "arjun@gmail.com";

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

            Group group = new Group()
            {
                Id = "ba2c34a3-c0da-42c2-9e93-53d1bd307f45",
                AddedBy = "7800b494-9cf4-44ca-ab1a-cef1bcc056b4",
                Name = "trip",
                CreatedOn = new DateTime(2019, 12, 06, 15, 03, 53),
                SimplifyDebts = false,
                IsDeleted = false
            };

            List<ApplicationUser> applicationUsers = new List<ApplicationUser>() {
                new ApplicationUser
                {
                    Email="arjun@gmail.com",
                    Id="7800b494-9cf4-44ca-ab1a-cef1bcc056b4"
                }
            };

            List<Group> groups = new List<Group>();

            //Act

            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(applicationUsers.AsQueryable().BuildMock().Object);
            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<Group, bool>>>())).Returns(groups.AsQueryable().BuildMock().Object);
            var actual = await  _groupRepository.AddGroupMembers(groupAdd, Email, group);

            //Assert
            Assert.Equal(0,actual);

        }

        [Fact]
        public async Task GetGroupExpenseList_Group_Not_Exist()
        {
            //Arrange

            List<Ledger> ledgers = new List<Ledger>()
            {
                new Ledger()
                {
                    Id = "6cea59ce-2eff-45a9-bced-20a62401241a",
                    ExpenseId = "b36bc83e-50c0-41b9-a965-92820a00fca7",
                    UserId = "7800b494-9cf4-44ca-ab1a-cef1bcc056b4",
                    CreditedAmount = 500,
                    DebitedAmount = 250
                },
                new Ledger()
                {
                    Id = "722be205-bd1a-43b9-963e-e2a10e8cfbda",
                    ExpenseId = "b36bc83e-50c0-41b9-a965-92820a00fca7",
                    UserId = "82b21620-42b5-4529-8087-331b8b896172",
                    CreditedAmount = 0,
                    DebitedAmount = -250
                }
            };

            List<Expense>  expenses = new List<Expense>(){
                new Expense()
                {
                    Id = "b36bc83e-50c0-41b9-a965-92820a00fca7",
                    AddedBy="7800b494-9cf4-44ca-ab1a-cef1bcc056b4",
                    Amount= 500,
                    CreatedOn= new DateTime(2019, 12, 09, 00, 00, 00),
                    Description ="checking abhi",
                    ExpenseType="Equally",
                    Note="",
                    IsDeleted=false
                }
            };

            List<ApplicationUser> users = new List<ApplicationUser>() {
                new ApplicationUser
                {
                    Email="arjun@gmail.com",
                    Id="7800b494-9cf4-44ca-ab1a-cef1bcc056b4"
                },
                new ApplicationUser
                {
                    Email="abhi@gmail.com",
                    Id="82b21620-42b5-4529-8087-331b8b896172"
                }
            };

            List<Comment> comments = new List<Comment>();

            List<GroupExpense> groupExpenses = new List<GroupExpense>();

            string groupId = "";
            string email = "arjun@gmail.com";
            //Act
            _dataRepositoryMock.Setup(x=>x.Get<Ledger>()).Returns(Task.FromResult(ledgers));
            _dataRepositoryMock.Setup(x=>x.Get<Expense>()).Returns(Task.FromResult(expenses));
            _dataRepositoryMock.Setup(x => x.Get<ApplicationUser>()).Returns(Task.FromResult(users));
            _dataRepositoryMock.Setup(x=>x.Where(It.IsAny<Expression<Func<Ledger, bool>>>())).Returns(ledgers.AsQueryable().BuildMock().Object);
            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<GroupExpense, bool>>>()))
                .Returns(groupExpenses.AsQueryable().BuildMock().Object);

            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<Comment, bool>>>())).Returns(comments.AsQueryable().BuildMock().Object);

            var actual = await _groupRepository.GetGroupExpenseList(groupId, email);

            Assert.Empty(actual);
        }

        [Fact]
        public async Task GetGroupExpenseList()
        {
            //Arrange

            List<Ledger> ledgers = new List<Ledger>()
            {
                new Ledger()
                {
                    Id = "c9052f72-8cd1-4b19-91a9-a9e1fde3167f",
                    ExpenseId = "f2ddd4fd-bd9d-4bd1-8ed3-87ae699cee62",
                    UserId = "82b21620-42b5-4529-8087-331b8b896172",
                    CreditedAmount = 0,
                    DebitedAmount = -5
                },
                new Ledger()
                {
                    Id = "d4a0f163-db8d-4df0-899c-fe2aedd5c459",
                    ExpenseId = "f2ddd4fd-bd9d-4bd1-8ed3-87ae699cee62",
                    UserId = "7800b494-9cf4-44ca-ab1a-cef1bcc056b4",
                    CreditedAmount = 10,
                    DebitedAmount = 5
                }
            };

            List<Expense> expenses = new List<Expense>(){
                new Expense()
                {
                    Id = "f2ddd4fd-bd9d-4bd1-8ed3-87ae699cee62",
                    AddedBy="7800b494-9cf4-44ca-ab1a-cef1bcc056b4",
                    Amount= 10,
                    CreatedOn= new DateTime(2020, 01, 08, 00, 00, 00),
                    Description ="abc",
                    ExpenseType="Equally",
                    Note="",
                    IsDeleted=false
                }
            };

            List<ApplicationUser> users = new List<ApplicationUser>() {
                new ApplicationUser
                {
                    Email="arjun@gmail.com",
                    Id="7800b494-9cf4-44ca-ab1a-cef1bcc056b4"
                },
                new ApplicationUser
                {
                    Email="abhi@gmail.com",
                    Id="82b21620-42b5-4529-8087-331b8b896172"
                }
            };

            List<ApplicationUser> user = new List<ApplicationUser>() {
                new ApplicationUser
                {
                    Email="arjun@gmail.com",
                    Id="7800b494-9cf4-44ca-ab1a-cef1bcc056b4"
                }
            };

            List<Comment> comments = new List<Comment>();

            List<GroupExpense> groupExpenses = new List<GroupExpense>()
            {
                new GroupExpense
                {
                    Id = "58dfdc20-f609-4c36-b903-e35e4739a334",
                    ExpenseId = "bd01eff7-c654-4762-8351-a65f3c10346d",
                    GroupId = "f2ddd4fd-bd9d-4bd1-8ed3-87ae699cee62"
                }
            };

            string groupId = "bd01eff7-c654-4762-8351-a65f3c10346d";
            string email = "arjun@gmail.com";

            List<ExpenseLedger> expenseLedger = new List<ExpenseLedger>()
            {
                new ExpenseLedger()
                {
                    Name = null,
                    Owes = -5,
                    Paid = 0,
                    UserId = "82b21620-42b5-4529-8087-331b8b896172"
                }
            };

            List<ExpenseDetail> expected = new List<ExpenseDetail>()
            {
                new ExpenseDetail()
                {
                    AddedBy="",
                    Amount= 10,
                    CreatedOn= new DateTime(2020, 01, 08, 00, 00, 00),
                    Description="abc",
                    ExpenseId="f2ddd4fd-bd9d-4bd1-8ed3-87ae699cee62",
                    ExpenseType ="Equally",
                    GroupId="",
                    GroupName="",
                    Note="",
                    ExpenseLedgers= expenseLedger,
                    Comments= {}
                }
            };

            //Act
            _dataRepositoryMock.Setup(x => x.Get<Ledger>()).Returns(Task.FromResult(ledgers));
            _dataRepositoryMock.Setup(x => x.Get<Expense>()).Returns(Task.FromResult(expenses));
            _dataRepositoryMock.Setup(x => x.Get<ApplicationUser>()).Returns(Task.FromResult(users));
            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<Ledger, bool>>>())).Returns(ledgers.AsQueryable().BuildMock().Object);
            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<GroupExpense, bool>>>()))
                .Returns(groupExpenses.AsQueryable().BuildMock().Object);

            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(user.AsQueryable().BuildMock().Object);
            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<Comment, bool>>>())).Returns(comments.AsQueryable().BuildMock().Object);

            var actual = await _groupRepository.GetGroupExpenseList(groupId, email);

            //Assert
            Assert.NotEmpty(actual);
            Assert.Equal(actual.Count(), expected.Count());
        }

        [Fact]
        public async Task RemoveGroup()
        {
            //Arrange
            List<GroupMember> groupMembers = new List<GroupMember>()
            {
                new GroupMember()
                {
                    Id="9c189adb-c0fd-426b-969a-16569441b0f2",
                    GroupId = "bd01eff7-c654-4762-8351-a65f3c10346d",
                    UserId = "82b21620-42b5-4529-8087-331b8b896172"
                },
                new GroupMember()
                {
                    Id="7e193cke-v0bj-126b-909a-12985349b0v5",
                    GroupId = "bd01eff7-c654-4762-8351-a65f3c10346d",
                    UserId = "7800b494-9cf4-44ca-ab1a-cef1bcc056b4"
                }
            };

            List<GroupExpense> groupExpenses = new List<GroupExpense>()
            {
                new GroupExpense()
                {
                    Id = "58dfdc20-f609-4c36-b903-e35e4739a334",
                    ExpenseId = "f2ddd4fd-bd9d-4bd1-8ed3-87ae699cee62",
                    GroupId = "bd01eff7-c654-4762-8351-a65f3c10346d"
                }
            };

            List<Group> groups = new List<Group>()
            {
                new Group()
                {
                    Id = "bd01eff7-c654-4762-8351-a65f3c10346d",
                    AddedBy = "7800b494-9cf4-44ca-ab1a-cef1bcc056b4",
                    CreatedOn = new DateTime(2020,01,08,18,08,34),
                    Name = "check",
                    SimplifyDebts = false,
                    IsDeleted = false
                }
            };

            Group group = new Group()
            {
                Id = "bd01eff7-c654-4762-8351-a65f3c10346d",
                AddedBy = "7800b494-9cf4-44ca-ab1a-cef1bcc056b4",
                CreatedOn = new DateTime(2020, 01, 08, 18, 08, 34),
                Name = "check",
                SimplifyDebts = false,
                IsDeleted = false
            };

            string groupId = "bd01eff7-c654-4762-8351-a65f3c10346d";

            //Act
            _dataRepositoryMock.Setup(x=>x.Where(It.IsAny<Expression<Func<GroupMember ,bool>>>())).Returns(groupMembers.AsQueryable().BuildMock().Object);
            _dataRepositoryMock.Setup(x=>x.Where(It.IsAny<Expression<Func<GroupExpense ,bool>>>())).Returns(groupExpenses.AsQueryable().BuildMock().Object);
            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<Group, bool>>>())).Returns(groups.AsQueryable().BuildMock().Object);
            _dataRepositoryMock.Setup(x => x.Remove(group));
            await _groupRepository.RemoveGroup(groupId);

            //Assert
            _dataRepositoryMock.Verify(x => x.RemoveRange(It.IsAny<List<GroupMember>>()), Times.Once);
            _dataRepositoryMock.Verify(x => x.RemoveRange(It.IsAny<List<GroupExpense>>()), Times.Once);
            _dataRepositoryMock.Verify(x => x.Remove(It.IsAny<Group>()), Times.Once);
        }
    }
}