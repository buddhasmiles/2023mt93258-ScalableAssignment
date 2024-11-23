using Microsoft.AspNetCore.Mvc.Filters;
using Services;

namespace AProductService.Helpers
{
    /// <summary>
    /// Gives access only if the user is an admin
    /// </summary>

    public class AdminAccessAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string? role = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type ==
            "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;

            // Returns 403 status code if the user is not an admin

            if (role == "Customer")
            {
                throw new ForbiddenException("Sorry, only admins has access to this resource :(");
            }
        }
    }
}
