using Microsoft.AspNetCore.Mvc;
using Splitwise.DomainModel.Models;
using Splitwise.Repository.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

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
        public object ActivityList()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            var activitiesList = _unitOfWork.Activity.ActivityList(userId);
            return Ok(activitiesList);
        }

        [HttpDelete]
        public object DeleteActivity(string activityId)
        {
            if(_unitOfWork.Activity.DeleteActivity(activityId) == 1)
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
