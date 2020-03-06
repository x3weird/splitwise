using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using Splitwise.DomainModel.Models;
using Splitwise.Repository.DataRepository;
using Splitwise.Repository.ExpenseRepository;
using Splitwise.Repository.Test.Bootstrap;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Linq;
using MockQueryable.Moq;
using Xunit;

namespace Splitwise.Repository.Test.Modules.ExpenseRepositoryTest
{
    [Collection("Register Dependency")]
    public class ExpenseRepositoryTest
    {
        private Mock<IDataRepository> _dataRepositoryMock { get; }
        private Mock<UserManager<ApplicationUser>> _userManagerMock { get; }
        private IMapper _mapperMock { get; }
        private IExpenseRepository _expenseRepository { get; }

        public ExpenseRepositoryTest(Initialize initialize)
        {
            _dataRepositoryMock = initialize.ServiceProvider.GetService<Mock<IDataRepository>>();
            _mapperMock = initialize.ServiceProvider.GetService<IMapper>();
            _userManagerMock = initialize.ServiceProvider.GetService<Mock<UserManager<ApplicationUser>>>();
            _expenseRepository = initialize.ServiceProvider.GetService<IExpenseRepository>();
            _dataRepositoryMock.Reset();
        }

        [Fact]
        public async Task AddSettleUpExpense()
        {
            //Arrange

            string email = "arjun@gmail.com";
            List<ApplicationUser> user = new List<ApplicationUser>() {
                new ApplicationUser
                {
                    Email="arjun@gmail.com",
                    Id="7800b494-9cf4-44ca-ab1a-cef1bcc056b4",
                    FirstName="Arjun"
                }
            };

            SettleUp settleUp = new SettleUp()
            {
                Amount = 100,
                Date = new DateTime(2020, 1, 1, 01, 01, 01),
                Group = null,
                Note = null,
                Payer = "7800b494-9cf4-44ca-ab1a-cef1bcc056b4",
                Recipient = "82b21620-42b5-4529-8087-331b8b896172"
            };

            

            //Act
            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(user.AsQueryable().BuildMock().Object);
            await _expenseRepository.AddSettleUpExpense(settleUp, email);
            
            //Asert
            _dataRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Expense>()), Times.Once);
        }

        [Fact]
        public async Task GetExpenseList()
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

            List<Expense> expenses = new List<Expense>(){
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

            List<ApplicationUser> addedUser = new List<ApplicationUser>() {
                new ApplicationUser
                {
                    Email="arjun@gmail.com",
                    Id="7800b494-9cf4-44ca-ab1a-cef1bcc056b4"
                }
            };

            List<Comment> comments = new List<Comment>();

            List<ApplicationUser> commentUser = new List<ApplicationUser>();

            string email = "arjun@gmail.com";

            //Act
            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<Ledger, bool>>>())).Returns(ledgers.AsQueryable().BuildMock().Object);
            _dataRepositoryMock.Setup(x => x.Get<Expense>()).Returns(Task.FromResult(expenses));
            _dataRepositoryMock.Setup(x => x.Get<Ledger>()).Returns(Task.FromResult(ledgers));
            _dataRepositoryMock.Setup(x => x.Get<ApplicationUser>()).Returns(Task.FromResult(users));
            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<Comment, bool>>>())).Returns(comments.AsQueryable().BuildMock().Object);
            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
                //.Returns(commentUser.AsQueryable().BuildMock().Object)
                .Returns(addedUser.AsQueryable().BuildMock().Object);

            var actual = await _expenseRepository.GetExpenseList(email);

            //Assert

            Assert.NotEmpty(actual);

        }

        [Fact]
        public async Task GetExpenseList_Is_Deleted()
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

            List<Expense> expenses = new List<Expense>()
            {
                new Expense()
                {
                    Id = "b36bc83e-50c0-41b9-a965-92820a00fca7",
                    AddedBy="7800b494-9cf4-44ca-ab1a-cef1bcc056b4",
                    Amount= 500,
                    CreatedOn= new DateTime(2019, 12, 09, 00, 00, 00),
                    Description ="checking abhi",
                    ExpenseType="Equally",
                    Note="",
                    IsDeleted=true
                }
            };

            List<ApplicationUser> users = new List<ApplicationUser>()
            {
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

            List<ApplicationUser> addedUser = new List<ApplicationUser>()
            {
                new ApplicationUser
                {
                    Email="arjun@gmail.com",
                    Id="7800b494-9cf4-44ca-ab1a-cef1bcc056b4"
                }
            };

            List<Comment> comments = new List<Comment>();

            List<ApplicationUser> commentUser = new List<ApplicationUser>();

            string email = "arjun@gmail.com";

            //Act
            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<Ledger, bool>>>())).Returns(ledgers.AsQueryable().BuildMock().Object);
            _dataRepositoryMock.Setup(x => x.Get<Expense>()).Returns(Task.FromResult(expenses));
            _dataRepositoryMock.Setup(x => x.Get<Ledger>()).Returns(Task.FromResult(ledgers));
            _dataRepositoryMock.Setup(x => x.Get<ApplicationUser>()).Returns(Task.FromResult(users));

            var actual = await _expenseRepository.GetExpenseList(email);

            //Assert
            Assert.Empty(actual);
        }

        [Fact]
        public async Task DeleteExpense()
        {
            //Arrange
            List<Expense> expenses = new List<Expense>()
            {
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

            List<GroupExpense> groupExpenses = new List<GroupExpense>()
            {
                new GroupExpense
                {
                    Id = "58dfdc20-f609-4c36-b903-e35e4739a334",
                    ExpenseId = "bd01eff7-c654-4762-8351-a65f3c10346d",
                    GroupId = "f2ddd4fd-bd9d-4bd1-8ed3-87ae699cee62"
                }
            };

            List<ApplicationUser> user = new List<ApplicationUser>()
            {
                new ApplicationUser
                {
                    Email="arjun@gmail.com",
                    Id="7800b494-9cf4-44ca-ab1a-cef1bcc056b4",
                    FirstName="Arjun"
                }
            };

            string expenseId = "f2ddd4fd-bd9d-4bd1-8ed3-87ae699cee62";
            string currentUserId = "7800b494-9cf4-44ca-ab1a-cef1bcc056b4";
            //int expected = 1;

            //Act
            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<Expense, bool>>>())).Returns(expenses.AsQueryable().BuildMock().Object);
            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<GroupExpense, bool>>>())).Returns(groupExpenses.AsQueryable().BuildMock().Object);
            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(user.AsQueryable().BuildMock().Object);
            var actual = await _expenseRepository.DeleteExpense(expenseId, currentUserId);
            
            //Assert
            _dataRepositoryMock.Verify(x=>x.AddAsync(It.IsAny<Activity>()), Times.Once);
            //Assert.Equal(actual, expected);
        }

        [Fact]
        public async Task DeleteExpense_Already_Deleted()
        {
            //Arrange
            List<Expense> expenses = new List<Expense>();

            List<GroupExpense> groupExpenses = new List<GroupExpense>()
            {
                new GroupExpense
                {
                    Id = "58dfdc20-f609-4c36-b903-e35e4739a334",
                    ExpenseId = "bd01eff7-c654-4762-8351-a65f3c10346d",
                    GroupId = "f2ddd4fd-bd9d-4bd1-8ed3-87ae699cee62"
                }
            };

            List<ApplicationUser> user = new List<ApplicationUser>() {
                new ApplicationUser
                {
                    Email="arjun@gmail.com",
                    Id="7800b494-9cf4-44ca-ab1a-cef1bcc056b4",
                    FirstName="Arjun"
                }
            };

            string expenseId = "f2ddd4fd-bd9d-4bd1-8ed3-87ae699cee62";
            string currentUserId = "7800b494-9cf4-44ca-ab1a-cef1bcc056b4";
            //int expected = 0;

            //Act

            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<Expense, bool>>>())).Returns(expenses.AsQueryable().BuildMock().Object);
            var actual = await _expenseRepository.DeleteExpense(expenseId, currentUserId);

            //Assert

            Assert.NotNull(actual);
        }

        [Fact]
        public async Task AddExpenseInLedger()
        {
            
            //Arrange

            List<ApplicationUser> applicationUser1 = new List<ApplicationUser>()
            {
                new ApplicationUser()
                {
                    Id  = "7800b494-9cf4-44ca-ab1a-cef1bcc056b4",
                    FirstName = "Arjun",
                    Email = "arjun@gmail.com"
                },
                
            };

            List<ApplicationUser> applicationUser2 = new List<ApplicationUser>()
            {
                new ApplicationUser()
                {
                    Id  = "1254494-9cf4-44ca-ab3a-cef1sdf056b4",
                    FirstName = "Abhi",
                    Email = "abhi@gmail.com"
                },
            };

            List<UserExpenseDetail> ledgers = new List<UserExpenseDetail>()
            {
                new UserExpenseDetail()
                {
                    Amount = 10,
                    Email = "arjun@gmail.com"
                },
                new UserExpenseDetail()
                {
                    Amount = 10,
                    Email = "abhi@gmail.com"
                }
            };

            List<UserExpenseDetail> paidBy = new List<UserExpenseDetail>()
            {
                new UserExpenseDetail()
                {
                    Amount = 20,
                    Email = "arjun@gmail.com"
                }
            };

            List<string> emails = new List<string>
            {
                "arjun@gmail.com",
                "abhi@gmail.com"
            };

            AddExpense addExpense = new AddExpense()
            {
                Id = "12546645-d5df-ff21-532s-12sdfs12fs3",
                Amount = 20,
                AddedBy = "7800b494-9cf4-44ca-ab1a-cef1bcc056b4",
                Ledger = ledgers,
                PaidBy  = paidBy,
                CreatedOn = new DateTime(2020,12,1,15,01,01),
                Description = "check",
                EmailList = emails,
                ExpenseType = "Equally",
                 GroupId = null,
                 Note = null
            };

            Expense expense = new Expense()
            {
                Id = "f2ddd4fd-bd9d-4bd1-8ed3-87ae699cee62",
                AddedBy = "7800b494-9cf4-44ca-ab1a-cef1bcc056b4",
                Amount = 20,
                CreatedOn = new DateTime(2020, 12, 1, 15, 01, 01),
                Description = "check",
                ExpenseType = "Equally",
                Note = "",
                IsDeleted = false
            };

            Activity activity = new Activity()
            {
                Id = "sfsdfs-sfsd-1231s-1ds2fsdfs13",
                ActivityOn = "Expense",
                ActivityOnId = "f2ddd4fd-bd9d-4bd1-8ed3-87ae699cee62",
                Date = new DateTime(2020, 1, 1, 15, 5, 05),
                Log = "arjun added check"
            };

            //Act
            _dataRepositoryMock.SetupSequence(x => x.Where(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
                .Returns(applicationUser1.AsQueryable().BuildMock().Object)
                .Returns(applicationUser1.AsQueryable().BuildMock().Object)
                .Returns(applicationUser1.AsQueryable().BuildMock().Object)
                .Returns(applicationUser2.AsQueryable().BuildMock().Object)
                .Returns(applicationUser2.AsQueryable().BuildMock().Object)
                .Returns(applicationUser2.AsQueryable().BuildMock().Object);

            await _expenseRepository.AddExpenseInLedger(addExpense,expense,activity);

            //Assert
            _dataRepositoryMock.Verify(x=>x.AddRangeAsync(It.IsAny<List<ActivityUser>>()), Times.Once);
            _dataRepositoryMock.Verify(x => x.AddRangeAsync(It.IsAny<List<Ledger>>()), Times.Once);
        }

        [Fact]
        public async Task AddExpense()
        {
            //Arrange
            List<ApplicationUser> user = new List<ApplicationUser>() {
                new ApplicationUser
                {
                    Email="arjun@gmail.com",
                    Id="7800b494-9cf4-44ca-ab1a-cef1bcc056b4",
                    FirstName="Arjun"
                }
            };

            List<UserExpenseDetail> ledgers = new List<UserExpenseDetail>()
            {
                new UserExpenseDetail()
                {
                    Amount = 10,
                    Email = "arjun@gmail.com"
                },
                new UserExpenseDetail()
                {
                    Amount = 10,
                    Email = "abhi@gmail.com"
                }
            };

            List<UserExpenseDetail> paidBy = new List<UserExpenseDetail>()
            {
                new UserExpenseDetail()
                {
                    Amount = 20,
                    Email = "arjun@gmail.com"
                }
            };
            List<string> emails = new List<string>();
            emails.Add("arjun@gmail.com");
            emails.Add("abhi@gmail.com");

            AddExpense addExpense = new AddExpense()
            {
                Id = "12546645-d5df-ff21-532s-12sdfs12fs3",
                Amount = 20,
                AddedBy = "7800b494-9cf4-44ca-ab1a-cef1bcc056b4",
                Ledger = ledgers,
                PaidBy = paidBy,
                CreatedOn = new DateTime(2020, 12, 1, 15, 01, 01),
                Description = "check",
                EmailList = emails,
                ExpenseType = "Equally",
                GroupId = null,
                Note = null
            };

            //Act
            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(user.AsQueryable().BuildMock().Object);
            var actual = await _expenseRepository.AddExpense(addExpense);
            //Assert
            _dataRepositoryMock.Verify(x=>x.AddAsync(It.IsAny<Expense>()), Times.Once);
            Assert.NotNull(actual);
        }
    }
}
