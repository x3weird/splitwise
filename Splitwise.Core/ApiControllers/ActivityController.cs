using Microsoft.AspNetCore.Mvc;
using Splitwise.DomainModel.Models;
using Splitwise.Repository.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Splitwise.Core.ApiControllers
{
    [Route("api/activities")]
    [ApiController]
    public class ActivityController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ActivityController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<object> ActivityList()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            var activitiesList = await _unitOfWork.Activity.ActivityList(userId);
            return Ok(activitiesList);
        }

        [HttpDelete]
        public async Task<object> DeleteActivity(string activityId)
        {
            if (await _unitOfWork.Activity.DeleteActivity(activityId) == 1)
            {
                return Ok();
            }
            else
            {
                return NoContent();
            }
        }
    }
}
