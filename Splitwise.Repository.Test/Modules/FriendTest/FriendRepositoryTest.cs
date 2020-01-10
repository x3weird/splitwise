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

        }
    }
}
