using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GestionDeTareas.Models
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public string[]? Roles { get; set; }

        public AuthorizeAttribute(params string[] roles)
        {
            Roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedObjectResult(new { mensaje = "No autorizado" });
                return;
            }

            if (Roles != null && Roles.Length > 0)
            {
                var userRoles = user.Claims
                    .Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();

                if (!Roles.Any(role => userRoles.Contains(role)))
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }
        }
    }

}
