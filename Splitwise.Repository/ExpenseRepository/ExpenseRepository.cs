﻿using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Splitwise.DomainModel.Models;
using Splitwise.DomainModel.Models.ApplicationClasses;
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
        private readonly IMapper _mapper;

        public ExpenseRepository(SplitwiseDbContext db, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _db = db;
            _userManager = userManager;
            _mapper = mapper;
        }
        public async Task<List<ExpenseDetail>> GetExpenseList(string email)
        {
            //current user
            var user = await _userManager.FindByEmailAsync(email);

            //list of all expense which is related to current user
            var expenseIdList = _db.Ledgers.Where(l => l.UserId.Equals(user.Id)).Select(l => l.ExpenseId).Distinct();

            var expenses = await _db.Expenses.Join(expenseIdList, e => e.Id, x => x, (e, x) => e).ToListAsync();
            List<ExpenseDetail> ExpenseDetailList = new List<ExpenseDetail>();
            var userName = _db.Ledgers.Join(_db.Users, l => l.UserId, u => u.Id, (l, u) => new { u.Id, Name = u.FirstName}).Distinct();
            foreach (var expense in expenses)
            {
                if (expense.IsDeleted == false)
                {
                    var ledgers = await _db.Ledgers.Where(l => l.ExpenseId.Equals(expense.Id)).ToListAsync();
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

                    foreach (var comment in commentList)
                    {
                        //CommentDetails commentDetail = new CommentDetails
                        //{
                        //    Id = comment.Id,
                        //    Content = comment.CommentData,
                        //    UserId = comment.UserId,
                        //    Name = await _db.Users.Where(u => u.Id.Equals(comment.UserId)).Select(s=>s.FirstName).FirstOrDefaultAsync()
                        //};

                        CommentDetails commentDetail = _mapper.Map<CommentDetails>(comment);
                        commentDetail.Name = await _db.Users.Where(u => u.Id.Equals(comment.UserId)).Select(s => s.FirstName).SingleAsync();

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
                    //    Comments = commentList
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

        public async Task<int> DeleteExpense(string expenseId, string currentUserId)
        {
            var expense = await _db.Expenses.Where(e => e.Id.Equals(expenseId)).SingleOrDefaultAsync();
            if (expense == null)
            {
                return 0;
            }
            else
            {
                expense.IsDeleted = true;
                
                var group = await _db.GroupExpenses.Where(g => g.ExpenseId.Equals(expenseId)).SingleOrDefaultAsync();
                Activity activity = new Activity
                {
                    Log = await _db.Users.Where(u=>u.Id.Equals(currentUserId)).Select(s=>s.FirstName).SingleAsync() + " deleted " + expense.Description,
                    ActivityOn = group == null?"Expense":"Group",
                    ActivityOnId = group == null ? expense.Id : group.GroupId,
                    Date = DateTime.Now
                };

                _db.Activities.Add(activity);
                return 1;
            }

            

        }

        public async Task AddExpense(AddExpense addExpense)
        {
            addExpense.AddedBy = await _db.Users.Where(u => u.Email.Equals(addExpense.AddedBy.ToLower())).Select(s => s.Id).SingleAsync();
            string userId;

            foreach (var item in addExpense.EmailList)
            {

                var Id = await _db.Users.Where(u => u.Email.Equals(item.ToLower())).Select(s => s.Id).SingleOrDefaultAsync();

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
                        var checkFriend = await _db.Friends.Where(f => f.FriendId.Equals(Id) && f.UserId.Equals(addExpense.AddedBy)).SingleOrDefaultAsync();
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
                await _db.SaveChangesAsync();
            }

            //Expense expense = new Expense()
            //{
            //    AddedBy = addExpense.AddedBy,
            //    CreatedOn = addExpense.CreatedOn,
            //    Description = addExpense.Description,
            //    ExpenseType = addExpense.ExpenseType,
            //    IsDeleted = false,
            //    Note = addExpense.Note,
            //    Amount = addExpense.Amount
            //};

            Expense expense = _mapper.Map<Expense>(addExpense);

            var addedExpense = _db.Expenses.Add(expense);

            if (addExpense.GroupId != "")
            {
              
                Activity activityVar = new Activity()
                {
                    Log = await _db.Users.Where(u => u.Id.Equals(addExpense.AddedBy)).Select(s => s.FirstName).FirstOrDefaultAsync() + " added " + addExpense.Description + " in " + _db.Groups.Where(g => g.Id.Equals(addExpense.GroupId)).Select(s => s.Name).SingleAsync(),
                    ActivityOn = "Group",
                    ActivityOnId = addExpense.GroupId,
                    Date = DateTime.Now
                };
                await _db.Activities.AddAsync(activityVar);

                GroupExpense groupExpense = new GroupExpense()
                {
                    ExpenseId = expense.Id,
                    GroupId = addExpense.GroupId
                };
                await _db.GroupExpenses.AddAsync(groupExpense); 
            }

            Activity activity = new Activity()
            {
                Log = await _db.Users.Where(u => u.Id.Equals(addExpense.AddedBy)).Select(s => s.FirstName).SingleAsync() + " added " + addExpense.Description,
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
                ledger.UserId = await _db.Users.Where(u => u.Email.Equals(item.ToLower())).Select(s => s.Id).SingleAsync();
                if (ledger.DebitedAmount > 0)
                {
                    //ActivityUser activityUser = new ActivityUser()
                    //{
                    //    Log = _db.Users.Where(u => u.Email.Equals(ledger.UserId.ToLower())).Select(s => s.FirstName).FirstOrDefault() + " gets back ₹" + ledger.DebitedAmount,
                    //    ActivityId = activity.Id,
                    //    ActivityUserId = _db.Users.Where(u => u.Email.Equals(item.ToLower())).Select(s => s.Id).FirstOrDefault()
                    //};

                    ActivityUser activityUser = _mapper.Map<ActivityUser>(activity);
                    activityUser.Log = await _db.Users.Where(u => u.Email.Equals(ledger.UserId.ToLower())).Select(s => s.FirstName).SingleAsync() + " gets back ₹" + ledger.DebitedAmount;
                    activityUser.ActivityUserId = await _db.Users.Where(u => u.Email.Equals(item.ToLower())).Select(s => s.Id).SingleAsync();

                    await _db.ActivityUsers.AddAsync(activityUser);
                } else
                {
                    //ActivityUser activityUser = new ActivityUser()
                    //{
                    //    Log = _db.Users.Where(u => u.Email.Equals(ledger.UserId.ToLower())).Select(s => s.FirstName).FirstOrDefault() + " owe ₹" + ledger.DebitedAmount,
                    //    ActivityId = activity.Id,
                    //    ActivityUserId = _db.Users.Where(u => u.Email.Equals(item.ToLower())).Select(s => s.Id).FirstOrDefault()
                    //};

                    ActivityUser activityUser = _mapper.Map<ActivityUser>(activity);
                    activityUser.Log = await _db.Users.Where(u => u.Email.Equals(ledger.UserId.ToLower())).Select(s => s.FirstName).SingleAsync() + " owe ₹" + ledger.DebitedAmount;
                    activityUser.ActivityUserId = await _db.Users.Where(u => u.Email.Equals(item.ToLower())).Select(s => s.Id).SingleAsync();

                    _db.ActivityUsers.Add(activityUser);
                }
                _db.Ledgers.Add(ledger);
            }
        }

        public async Task<List<UserExpense>> Dashboard(string email)
        {
            string currentUserId = await _db.Users.Where(u => u.Email.Equals(email.ToLower())).Select(s => s.Id).SingleAsync();
            var expenseIdList = await _db.Ledgers.Where(l => l.UserId.Equals(currentUserId)).Select(s => s.ExpenseId).Distinct().ToListAsync();
            var userIdList = await _db.Ledgers.Join(expenseIdList, l => l.ExpenseId, e => e, (l, e) => new { userId = l.UserId }).Distinct().ToListAsync();

            List<UserExpense> userExpenseList = new List<UserExpense>();
            

            foreach (var userId in userIdList)
            {
                foreach (var expenseId in expenseIdList)
                {
                    if(await _db.Expenses.Where(e=>e.Description.Equals("Settle-Up") && e.Id.Equals(expenseId)).SingleOrDefaultAsync()==null)
                    {
                        if(await _db.Expenses.Where(e=>e.IsDeleted.Equals(false) && e.Id.Equals(expenseId)).SingleOrDefaultAsync() != null)
                        {
                            var check = await _db.Ledgers.Where(l => l.UserId.Equals(currentUserId) && l.DebitedAmount > 0 && l.ExpenseId.Equals(expenseId)).SingleOrDefaultAsync();
                            if (check != null)
                            {
                                var userCheck = await _db.Ledgers.Where(l => l.UserId.Equals(userId.userId) && l.ExpenseId.Equals(check.ExpenseId) && l.DebitedAmount < 0).FirstOrDefaultAsync();
                                if (userCheck != null)
                                {
                                    var a = userExpenseList.Where(ue => ue.Id.Equals(userCheck.UserId)).FirstOrDefault();
                                    if (a == null)
                                    {
                                        //UserExpense userExpense = new UserExpense()
                                        //{
                                        //    Id = userCheck.UserId,
                                        //    Name = _db.Users.Where(us => us.Id.Equals(userCheck.UserId)).Select(s => s.FirstName).FirstOrDefault(),
                                        //    Amount = -userCheck.DebitedAmount
                                        //};

                                        UserExpense userExpense = _mapper.Map<UserExpense>(userCheck);
                                        userExpense.Name = await _db.Users.Where(us => us.Id.Equals(userCheck.UserId)).Select(s => s.FirstName).SingleAsync();
                                        userExpenseList.Add(userExpense);
                                    }
                                    else
                                    {
                                        a.Amount = a.Amount - userCheck.DebitedAmount;
                                    }
                                }
                            }

                            var check2 = await _db.Ledgers.Where(l => l.UserId.Equals(currentUserId) && l.DebitedAmount < 0 && l.ExpenseId.Equals(expenseId)).SingleOrDefaultAsync();
                            if (check2 != null)
                            {
                                var userCheck = await _db.Ledgers.Where(l => l.UserId.Equals(userId.userId) && l.ExpenseId.Equals(check.ExpenseId) && l.DebitedAmount > 0).SingleOrDefaultAsync();
                                if (userCheck != null)
                                {
                                    var a = userExpenseList.Where(ue => ue.Id.Equals(userCheck.UserId)).FirstOrDefault();
                                    if (a == null)
                                    {
                                        //UserExpense userExpense = new UserExpense()
                                        //{
                                        //    Id = userCheck.UserId,
                                        //    Name = _db.Users.Where(us => us.Id.Equals(userCheck.UserId)).Select(s => s.FirstName).FirstOrDefault(),
                                        //    Amount = -userCheck.DebitedAmount
                                        //};

                                        UserExpense userExpense = _mapper.Map<UserExpense>(userCheck);
                                        userExpense.Name = await _db.Users.Where(us => us.Id.Equals(userCheck.UserId)).Select(s => s.FirstName).SingleOrDefaultAsync();

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
                var expense = await _db.Expenses.Where(e => e.Id.Equals(expenseId) && e.Description.Equals("Settle-Up") && e.IsDeleted.Equals(false)).SingleOrDefaultAsync();
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
                    var check = await _db.Ledgers.Where(l => l.ExpenseId.Equals(expenseId) && l.UserId.Equals(currentUserId) && l.DebitedAmount > 0).SingleOrDefaultAsync();
                    if(check != null)
                    {
                        var userCheck = await _db.Ledgers.Where(l => l.ExpenseId.Equals(expenseId) && l.UserId.Equals(userId.userId) && l.DebitedAmount < 0).SingleOrDefaultAsync();
                        if (userCheck != null)
                        {
                            var a = userExpenseList.Where(ue => ue.Id.Equals(userCheck.UserId)).FirstOrDefault();
                            if (a == null)
                            {
                                //UserExpense userExpense = new UserExpense()
                                //{
                                //    Id = userCheck.UserId,
                                //    Name = _db.Users.Where(us => us.Id.Equals(userCheck.UserId)).Select(s => s.FirstName).FirstOrDefault(),
                                //    Amount = -userCheck.DebitedAmount
                                //};

                                UserExpense userExpense = _mapper.Map<UserExpense>(userCheck);
                                userExpense.Name = await _db.Users.Where(us => us.Id.Equals(userCheck.UserId)).Select(s => s.FirstName).SingleAsync();

                                userExpenseList.Add(userExpense);
                            }
                            else
                            {
                                a.Amount = a.Amount + userCheck.DebitedAmount;
                            }
                        }
                    }

                    var check2 = await _db.Ledgers.Where(l => l.ExpenseId.Equals(expenseId) && l.UserId.Equals(currentUserId) && l.DebitedAmount < 0).SingleOrDefaultAsync();
                    if (check2 != null)
                    {
                        var userCheck = await _db.Ledgers.Where(l => l.ExpenseId.Equals(expenseId) && l.UserId.Equals(userId.userId) && l.DebitedAmount > 0).SingleOrDefaultAsync();
                        if (userCheck != null)
                        {
                            var a = userExpenseList.Where(ue => ue.Id.Equals(userCheck.UserId)).FirstOrDefault();
                            if (a == null)
                            {
                                //UserExpense userExpense = new UserExpense()
                                //{
                                //    Id = userCheck.UserId,
                                //    Name = _db.Users.Where(us => us.Id.Equals(userCheck.UserId)).Select(s => s.FirstName).FirstOrDefault(),
                                //    Amount = -userCheck.DebitedAmount
                                //};

                                UserExpense userExpense = _mapper.Map<UserExpense>(userCheck);
                                userExpense.Name = await _db.Users.Where(us => us.Id.Equals(userCheck.UserId)).Select(s => s.FirstName).SingleAsync();

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
            string AddedBy = await _db.Users.Where(u => u.Email.Equals(email.ToLower())).Select(s => s.Id).SingleAsync();

            //Expense expense = new Expense()
            //{
            //    AddedBy = AddedBy,
            //    CreatedOn = settleUp.Date,
            //    Description = "Settle-Up",
            //    ExpenseType = "Settle-Up",
            //    IsDeleted = false,
            //    Note = settleUp.Note,
            //    Amount = settleUp.Amount
            //};

            Expense expense = _mapper.Map<Expense>(settleUp);

            var addedExpense = _db.Expenses.AddAsync(expense);
            await _db.SaveChangesAsync();
            if (settleUp.Group != "")
            {
                GroupExpense groupExpense = new GroupExpense()
                {
                    ExpenseId = expense.Id,
                    GroupId = settleUp.Group
                };
                await _db.GroupExpenses.AddAsync(groupExpense);
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
                Log = await _db.Users.Where(u=>u.Id.Equals(settleUp.Payer)).Select(s=>s.FirstName).SingleAsync() + " Paid " + await _db.Users.Where(u => u.Id.Equals(settleUp.Recipient)).Select(s => s.FirstName).SingleAsync(),
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

            await _db.ActivityUsers.AddAsync(activityUser);
            await _db.ActivityUsers.AddAsync(activityUser2);

            await _db.Ledgers.AddAsync(ledgerPayer);
            await _db.Ledgers.AddAsync(ledgerRecipient);

        }

        public async Task UnDeleteExpense(string expenseId, string currentUserId)
        {
            var expense = await _db.Expenses.Where(e => e.Id.Equals(expenseId)).SingleOrDefaultAsync();
            if(expense!=null)
            {
                expense.IsDeleted = false;
            }

            var group = await _db.GroupExpenses.Where(g => g.ExpenseId.Equals(expenseId)).SingleOrDefaultAsync();
            Activity activity = new Activity
            {
                Log = _db.Users.Where(u => u.Id.Equals(currentUserId)).Select(s => s.FirstName).SingleAsync() + " unDeleted " + expense.Description,
                ActivityOn = group == null ? "Expense" : "Group",
                ActivityOnId = group == null ? expense.Id : group.GroupId,
                Date = DateTime.Now
            };

            await _db.Activities.AddAsync(activity);
        }
    }
}
