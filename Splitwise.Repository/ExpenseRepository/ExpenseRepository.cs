using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Splitwise.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splitwise.Repository.ExpenseRepository
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly SplitwiseDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public ExpenseRepository(SplitwiseDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public async Task<List<ExpenseDetail>> GetExpenseList(string email)
        {
            //current user
            var user = await _userManager.FindByEmailAsync(email);

            //list of all expense which is related to current user
            var expenseIdList = _db.Ledgers.Where(l => l.UserId.Equals(user.Id)).Select(l => l.ExpenseId).Distinct();

            var expenses = _db.Expenses.Join(expenseIdList, e => e.Id, x => x, (e, x) => e);
            List<ExpenseDetail> ExpenseDetailList = new List<ExpenseDetail>();
            var userName = _db.Ledgers.Join(_db.Users, l => l.UserId, u => u.Id, (l, u) => new { u.Id, Name = u.FirstName}).Distinct();
            foreach (var expense in expenses)
            {
                if (expense.IsDeleted == false)
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

                    var commentList = await _db.Comments.Where(c => c.ExpenseId.Equals(expense.Id)).Select(s => s.CommentData).ToListAsync();

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
                        Comments = commentList
                    };

                    ExpenseDetailList.Add(expenseDetail);
                }
                
            }

            return ExpenseDetailList;
        }

        public async Task<int> DeleteExpense(string expenseId, string currentUserId)
        {
            var expense = _db.Expenses.Where(e => e.Id.Equals(expenseId)).FirstOrDefault();
            if (expense == null)
            {
                return 0;
            }
            else
            {
                expense.IsDeleted = true;
                
                var group = _db.GroupExpenses.Where(g => g.ExpenseId.Equals(expenseId)).FirstOrDefault();
                Activity activity = new Activity
                {
                    Log = _db.Users.Where(u=>u.Id.Equals(currentUserId)).Select(s=>s.FirstName).FirstOrDefault() + " deleted " + expense.Description,
                    ActivityOn = group == null?"Expense":"Group",
                    ActivityOnId = group == null ? expense.Id : group.GroupId,
                    Date = DateTime.Now
                };

                _db.Activities.Add(activity);
                await _db.SaveChangesAsync();
                return 1;
            }

            

        }

        public async Task AddExpense(AddExpense addExpense)
        {
            addExpense.AddedBy = _db.Users.Where(u => u.Email.Equals(addExpense.AddedBy.ToLower())).Select(s => s.Id).FirstOrDefault();
            string userId;

            foreach (var item in addExpense.EmailList)
            {

                var Id = _db.Users.Where(u => u.Email.Equals(item.ToLower())).Select(s => s.Id).FirstOrDefault();

                if (addExpense.AddedBy != Id)
                {
                    if (Id == null)
                    {
                        var name = item;
                        int index = name.IndexOf('@');
                        if (index > 0)
                            name = name.Substring(0, index);
                        var user = new ApplicationUser
                        {
                            UserName = item,
                            Email = item,
                            FirstName = name,
                            LastName = item,
                            Currency = "INR",
                            PhoneNumber = "1111111111",
                            IsRegistered = false
                        };
                        var addedUser = _userManager.CreateAsync(user, "Random@123");
                        userId = user.Id;
                        Friend friend = new Friend()
                        {
                            FriendId = user.Id,
                            UserId = addExpense.AddedBy
                        };

                        _db.Friends.Add(friend);
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        var checkFriend = _db.Friends.Where(f => f.FriendId.Equals(Id) && f.UserId.Equals(addExpense.AddedBy)).FirstOrDefault();
                        if (checkFriend == null)
                        {
                            Friend friend = new Friend()
                            {
                                FriendId = Id,
                                UserId = addExpense.AddedBy
                            };

                            _db.Friends.Add(friend);
                            await _db.SaveChangesAsync();
                        }

                    }
                }


                _db.SaveChanges();
            }

            Expense expense = new Expense()
            {
                AddedBy = addExpense.AddedBy,
                CreatedOn = addExpense.CreatedOn,
                Description = addExpense.Description,
                ExpenseType = addExpense.ExpenseType,
                IsDeleted = false,
                Note = addExpense.Note,
                Amount = addExpense.Amount
            };

            var addedExpense = _db.Expenses.Add(expense);

            if (addExpense.GroupId != "")
            {
              
                Activity activityVar = new Activity()
                {
                    Log = _db.Users.Where(u => u.Id.Equals(addExpense.AddedBy)).Select(s => s.FirstName).FirstOrDefault() + " added " + addExpense.Description + " in " + _db.Groups.Where(g => g.Id.Equals(addExpense.GroupId)).Select(s => s.Name).FirstOrDefault(),
                    ActivityOn = "Group",
                    ActivityOnId = addExpense.GroupId,
                    Date = DateTime.Now
                };
                _db.Activities.Add(activityVar);

                GroupExpense groupExpense = new GroupExpense()
                {
                    ExpenseId = expense.Id,
                    GroupId = addExpense.GroupId
                };
                _db.GroupExpenses.Add(groupExpense); 
            }

            Activity activity = new Activity()
            {
                Log = _db.Users.Where(u => u.Id.Equals(addExpense.AddedBy)).Select(s => s.FirstName).FirstOrDefault() + " added " + addExpense.Description,
                ActivityOn = "Expense",
                ActivityOnId = expense.Id,
                Date = DateTime.Now
            };
            _db.Activities.Add(activity);

            foreach (var item in addExpense.EmailList)
            {
                Ledger ledger = new Ledger();
                foreach (var p in addExpense.PaidBy)
                {
                    if (item == p.Email)
                    {
                        ledger.CreditedAmount = p.Amount;
                    }
                }

                foreach (var p in addExpense.Ledger)
                {
                    if (item == p.Email)
                    {
                        if (ledger.CreditedAmount > 0)
                        {
                            ledger.DebitedAmount = ledger.CreditedAmount - p.Amount;
                        }
                        else
                        {
                            ledger.DebitedAmount = 0 - p.Amount;
                            
                        }

                    }
                }

                ledger.ExpenseId = expense.Id;
                ledger.UserId = _db.Users.Where(u => u.Email.Equals(item.ToLower())).Select(s => s.Id).FirstOrDefault();
                if (ledger.DebitedAmount > 0)
                {
                    ActivityUser activityUser = new ActivityUser()
                    {
                        Log = _db.Users.Where(u => u.Email.Equals(ledger.UserId.ToLower())).Select(s => s.FirstName).FirstOrDefault() + " gets back ₹" + ledger.DebitedAmount,
                        ActivityId = activity.Id,
                        ActivityUserId = _db.Users.Where(u => u.Email.Equals(item.ToLower())).Select(s => s.Id).FirstOrDefault()
                    };
                    _db.ActivityUsers.Add(activityUser);
                } else
                {
                    ActivityUser activityUser = new ActivityUser()
                    {
                        Log = _db.Users.Where(u => u.Email.Equals(ledger.UserId.ToLower())).Select(s => s.FirstName).FirstOrDefault() + " owe ₹" + ledger.DebitedAmount,
                        ActivityId = activity.Id,
                        ActivityUserId = _db.Users.Where(u => u.Email.Equals(item.ToLower())).Select(s => s.Id).FirstOrDefault()
                    };
                    _db.ActivityUsers.Add(activityUser);
                }
                

                _db.Ledgers.Add(ledger);
                _db.SaveChanges();
            }
        }

        public async Task<List<UserExpense>> Dashboard(string email)
        {
            string currentUserId = _db.Users.Where(u => u.Email.Equals(email.ToLower())).Select(s => s.Id).Single();
            var expenseIdList = _db.Ledgers.Where(l => l.UserId.Equals(currentUserId)).Select(s => s.ExpenseId).Distinct().ToList();
            var userIdList = await _db.Ledgers.Join(expenseIdList, l => l.ExpenseId, e => e, (l, e) => new { userId = l.UserId }).Distinct().ToListAsync();


            List<UserExpense> userExpenseList = new List<UserExpense>();
            

            foreach (var userId in userIdList)
            {
                foreach (var expenseId in expenseIdList)
                {
                    if(_db.Expenses.Where(e=>e.Description.Equals("Settle-Up") && e.Id.Equals(expenseId)).FirstOrDefault()==null)
                    {
                        if(_db.Expenses.Where(e=>e.IsDeleted.Equals(false) && e.Id.Equals(expenseId)).FirstOrDefault() != null)
                        {
                            var check = await _db.Ledgers.Where(l => l.UserId.Equals(currentUserId) && l.DebitedAmount > 0 && l.ExpenseId.Equals(expenseId)).FirstOrDefaultAsync();
                            if (check != null)
                            {
                                var userCheck = await _db.Ledgers.Where(l => l.UserId.Equals(userId.userId) && l.ExpenseId.Equals(check.ExpenseId) && l.DebitedAmount < 0).FirstOrDefaultAsync();
                                if (userCheck != null)
                                {
                                    var a = userExpenseList.Where(ue => ue.Id.Equals(userCheck.UserId)).FirstOrDefault();
                                    if (a == null)
                                    {
                                        UserExpense userExpense = new UserExpense()
                                        {
                                            Id = userCheck.UserId,
                                            Name = _db.Users.Where(us => us.Id.Equals(userCheck.UserId)).Select(s => s.FirstName).FirstOrDefault(),
                                            Amount = -userCheck.DebitedAmount
                                        };
                                        userExpenseList.Add(userExpense);
                                    }
                                    else
                                    {
                                        a.Amount = a.Amount - userCheck.DebitedAmount;
                                    }
                                }
                            }

                            var check2 = await _db.Ledgers.Where(l => l.UserId.Equals(currentUserId) && l.DebitedAmount < 0 && l.ExpenseId.Equals(expenseId)).FirstOrDefaultAsync();
                            if (check2 != null)
                            {
                                var userCheck = await _db.Ledgers.Where(l => l.UserId.Equals(userId.userId) && l.ExpenseId.Equals(check.ExpenseId) && l.DebitedAmount > 0).FirstOrDefaultAsync();
                                if (userCheck != null)
                                {
                                    var a = userExpenseList.Where(ue => ue.Id.Equals(userCheck.UserId)).FirstOrDefault();
                                    if (a == null)
                                    {
                                        UserExpense userExpense = new UserExpense()
                                        {
                                            Id = userCheck.UserId,
                                            Name = _db.Users.Where(us => us.Id.Equals(userCheck.UserId)).Select(s => s.FirstName).FirstOrDefault(),
                                            Amount = -userCheck.DebitedAmount
                                        };
                                        userExpenseList.Add(userExpense);
                                    }
                                    else
                                    {
                                        a.Amount = a.Amount - userCheck.DebitedAmount;
                                    }
                                }

                            }
                        }
                        
                    }
                    
                }
            }
            List<Expense> settleUpList = new List<Expense>();
            foreach (var expenseId in expenseIdList)
            {
                var expense = await _db.Expenses.Where(e => e.Id.Equals(expenseId) && e.Description.Equals("Settle-Up") && e.IsDeleted.Equals(false)).FirstOrDefaultAsync();
                if (expense != null)
                {
                    settleUpList.Add(expense);
                }
            }

            expenseIdList = settleUpList.Select(e=>e.Id).ToList();

            foreach (var userId in userIdList)
            {
                foreach (var expenseId in expenseIdList)
                {
                    var check = await _db.Ledgers.Where(l => l.ExpenseId.Equals(expenseId) && l.UserId.Equals(currentUserId) && l.DebitedAmount > 0).FirstOrDefaultAsync();
                    if(check != null)
                    {
                        var userCheck = await _db.Ledgers.Where(l => l.ExpenseId.Equals(expenseId) && l.UserId.Equals(userId.userId) && l.DebitedAmount < 0).FirstOrDefaultAsync();
                        if (userCheck != null)
                        {
                            var a = userExpenseList.Where(ue => ue.Id.Equals(userCheck.UserId)).FirstOrDefault();
                            if (a == null)
                            {
                                UserExpense userExpense = new UserExpense()
                                {
                                    Id = userCheck.UserId,
                                    Name = _db.Users.Where(us => us.Id.Equals(userCheck.UserId)).Select(s => s.FirstName).FirstOrDefault(),
                                    Amount = -userCheck.DebitedAmount
                                };
                                userExpenseList.Add(userExpense);
                            }
                            else
                            {
                                a.Amount = a.Amount + userCheck.DebitedAmount;
                            }
                        }
                    }

                    var check2 = await _db.Ledgers.Where(l => l.ExpenseId.Equals(expenseId) && l.UserId.Equals(currentUserId) && l.DebitedAmount < 0).FirstOrDefaultAsync();
                    if (check2 != null)
                    {
                        var userCheck = await _db.Ledgers.Where(l => l.ExpenseId.Equals(expenseId) && l.UserId.Equals(userId.userId) && l.DebitedAmount > 0).FirstOrDefaultAsync();
                        if (userCheck != null)
                        {
                            var a = userExpenseList.Where(ue => ue.Id.Equals(userCheck.UserId)).FirstOrDefault();
                            if (a == null)
                            {
                                UserExpense userExpense = new UserExpense()
                                {
                                    Id = userCheck.UserId,
                                    Name = _db.Users.Where(us => us.Id.Equals(userCheck.UserId)).Select(s => s.FirstName).FirstOrDefault(),
                                    Amount = -userCheck.DebitedAmount
                                };
                                userExpenseList.Add(userExpense);
                            }
                            else
                            {
                                a.Amount = a.Amount + userCheck.DebitedAmount;
                            }
                        }
                    }
                }
            }
                    return userExpenseList;
        }


            


        public async Task SettleUp(SettleUp settleUp, string email)
        {
            string AddedBy = _db.Users.Where(u => u.Email.Equals(email.ToLower())).Select(s => s.Id).FirstOrDefault();

            Expense expense = new Expense()
            {
                AddedBy = AddedBy,
                CreatedOn = settleUp.Date,
                Description = "Settle-Up",
                ExpenseType = "Settle-Up",
                IsDeleted = false,
                Note = settleUp.Note,
                Amount = settleUp.Amount
            };

            var addedExpense = _db.Expenses.Add(expense);
            await _db.SaveChangesAsync();
            if (settleUp.Group != "")
            {
                GroupExpense groupExpense = new GroupExpense()
                {
                    ExpenseId = expense.Id,
                    GroupId = settleUp.Group
                };
                _db.GroupExpenses.Add(groupExpense);
            }

            if (settleUp.Payer == "you")
            {
                settleUp.Payer = AddedBy;
            }
            if (settleUp.Recipient == "you")
            {
                settleUp.Recipient = AddedBy;
            }

            Ledger ledgerPayer = new Ledger()
            {
                ExpenseId = expense.Id,
                UserId = settleUp.Payer,
                DebitedAmount = settleUp.Amount,
                CreditedAmount = 0
            };

            Ledger ledgerRecipient = new Ledger()
            {
                ExpenseId = expense.Id,
                UserId = settleUp.Recipient,
                DebitedAmount = -settleUp.Amount,
                CreditedAmount = 0
            };

            Activity activity = new Activity
            {
                Log = _db.Users.Where(u=>u.Id.Equals(settleUp.Payer)).Select(s=>s.FirstName).FirstOrDefault() + " Paid " + _db.Users.Where(u => u.Id.Equals(settleUp.Recipient)).Select(s => s.FirstName).FirstOrDefault(),
                ActivityOn = "Expense",
                ActivityOnId = expense.Id,
                Date = DateTime.Now
            };

            _db.Activities.Add(activity);

            ActivityUser activityUser = new ActivityUser
            {
                Log = "you paid " + settleUp.Amount,
                ActivityUserId = settleUp.Payer,
                ActivityId = activity.Id
            };

            ActivityUser activityUser2 = new ActivityUser
            {
                Log = "you get back " + settleUp.Amount,
                ActivityUserId = settleUp.Recipient,
                ActivityId = activity.Id
            };

            _db.ActivityUsers.Add(activityUser);
            _db.ActivityUsers.Add(activityUser2);

            _db.Ledgers.Add(ledgerPayer);
            _db.Ledgers.Add(ledgerRecipient);
            await _db.SaveChangesAsync();

        }

        public async Task UnDeleteExpense(string expenseId, string currentUserId)
        {
            var expense = _db.Expenses.Where(e => e.Id.Equals(expenseId)).FirstOrDefault();
            if(expense!=null)
            {
                expense.IsDeleted = false;
            }

            var group = _db.GroupExpenses.Where(g => g.ExpenseId.Equals(expenseId)).FirstOrDefault();
            Activity activity = new Activity
            {
                Log = _db.Users.Where(u => u.Id.Equals(currentUserId)).Select(s => s.FirstName).FirstOrDefault() + " unDeleted " + expense.Description,
                ActivityOn = group == null ? "Expense" : "Group",
                ActivityOnId = group == null ? expense.Id : group.GroupId,
                Date = DateTime.Now
            };

            _db.Activities.Add(activity);

            await _db.SaveChangesAsync();
        }
    }
}
