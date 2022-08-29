using System;
using System.Collections.Generic;

namespace WebApplicationAPI.Models
{
    public partial class UserTable
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string UserSex { get; set; } = null!;
        public DateTime UserBirthDay { get; set; }
        public string UserMobilePhone { get; set; } = null!;
    }
}
