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

namespace Splitwise.Repository.Test.Modules.ExpenseRepositoryTest
{
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
        }

        public async Task GetExpenseList()
        {
            //Arrange

            //Act

            //Assert
        }
    }
}
