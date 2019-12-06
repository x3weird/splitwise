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
            var user = await _userManager.FindByEmailAsync(email);
            var expenseIdList = _db.Ledgers.Where(l => l.UserId.Equals(user.Id)).Select(l => l.ExpenseId).Distinct();
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
                    Description = expense.Description
                };

                ExpenseDetailList.Add(expenseDetail);
            }

            return ExpenseDetailList;
        }

        public async Task<int> DeleteExpense(string expenseId)
        {
            var expense = _db.Expenses.Where(e => e.Id.Equals(expenseId)).FirstOrDefault();
            if (expense == null)
            {
                return 0;
            }
            else
            {
                expense.IsDeleted = true;
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
                //Activity activity = new Activity()
                //{
                //    Log = _db.Users.Where(u => u.Email.ToLower().Equals(addExpense.AddedBy.ToLower())).Select(s => s.FirstName).FirstOrDefault() + " added " + addExpense.Description,
                //    ActivityOn = "Expense",
                //    ActivityOnId = expense.Id
                //};
                //_db.Activities.Add(activity);

                Activity activityVar = new Activity()
                {
                    Log = _db.Users.Where(u => u.Id.Equals(addExpense.AddedBy)).Select(s => s.FirstName).FirstOrDefault() + " added " + addExpense.Description + " in " + _db.Groups.Where(g => g.Id.Equals(addExpense.GroupId)).Select(s => s.Name).FirstOrDefault(),
                    ActivityOn = "Group",
                    ActivityOnId = addExpense.GroupId
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
                ActivityOnId = expense.Id
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

        public async Task<AddExpense> EditExpense(string expenseId)
        {
            Expense expense = _db.Expenses.Where(e => e.Id.Equals(expenseId)).FirstOrDefault();
            List<UserExpense> paidBy = new List<UserExpense>();
            List<UserExpense> ledgerList = new List<UserExpense>();
            AddExpense addExpense = new AddExpense()
            {
                AddedBy = expense.AddedBy,
                CreatedOn = expense.CreatedOn,
                Description = expense.Description,
                ExpenseType = expense.ExpenseType,
                //GroupId = expense.GroupId, //changes
                Note = expense.Note,
                Amount = 0
            };

            foreach (var l in _db.Ledgers.Where(l => (l.CreditedAmount > 0) && l.ExpenseId.Equals(expenseId)))
            {
                addExpense.Amount = +l.CreditedAmount;
                UserExpense userExpense = new UserExpense()
                {
                    Id = l.UserId,
                    Amount = l.CreditedAmount,
                    Name = _db.Users.Where(u => u.Id.Equals(l.UserId)).Select(u => u.FirstName).FirstOrDefault()
                };
                paidBy.Add(userExpense);
            }

            foreach (var l in _db.Ledgers.Where(l => (l.DebitedAmount > 0) && l.ExpenseId.Equals(expenseId)))
            {
                UserExpense userExpense = new UserExpense()
                {
                    Id = l.UserId,
                    Amount = l.DebitedAmount,
                    Name = _db.Users.Where(u => u.Id.Equals(l.UserId)).Select(u => u.FirstName).FirstOrDefault()
                };
                ledgerList.Add(userExpense);
            }
            //addExpense.PaidBy = paidBy; //changes
            //addExpense.Ledger = ledgerList;  //changes
            await _db.SaveChangesAsync();
            return addExpense;
        }

        public async Task<int> EditExpense(AddExpense addExpense)
        {
            Expense expense = new Expense()
            {
                AddedBy = addExpense.AddedBy,
                CreatedOn = addExpense.CreatedOn,
                Description = addExpense.Description,
                ExpenseType = addExpense.ExpenseType,
                //GroupId = addExpense.GroupId,  //changes
                IsDeleted = false,
                Note = addExpense.Note,
                Amount = 0
            };
            foreach (var e in addExpense.PaidBy)
            {
                expense.Amount = +e.Amount;
                var entity = _db.Expenses.Attach(expense);
                entity.State = EntityState.Modified;
                Ledger ledger = new Ledger()
                {
                    //UserId = e.Id,  //changes
                    CreditedAmount = e.Amount,
                    ExpenseId = expense.Id
                };
                var entity2 = _db.Ledgers.Attach(ledger);
                entity2.State = EntityState.Modified;
            }
            foreach (var e in addExpense.Ledger)
            {
                Ledger ledger = new Ledger()
                {
                    //UserId = e.Id,  //changes
                    DebitedAmount = e.Amount,
                    ExpenseId = expense.Id
                };
                var entity = _db.Ledgers.Attach(ledger);
                entity.State = EntityState.Modified;
            }
            await _db.SaveChangesAsync();
            return 1;
        }

        public async Task<List<UserExpense>> Dashboard(string email)
        {
            string currentUserId = _db.Users.Where(u => u.Email.Equals(email.ToLower())).Select(s => s.Id).Single();
            var expenseIdList = _db.Ledgers.Where(l => l.UserId.Equals(currentUserId)).Select(s => s.ExpenseId).Distinct().ToList();
            var userIdList = await _db.Ledgers.Join(expenseIdList, l => l.ExpenseId, e => e, (l, e) => new { userId = l.UserId }).Distinct().ToListAsync();


            List<UserExpense> userExpenseList = new List<UserExpense>();
            foreach (var u in userIdList)
            {
                if (u.userId != currentUserId)
                {
                    foreach (var expenseId in expenseIdList)
                    {
                        foreach (var l in _db.Ledgers.Where(l => (l.UserId.Equals(currentUserId) || l.UserId.Equals(u.userId)) && l.ExpenseId.Equals(expenseId)))
                        {
                            if (l.UserId == currentUserId && l.CreditedAmount > 0)
                            {
                                //var a = userExpenseList.Find(ue => ue.Id == l.UserId);
                                var a = userExpenseList.Where(ue => ue.Id.Equals(u.userId)).FirstOrDefault();
                                if (a == null)
                                {
                                    UserExpense userExpense = new UserExpense()
                                    {
                                        Id = u.userId,
                                        Name = _db.Users.Where(us => us.Id.Equals(u.userId)).Select(s => s.FirstName).FirstOrDefault(),
                                        Amount = l.DebitedAmount
                                    };
                                    userExpenseList.Add(userExpense);
                                }
                                else
                                {
                                    a.Amount = a.Amount + l.DebitedAmount;
                                }
                            }
                            else if (l.UserId == u.userId && l.CreditedAmount > 0)
                            {
                                //var a = userExpenseList.Find(ue => ue.Id == l.UserId);
                                var a = userExpenseList.Where(ue => ue.Id.Equals(u.userId)).FirstOrDefault();
                                if (a == null)
                                {
                                    UserExpense userExpenses = new UserExpense()
                                    {
                                        Id = u.userId,
                                        Name = _db.Users.Where(us => us.Id.Equals(u.userId)).Select(s => s.FirstName).FirstOrDefault(),
                                        Amount = l.DebitedAmount
                                    };
                                    userExpenseList.Add(userExpenses);
                                }
                                else
                                {

                                    a.Amount = a.Amount + l.DebitedAmount;
                                }
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

            _db.Ledgers.Add(ledgerPayer);
            _db.Ledgers.Add(ledgerRecipient);
            await _db.SaveChangesAsync();

        }
    }
}
