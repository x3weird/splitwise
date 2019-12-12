using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Splitwise.DomainModel.Models;
using Splitwise.DomainModel.Models.ApplicationClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Splitwise.Repository.FriendRepository
{
    public class FriendRepository : IFriendRepository
    {
        private readonly SplitwiseDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public FriendRepository(SplitwiseDbContext db, UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _db = db;
            _userManager = userManager;
            _mapper = mapper;
        }
        public async Task<List<UserNameWithId>> GetFriendList(string userId)
        {
            var friendList = await _db.Friends.Where(f => f.FriendId.Equals(userId) || f.UserId.Equals(userId)).ToListAsync();
            List<string> friendListUserId = new List<string>();
            List<UserNameWithId> friendLists = new List<UserNameWithId>();
            foreach (var friend in friendList)
            {
                if (friend.UserId.Equals(userId))
                {
                    friendListUserId.Add(friend.FriendId);
                }
                else
                {
                    friendListUserId.Add(friend.UserId);
                }
            }
            var friendListWithName = await _db.Users.Join(friendListUserId,
                                                    u => u.Id,
                                                    f => f,
                                                    (u, f) => new
                                                    {
                                                        userId = f,
                                                        name = u.FirstName
                                                    }).ToListAsync();

            foreach (var friend in friendListWithName)
            {
                UserNameWithId friendListItem = new UserNameWithId
                {
                    Name = friend.name,
                    UserId = friend.userId
                };
                friendLists.Add(friendListItem);
            }
            return friendLists;
        }

        public async Task<string> InviteFriend(InviteFriend inviteFriend, string currentUserId)
        {

            for (int i = 0; i < inviteFriend.Email.Count(); i++)
            {
                var check = await _db.Users.Where(u => u.Email.Equals(inviteFriend.Email[i].ToLower())).Select(s => s.Id).FirstOrDefaultAsync();

                if (check != null)
                {
                    var checkFriend = _db.Friends.Where(f => (f.FriendId.Equals(check) && f.UserId.Equals(currentUserId)) || (f.FriendId.Equals(currentUserId) && f.UserId.Equals(check))).FirstOrDefault();

                    if (checkFriend == null)
                    {
                        Friend friend = new Friend
                        {
                            FriendId = check,
                            UserId = currentUserId
                        };
                        _db.Add(friend);
                    }
                }
                else
                {
                    var name = inviteFriend.Email[i];
                    int index = name.IndexOf('@');
                    if (index > 0)
                        name = name.Substring(0, index);
                    var user = new ApplicationUser
                    {
                        UserName = inviteFriend.Email[i],
                        Email = inviteFriend.Email[i],
                        FirstName = name,
                        LastName = inviteFriend.Email[i],
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

                    //sending mail code

                    string to = inviteFriend.Email[i]; //To address    
                    string from = "splitwise2364@gmail.com"; //From address 
                    string mailbody = "Hello there, you are invite to join splitwise, team splitwise :)";
                    MailMessage message = new MailMessage(from, to);
                    if (inviteFriend.Message != null)
                        mailbody = inviteFriend.Message;

                    message.Subject = "Invitation mail";
                    message.Body = mailbody;
                    message.BodyEncoding = Encoding.UTF8;
                    message.IsBodyHtml = true;
                    SmtpClient client = new SmtpClient("smtp.gmail.com", 587); //Gmail smtp    
                    System.Net.NetworkCredential basicCredential1 = new
                    System.Net.NetworkCredential("splitwise2364@gmail.com", "vjdjs@4021");
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = basicCredential1;

                    // Disabling certificate validation 

                    ServicePointManager.ServerCertificateValidationCallback =
                        delegate (
                            object s,
                            X509Certificate certificate,
                            X509Chain chain,
                            SslPolicyErrors sslPolicyErrors
                        ) {
                            return true;
                        };

                    try
                    {
                        client.Send(message);
                    }

                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            return "sent sucessfully";
        }

        public async Task<List<ExpenseDetail>> GetFriendExpenseList(string friendId, string email)
        {

            int flag = 0;
            List<string> expenseIdList = new List<string>();
            var user = await _userManager.FindByEmailAsync(email);
            foreach (var expense in _db.Expenses)
            {
                flag = 0;
                foreach (var ledger in _db.Ledgers.Where(l => l.ExpenseId.Equals(expense.Id)))
                {
                    if (ledger.UserId.Equals(user.Id))
                    {
                        flag++;
                    }
                    if (ledger.UserId.Equals(friendId))
                    {
                        flag++;
                    }
                }
                if (flag == 2)
                {
                    expenseIdList.Add(expense.Id);
                }
            }
            //var expenseIdList = _db.Ledgers.Where(l => l.UserId.Equals(user.Id)).Select(l => l.ExpenseId).Distinct();
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
                        expenseLedger.Name = userName.Where(u => u.Id.Equals(ledger.UserId)).Select(s => s.Name).FirstOrDefault();

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
                        //    Name = _db.Users.Where(u => u.Id.Equals(comment.UserId)).Select(s => s.FirstName).FirstOrDefault()
                        //};

                        CommentDetails commentDetail = _mapper.Map<CommentDetails>(comment);
                        commentDetail.Name = _db.Users.Where(u => u.Id.Equals(comment.UserId)).Select(s => s.FirstName).FirstOrDefault();

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
                    //    Description = expense.Description
                    //};

                    ExpenseDetail expenseDetail = _mapper.Map<ExpenseDetail>(expense);
                    expenseDetail.AddedBy = _db.Users.Where(u => u.Id.Equals(expense.AddedBy)).Select(s => s.FirstName).FirstOrDefault();
                    expenseDetail.ExpenseLedgers = ExpenseLedgerList;
                    expenseDetail.Comments = commentDetails;

                    ExpenseDetailList.Add(expenseDetail);
                }
            }

            return ExpenseDetailList;
        }

        public async Task<UserExpense> UserExpense(string userId)
        {
            var expenseIdList = _db.Ledgers.Where(l => l.UserId.Equals(userId)).Select(l => l.ExpenseId).Distinct();
            List<string> ledgerIdList = new List<string>();
            List<UserExpense> userExpenses = new List<UserExpense>();
            List<Ledger> ledgers = new List<Ledger>();
            foreach (var x in expenseIdList)
            {
                ledgers = await _db.Ledgers.Where(l => l.ExpenseId.Equals(x)).ToListAsync();
            }
            float sum = 0;
            foreach (var x in ledgers)
            {
                sum = +x.CreditedAmount;
                sum = -x.DebitedAmount;
            }
            UserExpense userExpense = new UserExpense()
            {
                Name = await _db.Users.Where(u => u.Id.Equals(userId)).Select(s => s.FirstName).SingleOrDefaultAsync(),
                Amount = sum,
                Id = userId
            };

            return userExpense;
        }

        public async Task RemoveFriend(string friendId, string userId)
        {
            var friendList = await _db.Friends.Where(f => (f.UserId.Equals(friendId) && f.FriendId.Equals(userId)) || (f.UserId.Equals(userId) && f.FriendId.Equals(friendId))).ToListAsync();
            _db.Friends.RemoveRange(friendList);
        }
    }
}
