using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Splitwise.DomainModel.Models;
using Splitwise.Repository.DataRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Splitwise.Repository.ActivityRepository
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly SplitwiseDbContext _db;
        private readonly IMapper _mapper;
        private readonly IDataRepository _dal;

        public ActivityRepository(IMapper mapper, IDataRepository dal)
        {
            _mapper = mapper;
            _dal = dal;
        }

        public async Task<List<ActivityDetails>> ActivityList(string userId)
        {

            List<ActivityDetails> activityDetails = new List<ActivityDetails>();
            //List<Activity> activityList = await _db.Activities.ToListAsync();
            List<Activity> activityList = await _dal.Get<Activity>();
            foreach (var activities in activityList)
            {
                //var activityLog = await _db.ActivityUsers.Where(a => a.ActivityId.Equals(activities.Id) && a.ActivityUserId.Equals(userId)).ToListAsync();

                var activityLog = await _dal.Where<ActivityUser>(a => a.ActivityId.Equals(activities.Id) && a.ActivityUserId.Equals(userId)).ToListAsync();

                if (activityLog.FirstOrDefault() != null)
                {
                    foreach (var activityUsers in activityLog)
                    {
                        ActivityDetails activityDetail = _mapper.Map<ActivityDetails>(activities);
                        activityDetail.Log2 = activityUsers.Log;
                        activityDetails.Add(activityDetail);
                    }
                } else
                {
                    ActivityDetails activityDetail = _mapper.Map<ActivityDetails>(activities);
                    activityDetails.Add(activityDetail);
                }
            }
            return activityDetails;
        }
    }
}