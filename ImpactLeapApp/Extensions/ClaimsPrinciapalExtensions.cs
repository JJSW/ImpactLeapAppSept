using System;
using System.Security.Claims;

namespace ImpactLeapApp.Common
{
    // Get user id easily without going though Claims
    public static class ClaimsPrinciapalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }
            var claim = principal.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? claim.Value : null;
        }
    }
}
