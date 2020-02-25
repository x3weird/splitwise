using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Splitwise.DomainModel.Models.ApplicationClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace Splitwise.DomainModel.Models
{
    public class SplitwiseDbContext : IdentityDbContext<ApplicationUser>
    {
        #region base methods

        public SplitwiseDbContext(DbContextOptions<SplitwiseDbContext> options)
               : base(options)
        {

        }

        #endregion

        #region Properties

        public DbSet<Activity> Activities { get; set; }
        public DbSet<ActivityUser> ActivityUsers { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Friend> Friends { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupExpense> GroupExpenses { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<Ledger> Ledgers { get; set; }
        public DbSet<NotificationHub> NotificationHubs { get; set; }
        public DbSet<ExpenseNotification> ExpenseNotifications { get; set; }

        #endregion
    }
}
