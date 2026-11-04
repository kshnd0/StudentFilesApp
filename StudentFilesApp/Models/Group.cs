using System.Collections.Generic;

namespace StudentFilesApp.Models
{
    public class Group
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int Course { get; set; }
        public string FormOfStudy { get; set; }

        public int SpecialtyId { get; set; }
        public Specialty Specialty { get; set; }

        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}