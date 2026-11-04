using System.Collections.Generic;

namespace StudentFilesApp.Models
{
    public class Specialty
    {
        public int SpecialtyId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }

        public ICollection<Group> Groups { get; set; } = new List<Group>();
    }
}