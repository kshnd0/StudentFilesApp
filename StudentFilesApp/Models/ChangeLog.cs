using System;

namespace StudentFilesApp.Models
{
    public class ChangeLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public string Entity { get; set; }
        public int EntityId { get; set; }
        public string Action { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}