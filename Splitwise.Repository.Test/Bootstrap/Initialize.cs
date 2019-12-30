using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Splitwise.DomainModel.Models;
using Splitwise.Repository.ActivityRepository;
using Splitwise.Repository.CommentRepository;
using Splitwise.Repository.DataRepository;
using Splitwise.Repository.ExpenseRepository;
using Splitwise.Repository.FriendRepository;
using Splitwise.Repository.GroupRepository;
using Splitwise.Repository.UnitOfWork;
using Splitwise.Repository.User;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Splitwise.Repository.Test.Bootstrap
{
    
    public class Initialize
    {
        public readonly IServiceProvider ServiceProvider;

        
        public Initialize()
        {
            var services = new ServiceCollection();

            services.AddScoped<IGroupRepository, GroupRepository.GroupRepository>();
            services.AddScoped<ICommentRepository, CommentRepository.CommentRepository>();
            services.AddScoped<IActivityRepository, ActivityRepository.ActivityRepository>();
            services.AddScoped<IExpenseRepository, ExpenseRepository.ExpenseRepository>();
            services.AddScoped<IFriendRepository, FriendRepository.FriendRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

            services.AddScoped<IDataRepository, DataRepository.DataRepository>();

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            services.AddScoped(x => userStoreMock);
            services.AddScoped(x => userStoreMock.Object);
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            services.AddScoped(x => userManagerMock);
            services.AddScoped(x => userManagerMock.Object);

            var dataRepositoryMock = new Mock<IDataRepository>();
            services.AddSingleton(x => dataRepositoryMock);
            services.AddSingleton(x => dataRepositoryMock.Object);

            
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            ServiceProvider = services.BuildServiceProvider();
        }
    }
}
