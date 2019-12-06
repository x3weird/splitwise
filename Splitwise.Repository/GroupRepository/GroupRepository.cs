using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Splitwise.DomainModel.Models;

namespace Splitwise.Repository.GroupRepository
{
    public class GroupRepository : IGroupRepository
    {
        private readonly SplitwiseDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public GroupRepository(SplitwiseDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<List<UserNameWithId>> GetGroupList()
        {
            List<UserNameWithId> query = await _db.Groups.Select(g => new UserNameWithId
            {
                UserId = g.Id,
                Name = g.Name
            }).ToListAsync();

            return query;
        }

        public async Task<int> AddGroup(GroupAdd groupAdd, string email)
        {
            var currentUserId = await _db.Users.Where(u => u.Email.Equals(email.ToLower())).Select(s=>s.Id).FirstOrDefaultAsync();
            var query = _db.Groups.Where(g => g.Name.Equals(groupAdd.Name)).SingleOrDefault();
            if (query == null)
            {
                Group group = new Group
                {
                    Name = groupAdd.Name,
                    AddedBy = currentUserId,
                    CreatedOn = DateTime.Now,
                    SimplifyDebts = groupAdd.SimplifyDebts,
                    IsDeleted = false
                };

                _db.Groups.Add(group);
                await _db.SaveChangesAsync();
                foreach (var x in groupAdd.Users)
                {
                    var User = await _db.Users.Where(u=>u.Email.Equals(x.Email.ToLower())).FirstOrDefaultAsync();
                    if (User == null)
                    {
                        var user = new ApplicationUser
                        {
                            UserName = x.Email,
                            Email = x.Email,
                            FirstName = x.Name,
                            LastName = x.Email,
                            Currency = "INR",
                            PhoneNumber = "1111111111",
                            IsRegistered = false
                        };
                        var addedUser = _userManager.CreateAsync(user, "Random@123");

                        if (addedUser.Result.Succeeded)
                        {
                            await _db.SaveChangesAsync();
                        }


                        Friend friend = new Friend()
                        {
                            FriendId = user.Id,
                            UserId = currentUserId
                        };

                        _db.Friends.Add(friend);

                        GroupMember groupMember = new GroupMember
                        {
                            GroupId = group.Id,
                            UserId = user.Id
                        };
                        _db.GroupMembers.Add(groupMember);

                    }
                    else
                    {
                        var checkFriend = _db.Friends.Where(f => (f.FriendId.Equals(User.Id) && f.UserId.Equals(currentUserId)) || (f.FriendId.Equals(currentUserId) && f.UserId.Equals(User.Id))).FirstOrDefault();

                        if (checkFriend == null)
                        {
                            Friend friend = new Friend
                            {
                                FriendId = User.Id,
                                UserId = currentUserId
                            };
                            _db.Add(friend);
                        }

                        GroupMember groupMember = new GroupMember
                        {
                            GroupId = group.Id,
                            UserId = User.Id
                        };
                        _db.GroupMembers.Add(groupMember);

                        
                    }

                    Activity activity = new Activity()
                    {
                        Log = await _db.Users.Where(u => u.Email.Equals(email.ToLower())).Select(s => s.FirstName).FirstOrDefaultAsync() + " created the group " + groupAdd.Name,
                        ActivityOn = "Group",
                        ActivityOnId = group.Id
                    };

                    _db.Activities.Add(activity);

                    await _db.SaveChangesAsync();
                }



                return 1;
            }
            else
            {
                return 0;
            }
        }

        public async Task<int> EditGroup(string groupId, GroupAdd groupAdd)
        {
            Group gp = _db.Groups.Where(g => g.Id.Equals(groupId) && g.IsDeleted.Equals(false)).SingleOrDefault();

            if ((gp != null) && (groupAdd.Users != null))
            {
                gp.Name = groupAdd.Name;
                gp.AddedBy = groupAdd.AddedBy;
                gp.CreatedOn = groupAdd.CreatedOn;
                gp.IsDeleted = false;
                gp.SimplifyDebts = groupAdd.SimplifyDebts;

                _db.GroupMembers.RemoveRange(_db.GroupMembers.Where(g => g.GroupId.Equals(groupId)));
                foreach (var userId in groupAdd.Users)
                {

                    GroupMember groupMember = new GroupMember
                    {
                        GroupId = groupId,
                        //UserId = 
                    };
                    _db.GroupMembers.Add(groupMember);

                }
                await _db.SaveChangesAsync();
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public async Task<GroupDetails> GetGroupDetails(string groupId)
        {
            Group gp = _db.Groups.Where(g => g.Id.Equals(groupId) && g.IsDeleted.Equals(false)).SingleOrDefault();

            if (gp != null)
            {
                var query = await _db.GroupMembers.Join(_db.Users,
                                                    g => g.UserId,
                                                    u => u.Id,
                                                    (g, u) => new
                                                    {
                                                        Id = g.UserId,
                                                        g.GroupId,
                                                        name = u.FirstName + " " + u.LastName,
                                                        email = u.Email
                                                    })
                                                    .Where(g => g.GroupId.Equals(groupId))
                                                    .Select(
                                                        g => new
                                                        {
                                                            g.Id,
                                                            g.name,
                                                            g.email
                                                        }).ToListAsync();

                List<GroupUsers> groupUsersList = new List<GroupUsers>();

                foreach (var q in query)
                {
                    GroupUsers groupUsers = new GroupUsers
                    {
                        //Id = q.Id,
                        Name = q.name,
                        Email = q.email
                    };
                    groupUsersList.Add(groupUsers);
                }

                GroupDetails groupDetails = new GroupDetails
                {
                    GroupId = groupId,
                    GroupName = gp.Name,
                    AddedBy = gp.AddedBy,
                    CreatedOn = gp.CreatedOn,
                    SimplifyDebts = gp.SimplifyDebts,
                    Users = groupUsersList
                };

                return groupDetails;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<UserExpense>> GroupUserExpense(string groupId, List<string> users)
        {
            //var query = _db.Expenses.Where(e => e.GroupId.Equals(groupId)); //changes
            List<string> ledgerIdList = new List<string>();
            List<UserExpense> userExpenses = new List<UserExpense>();
            List<Ledger> ledgers = new List<Ledger>();
            //foreach (var x in query)  //changes
            //{
            //    ledgers = _db.Ledgers.Where(l => l.ExpenseId.Equals(x.Id)).ToList();
            //}

            foreach (var userId in users)
            {
                var userLedgers = ledgers.Where(l => l.UserId.Equals(userId));
                float sum = 0;
                foreach (var x in userLedgers)
                {
                    sum = +x.CreditedAmount;
                    sum = -x.DebitedAmount;
                }
                UserExpense userExpense = new UserExpense()
                {
                    Name = _db.Users.Where(u => u.Id.Equals(userId)).Select(s => s.FirstName).Single(),
                    Amount = sum,
                    Id = userId
                };
                userExpenses.Add(userExpense);
                await _db.SaveChangesAsync();
            }

            return userExpenses;
        }

        public async Task<List<ExpenseDetail>> GetGroupExpenseList(string groupId, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            List<string> expenseIdList = new List<string>();
            var expenseList = _db.Ledgers.Where(l => l.UserId.Equals(user.Id)).Select(l => l.ExpenseId).Distinct();
            foreach (var expenseId in expenseList)
            {
                var check = _db.GroupExpenses.Where(g => g.ExpenseId.Equals(expenseId) && g.GroupId.Equals(groupId)).FirstOrDefault();
                if (check != null)
                {
                    expenseIdList.Add(expenseId);
                }
            }
            var expenses = _db.Expenses.Join(expenseIdList, e => e.Id, x => x, (e, x) => e);
            List<ExpenseDetail> ExpenseDetailList = new List<ExpenseDetail>();
            var userName = _db.Ledgers.Join(_db.Users, l => l.UserId, u => u.Id, (l, u) => new { Id = u.Id, Name = u.FirstName + " " + u.LastName }).Distinct();
            foreach (var expense in expenses)
            {
                var ledgers = _db.Ledgers.Where(l => l.ExpenseId.Equals(expense.Id));
                var userIdLedger = ledgers.Select(l => l.UserId).Distinct();

                List<ExpenseLedger> ExpenseLedgerList = new List<ExpenseLedger>();

                foreach (var ledger in ledgers)
                {
                    ExpenseLedger expenseLedger = new ExpenseLedger
                    {
                        UserId = ledger.UserId,
                        Name = userName.Where(u => u.Id.Equals(ledger.UserId)).Select(s => s.Name).FirstOrDefault(),
                        Paid = ledger.CreditedAmount,
                        Owes = ledger.DebitedAmount
                    };
                    ExpenseLedgerList.Add(expenseLedger);
                }

                ExpenseDetail expenseDetail = new ExpenseDetail
                {
                    ExpenseLedgers = ExpenseLedgerList,
                    AddedBy = _db.Users.Where(u => u.Id.Equals(expense.AddedBy)).Select(s => s.FirstName).FirstOrDefault(),
                    Amount = expense.Amount,
                    ExpenseId = expense.Id,
                    CreatedOn = expense.CreatedOn,
                    ExpenseType = expense.ExpenseType,
                    Note = expense.Note,
                    Description = expense.Description,
                    GroupId = groupId
                };

                ExpenseDetailList.Add(expenseDetail);
            }

            return ExpenseDetailList;
        }
    }
}