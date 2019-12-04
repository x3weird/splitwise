using Splitwise.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Splitwise.Repository.Activity
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly SplitwiseDbContext _db;

        public ActivityRepository(SplitwiseDbContext db)
        {
            _db = db;
        }

        public List<ActivityDetails> ActivityList(string userId)
        {

            List<ActivityDetails> activityDetails = new List<ActivityDetails>();
            foreach (var activities in _db.Activities)
            {
                foreach (var activityUsers in _db.ActivityUsers.Where(a => a.ActivityId.Equals(activities.Id) && a.ActivityUserId.Equals(userId)))
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
                }
            }
            return activityDetails;
        }

        public int DeleteActivity(string activityId)
        {
            var activity = _db.Activities.Where(a => a.Id.Equals(activityId)).FirstOrDefault();
            if(activity != null)
            {
                _db.Activities.Remove(activity);
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}