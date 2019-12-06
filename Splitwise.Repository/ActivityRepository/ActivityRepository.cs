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

        public ActivityRepository(SplitwiseDbContext db)
        {
            _db = db;
        }

        public async Task<List<ActivityDetails>> ActivityList(string userId)
        {

            List<ActivityDetails> activityDetails = new List<ActivityDetails>();
            foreach (var activities in _db.Activities)
            {
                var activityLog = _db.ActivityUsers.Where(a => a.ActivityId.Equals(activities.Id) && a.ActivityUserId.Equals(userId));
                if (activityLog != null)
                {
                    foreach (var activityUsers in activityLog)
                    {
                        ActivityDetails activityDetail = new ActivityDetails
                        {
                            Id = activities.Id,
                            Log = activities.Log,
                            ActivityOn = activities.ActivityOn,
                            ActivityOnId = activities.ActivityOnId,
                            Log2 = activityUsers.Log
                        };
                        activityDetails.Add(activityDetail);
                        await _db.SaveChangesAsync();
                    }
                } else
                {
                    ActivityDetails activityDetail = new ActivityDetails
                    {
                        Id = activities.Id,
                        Log = activities.Log,
                        ActivityOn = activities.ActivityOn,
                        ActivityOnId = activities.ActivityOnId,
                    };
                    activityDetails.Add(activityDetail);
                }
                
            }
            return activityDetails;
        }

        public async Task<int> DeleteActivity(string activityId)
        {
            var activity = _db.Activities.Where(a => a.Id.Equals(activityId)).FirstOrDefault();
            if (activity != null)
            {
                _db.Activities.Remove(activity);
                await _db.SaveChangesAsync();
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}