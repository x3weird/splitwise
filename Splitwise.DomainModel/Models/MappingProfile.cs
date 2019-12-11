using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Splitwise.DomainModel.Models
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<Activity, ActivityDetails>();

            CreateMap<CommentData, Comment>()
                .ForMember(dest =>
                dest.CommentData,
                opt => opt.MapFrom(src => src.Content))
                .ForMember(dest =>
                dest.CreatedOn,
                opt => opt.MapFrom(src => DateTime.Now));

            CreateMap<Ledger, ExpenseLedger>()
                .ForMember(dest =>
                dest.Owes,
                opt => opt.MapFrom(src => src.DebitedAmount))
                .ForMember(dest =>
                dest.Paid,
                opt => opt.MapFrom(src => src.CreditedAmount)
                );

            CreateMap<Expense, ExpenseDetail>()
                .ForMember( dest => 
                dest.ExpenseId,
                opt=>opt.MapFrom(src => src.Id));

            CreateMap<AddExpense, Expense>()
                .ForMember(dest =>
                dest.IsDeleted,
                opt => opt.MapFrom(src => false));

            CreateMap<SettleUp, Expense>()
                .ForMember(dest => 
                dest.CreatedOn,
                opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => 
                dest.Description,
                opt => opt.MapFrom(src => "Settle-Up"))
                .ForMember(dest =>
                dest.ExpenseType,
                opt => opt.MapFrom(src => "Settle-Up"))
                .ForMember(dest =>
                dest.IsDeleted,
                opt => opt.MapFrom(src => false));

            CreateMap<Ledger, UserExpense>()
                .ForMember(dest =>
                dest.Id,
                opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest =>
                dest.Amount,
                opt => opt.MapFrom(src => -src.DebitedAmount));

            CreateMap<GroupAdd, Group>()
                .ForMember(dest =>
                dest.CreatedOn,
                opt => opt.MapFrom(src=> DateTime.Now))
                .ForMember(dest =>
                dest.IsDeleted,
                opt => opt.MapFrom(src => false));

            CreateMap<Group, GroupDetails>();

            CreateMap<GroupMember, GroupUsers>();

            CreateMap<Activity, ActivityUser>()
                .ForMember(dest => 
                dest.ActivityId,
                opt => opt.MapFrom(src=>src.Id));
        }
    }
}
