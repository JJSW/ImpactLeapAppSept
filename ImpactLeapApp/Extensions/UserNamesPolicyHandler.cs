using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImpactLeapApp.Extensions
{
    public class UserNamesPolicyHandler : AuthorizationHandler<UserNamesRequirement>
    {

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserNamesRequirement requirement)
        {
            var userName = context.User.Identity.Name;

            if (requirement.UserNames.ToList().Contains(userName))
                context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }

    public class UserNamesRequirement : IAuthorizationRequirement
    {
        public string[] UserNames { get; set; }

        public UserNamesRequirement(params string[] userNames)
        {
            UserNames = userNames;
        }
    }
}
