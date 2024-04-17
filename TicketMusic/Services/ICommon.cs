using Microsoft.AspNetCore.Identity;
using System.Drawing;
using System.Collections;
using TicketMusic.Models;

namespace TicketMusic.Services
{
    public interface ICommon
    {
        Task<string> UploadedFile(IFormFile ProfilePicture);
        string GetSHA256(string str);
		string GetMD5(string str);

        void SendEmail(ApplicationUser user);

        Task<bool> AddUrlToSitemap(string url);

        Task<bool> RemoveUrlFromSitemap(string url);

        Task<bool> UpdateUrlInSitemap(string oldUrl, string newUrl);

        //void SendEmail(UserConfig user, int PayMoney);
        //void SendEmailUserNap(UserConfig user, int payMoney);

        //void SendEmail(DataUser request);
        //string GenerateToken();
        List<Categories> GetCategories();
        string GenerateRandomString(int lenght);

    }
}
