using System;
using System.ComponentModel.DataAnnotations;

namespace AS_Assigment2.Model
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Activity { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
