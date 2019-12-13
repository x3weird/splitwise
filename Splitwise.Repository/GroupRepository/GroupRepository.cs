using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Splitwise.DomainModel.Models;
using Splitwise.DomainModel.Models.ApplicationClasses;

namespace Splitwise.Repository.GroupRepository
{
    public class GroupRepository : IGroupRepository
    {
        private readonly SplitwiseDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public GroupRepository(SplitwiseDbContext db, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _db = db;
            _userManager = userManager;
            _mapper = mapper;
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
            var currentUserId = await _db.Users.Where(u => u.Email.Equals(email.ToLower())).Select(s=>s.Id).SingleAsync();
            var query = await _db.Groups.Where(g => g.Name.Equals(groupAdd.Name)).SingleOrDefaultAsync();
            if (query == null)
            {
                //Group group = new Group
                //{
                //    Name = groupAdd.Name,
                //    AddedBy = currentUserId,
                //    CreatedOn = DateTime.Now,
                //    SimplifyDebts = groupAdd.SimplifyDebts,
                //    IsDeleted = false
                //};

                Group group = _mapper.Map<Group>(groupAdd);

                _db.Groups.Add(group);
                await _db.SaveChangesAsync();
                foreach (var x in groupAdd.Users)
                {
                    var User = await _db.Users.Where(u=>u.Email.Equals(x.Email.ToLower())).SingleOrDefaultAsync();
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
                        await _db.GroupMembers.AddAsync(groupMember);

                    }
                    else
                    {
                        var checkFriend = await _db.Friends.Where(f => (f.FriendId.Equals(User.Id) && f.UserId.Equals(currentUserId)) || (f.FriendId.Equals(currentUserId) && f.UserId.Equals(User.Id))).SingleOrDefaultAsync();

                        if (checkFriend == null)
                        {
                            Friend friend = new Friend
                            {
                                FriendId = User.Id,
                                UserId = currentUserId
                            };
                            await _db.Friends.AddAsync(friend);
                        }

                        GroupMember groupMember = new GroupMember
                        {
                            GroupId = group.Id,
                            UserId = User.Id
                        };
                        await _db.GroupMembers.AddAsync(groupMember);

                        
                    }

                    Activity activity = new Activity()
                    {
                        Log = await _db.Users.Where(u => u.Email.Equals(email.ToLower())).Select(s => s.FirstName).FirstOrDefaultAsync() + " created the group " + groupAdd.Name,
                        ActivityOn = "Group",
                        ActivityOnId = group.Id
                    };

                    _db.Activities.Add(activity);
                }
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public async Task<GroupDetails> GetGroupDetails(string groupId)
        {
            Group gp = await _db.Groups.Where(g => g.Id.Equals(groupId) && g.IsDeleted.Equals(false)).SingleOrDefaultAsync();

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
                    //GroupUsers groupUsers = new GroupUsers
                    //{
                    //    Name = q.name,
                    //    Email = q.email
                    //};

                    GroupUsers groupUsers = _mapper.Map<GroupUsers>(q);

                    groupUsersList.Add(groupUsers);
                }

                //GroupDetails groupDetails = new GroupDetails
                //{
                //    GroupId = groupId,
                //    GroupName = gp.Name,
                //    AddedBy = gp.AddedBy,
                //    CreatedOn = gp.CreatedOn,
                //    SimplifyDebts = gp.SimplifyDebts,
                //    Users = groupUsersList
                //};

                GroupDetails groupDetails = _mapper.Map<GroupDetails>(gp);

                return groupDetails;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<UserExpense>> GroupUserExpense(string groupId, List<string> users)
        {
            
            List<string> ledgerIdList = new List<string>();
            List<UserExpense> userExpenses = new List<UserExpense>();
            List<Ledger> ledgers = new List<Ledger>();
        

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
                    Name = await _db.Users.Where(u => u.Id.Equals(userId)).Select(s => s.FirstName).SingleAsync(),
                    Amount = sum,
                    Id = userId
                };
                userExpenses.Add(userExpense);
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
                var check = await _db.GroupExpenses.Where(g => g.ExpenseId.Equals(expenseId) && g.GroupId.Equals(groupId)).SingleOrDefaultAsync();
                if (check != null)
                {
                    expenseIdList.Add(expenseId);
                }
            }
            var expenses = _db.Expenses.Join(expenseIdList, e => e.Id, x => x, (e, x) => e);
            List<ExpenseDetail> ExpenseDetailList = new List<ExpenseDetail>();
            var userName = _db.Ledgers.Join(_db.Users, l => l.UserId, u => u.Id, (l, u) => new { u.Id, Name = u.FirstName }).Distinct();
            foreach (var expense in expenses)
            {
                if (expense.IsDeleted.Equals(false))
                {
                    var ledgers = _db.Ledgers.Where(l => l.ExpenseId.Equals(expense.Id));
                    var userIdLedger = ledgers.Select(l => l.UserId).Distinct();

                    List<ExpenseLedger> ExpenseLedgerList = new List<ExpenseLedger>();

                    foreach (var ledger in ledgers)
                    {
                        //ExpenseLedger expenseLedger = new ExpenseLedger
                        //{
                        //    UserId = ledger.UserId,
                        //    Name = userName.Where(u => u.Id.Equals(ledger.UserId)).Select(s => s.Name).FirstOrDefault(),
                        //    Paid = ledger.CreditedAmount,
                        //    Owes = ledger.DebitedAmount
                        //};

                        ExpenseLedger expenseLedger = _mapper.Map<ExpenseLedger>(ledger);
                        expenseLedger.Name = await userName.Where(u => u.Id.Equals(ledger.UserId)).Select(s => s.Name).SingleAsync();

                        ExpenseLedgerList.Add(expenseLedger);
                    }

                    List<CommentDetails> commentDetails = new List<CommentDetails>();

                    var commentList = await _db.Comments.Where(c => c.ExpenseId.Equals(expense.Id)).ToListAsync();
                    //var commentList = await _db.Comments.Where(c => c.ExpenseId.Equals(expense.Id)).ToListAsync();

                    foreach (var comment in commentList)
                    {
                        //CommentDetails commentDetail = new CommentDetails
                        //{
                        //    Id = comment.Id,
                        //    Content = comment.CommentData,
                        //    UserId = comment.UserId,
                        //    Name = await _db.Users.Where(u => u.Id.Equals(comment.UserId)).Select(s => s.FirstName).FirstOrDefaultAsync()
                        //};

                        CommentDetails commentDetail = _mapper.Map<CommentDetails>(comment);
                        commentDetail.Name = await _db.Users.Where(u => u.Id.Equals(comment.UserId)).Select(s => s.FirstName).SingleAsync();

                        commentDetails.Add(commentDetail);

                        commentDetails.Add(commentDetail);
                    }

                    //ExpenseDetail expenseDetail = new ExpenseDetail
                    //{
                    //    ExpenseLedgers = ExpenseLedgerList,
                    //    AddedBy = _db.Users.Where(u => u.Id.Equals(expense.AddedBy)).Select(s => s.FirstName).FirstOrDefault(),
                    //    Amount = expense.Amount,
                    //    ExpenseId = expense.Id,
                    //    CreatedOn = expense.CreatedOn,
                    //    ExpenseType = expense.ExpenseType,
                    //    Note = expense.Note,
                    //    Description = expense.Description,
                    //    GroupId = groupId
                    //};

                    ExpenseDetail expenseDetail = _mapper.Map<ExpenseDetail>(expense);
                    expenseDetail.AddedBy = await _db.Users.Where(u => u.Id.Equals(expense.AddedBy)).Select(s => s.FirstName).SingleAsync();
                    expenseDetail.ExpenseLedgers = ExpenseLedgerList;
                    expenseDetail.Comments = commentDetails;

                    ExpenseDetailList.Add(expenseDetail);
                }
            }

            return ExpenseDetailList;
        }

        public async Task RemoveGroup(string groupId)
        {
            var groupMember = _db.GroupMembers.Where(gm => gm.GroupId.Equals(groupId));
            _db.GroupMembers.RemoveRange(groupMember);
            await _db.SaveChangesAsync();
            var groupExpense = _db.GroupExpenses.Where(ge => ge.GroupId.Equals(groupId));
            _db.GroupExpenses.RemoveRange(groupExpense);
            await _db.SaveChangesAsync();
            var group = await _db.Groups.Where(g => g.Id.Equals(groupId)).SingleAsync();
            _db.Groups.Remove(group);
        }
    }
}