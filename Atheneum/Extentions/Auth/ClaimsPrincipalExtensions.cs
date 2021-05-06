using Atheneum.Enums;
using System;
using System.Linq;
using System.Security.Claims;

namespace Atheneum.Extentions.Auth
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Получение идентификатора аутентифицированного пользователя
        /// </summary>
        public static Guid GetUserId(this ClaimsPrincipal principal)
        {
            if (principal == null) throw new ArgumentNullException(nameof(principal));

            var id = Guid.Parse(principal.FindFirst(ClaimTypes.NameIdentifier).Value);
            return id;
        }

        /// <summary>
        /// Получение идентификатора аутентифицированного пользователя
        /// </summary>
        public static bool HasRole(this ClaimsPrincipal principal, RoleEnum role)
        {
            if (principal == null) throw new ArgumentNullException(nameof(principal));

            var userRoles = principal.FindAll(ClaimTypes.Role).Select(x=> (RoleEnum)int.Parse(x.Value)).ToList();

            return userRoles.Contains(role);
        }
    }
}
