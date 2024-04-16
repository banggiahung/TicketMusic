using TicketMusic.Data;


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
    }
}
