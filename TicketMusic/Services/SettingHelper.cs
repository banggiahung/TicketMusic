using TicketMusic.Data;
using TicketMusic.Models;

namespace TicketMusic.Services
{
    public class SettingHelper
    {
        private readonly ApplicationDbContext _context;

        public SettingHelper(ApplicationDbContext context)
        {
            _context = context;
        }

        public string GetValue(string key)
        {
           
            return "";
        }

        public ApplicationUser GetUser(string UserId)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == UserId);
            if (user == null)
            {
                return null;

            }
            return user;
        }
    }
}
