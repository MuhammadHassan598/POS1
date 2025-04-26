
using Microsoft.EntityFrameworkCore;
using POS1.Data;
using POS1.Utilities;


namespace POS1.Services
{
    public class TenantServices
    {


        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public TenantServices(IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public List<User> GetAllUsers()
        {
            using var _context = _contextFactory.CreateDbContext();
            return _context.Users.ToList();
            }

         
    }

}
