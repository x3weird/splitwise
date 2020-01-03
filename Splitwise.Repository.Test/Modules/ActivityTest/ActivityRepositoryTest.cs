using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using Splitwise.DomainModel.Models;
using Splitwise.Repository.ActivityRepository;
using Splitwise.Repository.DataRepository;
using Splitwise.Repository.GroupRepository;
using Splitwise.Repository.Test.Bootstrap;
using Splitwise.Repository.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace Splitwise.Repository.Test.Modules.ActivityTest
{

    [Collection("Register Dependency")]
    public class ActivityRepositoryTest
    {
        private Mock<IDataRepository> _dataRepositoryMock { get; }
        private Mock<UserManager<ApplicationUser>> _userManagerMock { get; }
        private IMapper _mapperMock { get; }
        private IActivityRepository _activityRepository { get; }

        public ActivityRepositoryTest(Initialize initialize)
        {
            _dataRepositoryMock = initialize.ServiceProvider.GetService<Mock<IDataRepository>>();
            _mapperMock = initialize.ServiceProvider.GetService<IMapper>();
            _userManagerMock = initialize.ServiceProvider.GetService<Mock<UserManager<ApplicationUser>>>();
            _activityRepository = initialize.ServiceProvider.GetService<IActivityRepository>();
        }

        [Fact]
        public void ActivityList()
        {
            //Arange
            string userId= "7800b494-9cf4-44ca-ab1a-cef1bcc056b4";
            List<Activity> list = new List<Activity>() {
                new Activity
                {
                    Id = "11acd072-5938-4ffd-8029-09df732689b7",
                    Date = new DateTime(2019, 18, 12, 12, 3, 53),
                    ActivityOn = "Expense",
                    ActivityOnId = "dc3e2e99-379b-41d6-b8a7-7caf0e3710a6",
                    Log = "arjun added check"
                },
                new Activity
                {
                    Id = "20546eec-0c22-4008-9636-c017c42b0b05",
                    Date = new DateTime(0000, 00, 00, 00, 0, 00),
                    ActivityOn = "Expense",
                    ActivityOnId = "722dd145-aae9-4e15-855d-5948d67b9b0b",
                    Log = "arjun Paid harshil"
                }
            };

            var expected = new List<ActivityDetails>()
            {
                new ActivityDetails
                {
                    Id = "",
                    ActivityOn = "",
                    ActivityOnId = "",
                    Log = "",
                    Log2 = "",
                    Date = new DateTime(0000, 00, 00, 00, 0, 00)
                },
                new ActivityDetails
                {
                    Id = "",
                    ActivityOn = "",
                    ActivityOnId = "",
                    Log = "",
                    Log2 = "",
                    Date = new DateTime(0000, 00, 00, 00, 0, 00)
                }
            };

            //Act
            _dataRepositoryMock.Setup(x => x.Get<Activity>()).Returns(Task.FromResult(list));
            _dataRepositoryMock.Setup(x=>x.Where(It.IsAny<Expression<Func<, bool>>>()))
            _activityRepository.ActivityList(userId);

            //Assert
            Assert.Equal(1,1);
        }
    }
}