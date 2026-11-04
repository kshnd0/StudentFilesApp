using System.Collections.Generic;

namespace StudentFilesApp.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public string Head { get; set; }

        public ICollection<Specialty> Specialties { get; set; } = new List<Specialty>();
    }
}