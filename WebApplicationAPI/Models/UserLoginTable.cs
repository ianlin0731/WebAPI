using System;
using System.Collections.Generic;

namespace WebApplicationAPI.Models
{
    public partial class UserLoginTable
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string UserPassword { get; set; } = null!;
        public int? UserRank { get; set; }
        public string? UserApproved { get; set; }
    }
}
