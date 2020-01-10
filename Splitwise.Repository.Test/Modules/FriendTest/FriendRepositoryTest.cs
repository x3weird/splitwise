using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using Splitwise.DomainModel.Models;
using Splitwise.Repository.DataRepository;
using Splitwise.Repository.FriendRepository;
using Splitwise.Repository.Test.Bootstrap;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Linq;
using MockQueryable.Moq;

namespace Splitwise.Repository.Test.Modules.FriendTest
{
    [Collection("Register Dependency")]
    public class FriendRepositoryTest
    {
        private Mock<IDataRepository> _dataRepositoryMock { get; }
        private Mock<UserManager<ApplicationUser>> _userManagerMock { get; }
        private IMapper _mapperMock { get; }
        private IFriendRepository _friendRepository { get; }

        public FriendRepositoryTest(Initialize initialize)
        {
            _dataRepositoryMock = initialize.ServiceProvider.GetService<Mock<IDataRepository>>();
            _mapperMock = initialize.ServiceProvider.GetService<IMapper>();
            _userManagerMock = initialize.ServiceProvider.GetService<Mock<UserManager<ApplicationUser>>>();
            _friendRepository = initialize.ServiceProvider.GetService<IFriendRepository>();
        }

        [Fact]
        public async Task UserExpense()
        {
            //Arrange
            string userId = "7800b494-9cf4-44ca-ab1a-cef1bcc056b4";
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

            List<ApplicationUser> users = new List<ApplicationUser>() {
                new ApplicationUser
                {
                    Email="arjun@gmail.com",
                    Id="7800b494-9cf4-44ca-ab1a-cef1bcc056b4"
                }
            };


            //Act

            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<Ledger, bool>>>())).Returns(ledgers.AsQueryable().BuildMock().Object);
            _dataRepositoryMock.Setup(x => x.Where(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(users.AsQueryable().BuildMock().Object);
            var actual = _friendRepository.UserExpense(userId);
            //Assert
            Assert.NotNull(actual);
        }
    }
}
