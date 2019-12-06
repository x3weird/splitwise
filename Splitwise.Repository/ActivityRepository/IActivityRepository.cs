using Splitwise.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Splitwise.Repository.ActivityRepository
{
    public interface IActivityRepository
    {
        Task<List<ActivityDetails>> ActivityList(string userId);
        Task<int> DeleteActivity(string activityId);
    }
}
