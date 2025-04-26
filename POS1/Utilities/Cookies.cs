
using Microsoft.AspNetCore.Http;
using POS1.Data;
using POS1.Services;
using System.Threading.Tasks;



namespace POS1.Utilities
{
    public static class CookieUtility
    {
        public static async Task<string> GetUserFromCookieAsync(IHttpContextAccessor httpContextAccessor)
        {
            // Retrieve the username from the cookie
            var userName = httpContextAccessor.HttpContext.Request.Cookies["LoginName"];

            if (string.IsNullOrEmpty(userName))
            {

                return "Guest" ;
            }
            return userName;
        }
      
    }
}
