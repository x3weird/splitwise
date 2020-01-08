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
using Splitwise.Repository.DataRepository;

namespace Splitwise.Repository.GroupRepository
{
    public class GroupRepository : IGroupRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IDataRepository _dal;

        public GroupRepository(UserManager<ApplicationUser> userManager, IMapper mapper, IDataRepository dal)
        {
            _userManager = userManager;
            _mapper = mapper;
            _dal = dal;
        }

        public async Task<List<UserNameWithId>> GetGroupList()
        {
            var allGroupList = await _dal.Get<Group>();
            List<UserNameWithId> query = allGroupList.Select(g => new UserNameWithId
            {
                UserId = g.Id,
                Name = g.Name
            }).ToList();

            return query;
        }

        public async Task<int> AddGroupMembers(GroupAdd groupAdd, string email, Group group)
        {
            var currentUserId = await _dal.Where<ApplicationUser>(u => u.Email.Equals(email.ToLower())).Select(s=>s.Id).SingleAsync();
            var query = await _dal.Where<Group>(g => g.Name.Equals(groupAdd.Name)).SingleOrDefaultAsync();
            if (query != null)
            {
                foreach (var x in groupAdd.Users)
                {
                    var User = await _dal.Where<ApplicationUser>(u=>u.Email.Equals(x.Email.ToLower())).SingleOrDefaultAsync();
                    if (User != null)
                    {
                        var checkFriend = await _dal.Where<Friend>(f => (f.FriendId.Equals(User.Id) && f.UserId.Equals(currentUserId)) || (f.FriendId.Equals(currentUserId) && f.UserId.Equals(User.Id))).SingleOrDefaultAsync();

                        if (checkFriend == null)
                        {
                            Friend friend = new Friend
                            {
                                FriendId = User.Id,
                                UserId = currentUserId
                            };
                            await _dal.AddAsync<Friend>(friend);
                        }

                        GroupMember groupMember = new GroupMember
                        {
                            GroupId = group.Id,
                            UserId = User.Id
                        };
                        await _dal.AddAsync<GroupMember>(groupMember);                        
                    }

                    Activity activity = new Activity()
                    {
                        Log = await _dal.Where<ApplicationUser>(u => u.Email.Equals(email.ToLower())).Select(s => s.FirstName).FirstOrDefaultAsync() + " created the group " + groupAdd.Name,
                        ActivityOn = "Group",
                        ActivityOnId = group.Id
                    };

                    await _dal.AddAsync<Activity>(activity);
                }
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public async Task<Group> AddGroup(GroupAdd groupAdd, string email)
        {
            var currentUserId = await _dal.Where<ApplicationUser>(u => u.Email.Equals(email.ToLower())).Select(s => s.Id).SingleAsync();
            var query = await _dal.Where<Group>(g => g.Name.Equals(groupAdd.Name)).SingleOrDefaultAsync();
            if (query == null)
            {
                Group group = _mapper.Map<Group>(groupAdd);
                await _dal.AddAsync<Group>(group);
                foreach (var x in groupAdd.Users)
                {
                    var User = await _dal.Where<ApplicationUser>(u => u.Email.Equals(x.Email.ToLower())).SingleOrDefaultAsync();
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
                    }


                    Activity activity = new Activity()
                    {
                        Log = await _dal.Where<ApplicationUser>(u => u.Email.Equals(email.ToLower())).Select(s => s.FirstName).FirstOrDefaultAsync() + " created the group " + groupAdd.Name,
                        ActivityOn = "Group",
                        ActivityOnId = group.Id
                    };

                    await _dal.AddAsync<Activity>(activity);
                }
                return group;
            } else
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
                    Name = await _dal.Where<ApplicationUser>(u => u.Id.Equals(userId)).Select(s => s.FirstName).SingleAsync(),
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
            var allLedgerList = await _dal.Get<Ledger>();
            var allExpenseList = await _dal.Get<Expense>();
            var allUserList = await _dal.Get<ApplicationUser>();
            List<string> expenseIdList = new List<string>();
            var expenseList = _dal.Where<Ledger>(l => l.UserId.Equals(user.Id)).Select(l => l.ExpenseId).Distinct();
            foreach (var expenseId in expenseList)
            {
                var check = await _dal.Where<GroupExpense>(g => g.ExpenseId.Equals(expenseId) && g.GroupId.Equals(groupId)).SingleOrDefaultAsync();
                if (check != null)
                {
                    expenseIdList.Add(expenseId);
                }
            }
            
            var expenses = allExpenseList.Join(expenseIdList, e => e.Id, x => x, (e, x) => e);
            List<ExpenseDetail> ExpenseDetailList = new List<ExpenseDetail>();
            
            var userName = allLedgerList.Join(allUserList, l => l.UserId, u => u.Id, (l, u) => new { u.Id, Name = u.FirstName }).Distinct();
            foreach (var expense in expenses)
            {
                if (expense.IsDeleted.Equals(false))
                {
                    var ledgers = _dal.Where<Ledger>(l => l.ExpenseId.Equals(expense.Id));
                    var userIdLedger = ledgers.Select(l => l.UserId).Distinct();

                    List<ExpenseLedger> ExpenseLedgerList = new List<ExpenseLedger>();

                    foreach (var ledger in ledgers)
                    {

                        ExpenseLedger expenseLedger = _mapper.Map<ExpenseLedger>(ledger);
                        expenseLedger.Name = userName.Where(u => u.Id.Equals(ledger.UserId)).Select(s => s.Name).Single();

                        ExpenseLedgerList.Add(expenseLedger);
                    }

                    List<CommentDetails> commentDetails = new List<CommentDetails>();

                    var commentList = await _dal.Where<Comment>(c => c.ExpenseId.Equals(expense.Id)).ToListAsync();
                    
                    foreach (var comment in commentList)
                    {
                        
                        CommentDetails commentDetail = _mapper.Map<CommentDetails>(comment);
                        commentDetail.Name = await _dal.Where<ApplicationUser>(u => u.Id.Equals(comment.UserId)).Select(s => s.FirstName).SingleAsync();

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
                    expenseDetail.AddedBy = await _dal.Where<ApplicationUser>(u => u.Id.Equals(expense.AddedBy)).Select(s => s.FirstName).SingleAsync();
                    expenseDetail.ExpenseLedgers = ExpenseLedgerList;
                    expenseDetail.Comments = commentDetails;

                    ExpenseDetailList.Add(expenseDetail);
                }
            }

            return ExpenseDetailList;
        }

        public async Task RemoveGroup(string groupId)
        {
            List<GroupMember> groupMember = await _dal.Where<GroupMember>(gm => gm.GroupId.Equals(groupId)).ToListAsync();
            _dal.RemoveRange<GroupMember>(groupMember);
            List<GroupExpense> groupExpense =  await _dal.Where<GroupExpense>(ge => ge.GroupId.Equals(groupId)).ToListAsync();
            _dal.RemoveRange<GroupExpense>(groupExpense);
            var group = await _dal.Where<Group>(g => g.Id.Equals(groupId)).SingleAsync();
            _dal.Remove<Group>(group);
        }
    }
}