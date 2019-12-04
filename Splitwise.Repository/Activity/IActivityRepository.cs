using Splitwise.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Splitwise.Repository.Activity
{
    public interface IActivityRepository
    {
        List<ActivityDetails> ActivityList(string userId);
        int DeleteActivity(string activityId);
    }
}
