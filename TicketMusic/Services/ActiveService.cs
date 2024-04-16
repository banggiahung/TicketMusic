using Hangfire;
using TicketMusic.Data;
using System.Net.Http;

namespace TicketMusic.Services
{
    public class ActiveService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public ActiveService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
      

    }
}
