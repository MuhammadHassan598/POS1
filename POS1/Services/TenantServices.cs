
using Microsoft.EntityFrameworkCore;
using POS1.Data;
using POS1.Utilities;


namespace POS1.Services
{
    public class TenantServices
    {



            private readonly ApplicationDbContext _context;

            public TenantServices(ApplicationDbContext context)
            {
                _context = context;
            }

            public List<User> GetAllUsers()
            {
                return _context.Users.ToList();
            }

         
    }

}
