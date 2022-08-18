using System;
using System.Collections.Generic;

namespace WebApplicationAPI.Models
{
    public partial class ToDoListTable
    {
        public int Id { get; set; }
        public string Task { get; set; } = null!;
    }
}
