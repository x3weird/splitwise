using AutoMapper;
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
                var activityLog = _db.ActivityUsers.Where(a => a.ActivityId.Equals(activities.Id) && a.ActivityUserId.Equals(userId)).ToList();
                
                if (activityLog.FirstOrDefault() != null)
                {
                    foreach (var activityUsers in activityLog)
                    {
                        ActivityDetails activityDetail = _mapper.Map<ActivityDetails>(activities);
                        activityDetail.Log2 = activityUsers.Log;
                        activityDetails.Add(activityDetail);
                        await _db.SaveChangesAsync();
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