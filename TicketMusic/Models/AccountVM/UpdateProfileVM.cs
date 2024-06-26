﻿namespace TicketMusic.Models.AccountVM
{
    public class UpdateProfileVM
    {
        public string? UserName { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? InvitedCode { get; set; }
        public string? Address { get; set; }
        public IFormFile? AvatarFile { get; set; }
    }
}
