using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Splitwise.DomainModel.Models;
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

        public ActivityRepository(SplitwiseDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<List<ActivityDetails>> ActivityList(string userId)
        {

            List<ActivityDetails> activityDetails = new List<ActivityDetails>();
            foreach (var activities in _db.Activities)
            {
                var activityLog = await _db.ActivityUsers.Where(a => a.ActivityId.Equals(activities.Id) && a.ActivityUserId.Equals(userId)).ToListAsync();
                
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